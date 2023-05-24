using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PieViewer.Views.Controls;



/// <summary>
/// https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
/// https://stackoverflow.com/questions/42531608/hsv-triangle-in-c-sharp
/// </summary>
public sealed class HSVColorPicker : Control
{
    public int Size
    {
        get { return (int)GetValue(SizeProperty); }
        private set { SetValue(SizePropertyKey, value); }
    }

    private static readonly DependencyPropertyKey SizePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Size),
        typeof(int),
        typeof(HSVColorPicker),
        new FrameworkPropertyMetadata(150, FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty SizeProperty = SizePropertyKey.DependencyProperty;




    double hue = 0;
    double sat = 0.4;
    double val = 0.9;


    private Rect _rect;
    private Image _PART_Image;
    private Ellipse _PART_HueSelector;


    private int _halfSize;
    private int _halfSizeSquared;
    private int _innerRadius;
    private int _innerRadiusSquared;
    private double _angle = 0;
    private Point _center;
    private double _sqrt3;

    private double _cursorHalhSize;
    double _middleRadius;


    private int _triangleScale;
    private int _sizeScale;
    private int _halfSizeScaled;
    private int _innerRadiusScaled;


    private bool _isHueCircleCaptured;


    private int[,] boxBlur = new int[3, 3]
    {
        { 9, 9, 9 },
        { 9, 9, 9 },
        { 9, 9, 9 }
    };


    //private Point[] triangle = new Point[3] {
    //    new Point(101d, 416.5),
    //    new Point(499, 416.5),
    //    new Point(300d, 69.5),
    //};

    //private Point[] triangle = new Point[3] {
    //    new Point(101d, 416.5),
    //    new Point(300d, 69.5),
    //    new Point(499, 416.5),
    //};

    //private Point[] triangle = new Point[3] {
    //    new Point(300d, 69.5),
    //    new Point(101d, 416.5),
    //    new Point(499, 416.5),
    //};

    //private Point[] triangle = new Point[3] {
    //    new Point(300d, 69.5),
    //    new Point(499, 416.5),
    //    new Point(101d, 416.5),
    //};

    //private Point[] triangle = new Point[3] {
    //    new Point(499, 416.5),
    //    new Point(300d, 69.5),
    //    new Point(101d, 416.5),
    //};

    //private Point[] triangle = new Point[3] {
    //    new Point(499, 416.5),
    //    new Point(101d, 416.5),
    //    new Point(300d, 69.5),
    //};

    private Point[] triangle = new Point[3] {
        new Point(75, 17.4),
        new Point(124.8, 104.1),
        new Point(25.3, 104.1),
    };


    private double triangleArea;

    WriteableBitmap triBitmap;




    //public enum Area
    //{
    //    Outside,
    //    Wheel,
    //    Triangle
    //}

    //public struct PickResult
    //{
    //    public Area Area { get; set; }
    //    public double? Hue { get; set; }
    //    public double? Sat { get; set; }
    //    public double? Val { get; set; }
    //}


    #region Constructor

    static HSVColorPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HSVColorPicker), new FrameworkPropertyMetadata(typeof(HSVColorPicker)));
    }

    


    public HSVColorPicker()
    {
        MinWidth = Size + 20;
        MinHeight = Size + 20;


        _rect = new(0, 0, Size, Size);
        _halfSize = Size / 2;
        _halfSizeSquared = _halfSize * _halfSize;
        _innerRadius = _halfSize - 16;
        _innerRadiusSquared = _innerRadius * _innerRadius;
        _center = new(_halfSize, _halfSize);
        _middleRadius = (_innerRadius + _halfSize) / 2;
        _sqrt3 = Math.Sqrt(3);

        _triangleScale = 4;
        _sizeScale = Size * _triangleScale;
        _halfSizeScaled = _halfSize * _triangleScale;
        _innerRadiusScaled = _innerRadius * _triangleScale;

        //_hueCirle = new WriteableBitmap(Size, Size, 96, 96, PixelFormats.Bgra32, null);



        BitmapSource = new WriteableBitmap(Size, Size, 96, 96, PixelFormats.Bgra32, null);


        //Area = 0.5 * (-p1y * p2x + p0y * (-p1x + p2x) + p0x * (p1y - p2y) + p1x * p2y);
        triangleArea = 0.5 * (-triangle[1].Y * triangle[2].X + triangle[0].Y * (-triangle[1].X + triangle[2].X) + triangle[0].X * (triangle[1].Y - triangle[2].Y) + triangle[1].X * triangle[2].Y);

        //triBitmap = BitmapFactory.New(_sizeScale, _sizeScale);
        triBitmap = new WriteableBitmap(_sizeScale, _sizeScale, 96d, 96d, PixelFormats.Bgra32, null);

        MouseDown += HSVColorPicker_MouseDown;
        MouseLeftButtonDown += HSVColorPicker_MouseLeftButtonDown;
        MouseLeftButtonUp += HSVColorPicker_MouseLeftButtonUp;
        MouseMove += HSVColorPicker_MouseMove;
    }

    private void HSVColorPicker_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Point p = e.GetPosition(_PART_Image);
        if (IsPointInHueCircle(p, _center, _halfSizeSquared, _innerRadiusSquared))
        {
            _isHueCircleCaptured = true;
            double angle = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
            if (angle < 0)
                angle += 2 * Math.PI;
            _angle = angle;
            SetHueCursorPosition();
            DrawTriangle();

        }
    }

    private void HSVColorPicker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isHueCircleCaptured = false;
    }

    private void HSVColorPicker_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (_isHueCircleCaptured)
        {
            Point p = e.GetPosition(_PART_Image);
            double angle = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
            if (angle < 0)
                angle += 2 * Math.PI;
            _angle = angle;
            SetHueCursorPosition();
            DrawTriangle();
        }
    }

    private void HSVColorPicker_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            Point p = e.GetPosition(_PART_Image);
            if (IsPointInHueCircle(p, _center, _halfSizeSquared, _innerRadiusSquared))
            {
                double angle = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
                if (angle < 0)
                    angle += 2 * Math.PI;
                _angle = angle;
                SetHueCursorPosition();
                DrawTriangle();

            }
        }
    }

    #endregion Constructor


    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_Image") is Image image)
        {
            _PART_Image = image;
            DrawTriangle();
            _PART_Image.Source = triBitmap;
        }
        if (GetTemplateChild("PART_HueSelector") is Ellipse ellipse)
        {
            _PART_HueSelector = ellipse;
            _cursorHalhSize = ellipse.Width / 2;
            SetHueCursorPosition();
        }
    }


    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
    }


    public void SetHueCursorPosition()
    {
        Canvas.SetLeft(_PART_HueSelector, _halfSize + _middleRadius * Math.Sin(_angle) - _cursorHalhSize);
        Canvas.SetTop(_PART_HueSelector, _halfSize - _middleRadius * Math.Cos(_angle) - _cursorHalhSize);
    }


    private bool IsPointInHueCircle(Point point, Point center, int halfSizeSquared, int innerRadiusSquared)
    {
        double distanceSquared = (center - point).LengthSquared;
        return distanceSquared < halfSizeSquared && distanceSquared > innerRadiusSquared;
    }


    public static bool PointInTriangle(Point p, Point p0, Point p1, Point p2)
    {
        var s = (p0.X - p2.X) * (p.Y - p2.Y) - (p0.Y - p2.Y) * (p.X - p2.X);
        var t = (p1.X - p0.X) * (p.Y - p0.Y) - (p1.Y - p0.Y) * (p.X - p0.X);

        if ((s < 0) != (t < 0) && s != 0 && t != 0)
            return false;

        var d = (p2.X - p1.X) * (p.Y - p1.Y) - (p2.Y - p1.Y) * (p.X - p1.X);
        return d == 0 || (d < 0) == (s + t <= 0);
    }

    private bool PointInTriangle2(Point p, double x1, double y1)
    {
        return !(2 * y1 > 1 || _sqrt3 * x1 + (-1) * y1 > 1 || -_sqrt3 * x1 + (-1) * y1 > 1);
    }


    private void DrawTriangle()
    {
        //triBitmap.Clear(Colors.Transparent);

        triBitmap.Clear();
        int stride = triBitmap.BackBufferStride;
        int bufferSize = _sizeScale * stride;
        byte[] pixels = new byte[bufferSize];

        int i = 0;
        //Parallel.For(0, _sizeScale, y =>
        //{
        //    Parallel.For(0, _sizeScale, x =>
        //    {
        //        var rotatedX = Math.Cos(-_angle) * (x - _halfSizeScaled) - Math.Sin(-_angle) * (y - _halfSizeScaled) + _halfSizeScaled;
        //        var rotatedY = Math.Sin(-_angle) * (x - _halfSizeScaled) + Math.Cos(-_angle) * (y - _halfSizeScaled) + _halfSizeScaled;

        //        Point point = new(rotatedX, rotatedY);
        //        var x1 = (rotatedX - _halfSizeScaled) * 1.0 / _innerRadiusScaled;
        //        var y1 = (rotatedY - _halfSizeScaled) * 1.0 / _innerRadiusScaled;

        //        Color color;
        //        if (PointInTriangle2(point, x1, y1))
        //        {
        //            var sat = (1 - 2 * y1) / (_sqrt3 * x1 - y1 + 2);
        //            var val = (_sqrt3 * x1 - y1 + 2) / 3;

        //            color = HSV(_angle, sat, val, 1);

        //        }
        //        else
        //        {
        //            color = Colors.Transparent;
        //        }

        //        pixels[i * 4] = color.B;
        //        pixels[i * 4 + 1] = color.G;
        //        pixels[i * 4 + 2] = color.R;
        //        pixels[i * 4 + 3] = color.A;

        //        i++;
        //    });
        //});


        for (int y = 0; y < _sizeScale; y++)
        {
            for (int x = 0; x < _sizeScale; x++)
            {
                var rotatedX = Math.Cos(-_angle) * (x - _halfSizeScaled) - Math.Sin(-_angle) * (y - _halfSizeScaled) + _halfSizeScaled;
                var rotatedY = Math.Sin(-_angle) * (x - _halfSizeScaled) + Math.Cos(-_angle) * (y - _halfSizeScaled) + _halfSizeScaled;

                Point point = new(rotatedX, rotatedY);
                var x1 = (rotatedX - _halfSizeScaled) * 1.0 / _innerRadiusScaled;
                var y1 = (rotatedY - _halfSizeScaled) * 1.0 / _innerRadiusScaled;

                Color color;
                if (PointInTriangle2(point, x1, y1))
                {
                    var sat = (1 - 2 * y1) / (_sqrt3 * x1 - y1 + 2);
                    var val = (_sqrt3 * x1 - y1 + 2) / 3;

                    color = HSV(_angle, sat, val, 1);
                }
                else
                {
                    color = Colors.Transparent;
                }

                pixels[i * 4] = color.B;
                pixels[i * 4 + 1] = color.G;
                pixels[i * 4 + 2] = color.R;
                pixels[i * 4 + 3] = color.A;
                i++;
            }
        }

        triBitmap.FromByteArray(pixels, 0, bufferSize);
    }



    private void DrawHueCircle()
    {
        int scaledSize = 600;
        _hueCirle = BitmapFactory.New(scaledSize, scaledSize);
        _hueCirle.Clear(Colors.Transparent);

        int halfSize = 300;
        int halfSizeSquared = 90000;
        int innerRadius = 236;
        int innerRadiusSquared = 55696;
        Point center = new(halfSize, halfSize);


        for (int y = 0; y < scaledSize; y++)
        {
            for (int x = 0; x < scaledSize; x++)
            {
                Point point = new(x, y);
                if (IsPointInHueCircle(point, center, halfSizeSquared, innerRadiusSquared))
                {
                    double angle = Math.Atan2(y - halfSize, x - halfSize) + Math.PI / 2;
                    if (angle < 0)
                        angle += 2 * Math.PI;
                    Color color = HSV(angle, 1, 1, 1);
                    _hueCirle.SetPixel(x, y, color);
                }
            }
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    _hueCirle.DrawEllipseCentered((int)center.X, (int)center.Y, halfSize - i, halfSize - i, Colors.Black);
        //    _hueCirle.DrawEllipseCentered((int)center.X, (int)center.Y, innerRadius - i, innerRadius - i, Colors.Black);
        //}

        //_hueCirle.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
        FileStream stream = new("D:/Travail/Code/CSharp/Persos/PieViewer/temp.tga", FileMode.CreateNew , FileAccess.ReadWrite);
        _hueCirle.WriteTga(stream);

        //_PART_Image.Source = _hueCirle;
    }



    //private void DrawControl()
    //{
    //    for (int y = 0; y < Size; y++)
    //    {
    //        for (int x = 0; x < Size; x++)
    //        {
    //            Color color;
    //            var result = Pick(x, y);
    //            if (result.Area == Area.Outside)
    //            {
    //                // Outside
    //                color = Colors.Transparent;
    //            }
    //            else if (result.Area == Area.Wheel)
    //            {
    //                // Wheel
    //                color = HSV(result.Hue.Value, 1, 1, 1);
    //            }
    //            else
    //            {
    //                // Triangle
    //                color = HSV(hue, result.Sat.Value, result.Val.Value, 1);
    //            }
    //            //DrawPixel(x, y, color);
    //        }
    //    }


    //}


    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        //var img = new Bitmap(_size, _size, PixelFormat.Format32bppArgb);


        //drawingContext.DrawRectangle

        


        //drawingContext.DrawImage(_hueCirle, _rect);
        //drawingContext.Dra

        //Pen shapeOutlinePen = new(Brushes.Black, 1);
        //drawingContext.DrawEllipse(null, shapeOutlinePen, _center, _halfSize, _halfSize);
        //drawingContext.DrawEllipse(null, shapeOutlinePen, _center, _innerRadius, _innerRadius);
    }


    private Color HSV(double hue, double sat, double val, double alpha)
    {
        var chroma = val * sat;
        var step = Math.PI / 3;
        var interm = chroma * (1 - Math.Abs((hue / step) % 2.0 - 1));
        var shift = val - chroma;
        if (hue < 1 * step) return RGB(shift + chroma, shift + interm, shift + 0, alpha);
        if (hue < 2 * step) return RGB(shift + interm, shift + chroma, shift + 0, alpha);
        if (hue < 3 * step) return RGB(shift + 0, shift + chroma, shift + interm, alpha);
        if (hue < 4 * step) return RGB(shift + 0, shift + interm, shift + chroma, alpha);
        if (hue < 5 * step) return RGB(shift + interm, shift + 0, shift + chroma, alpha);
        return RGB(shift + chroma, shift + 0, shift + interm, alpha);
    }

    private Color RGB(double red, double green, double blue, double alpha)
    {
        Color color = Color.FromArgb(
            (byte)Math.Min(255, (alpha * 256)),
            (byte)Math.Min(255, (red * 256)),
            (byte)Math.Min(255, (green * 256)),
            (byte)Math.Min(255, (blue * 256)));

        return color;
    }

    private void DrawPixel(WriteableBitmap bitmap, int column, int row, Color color)
    {
        //int column = (int)e.GetPosition(i).X;
        //int row = (int)e.GetPosition(i).Y;

        try
        {
            // Reserve the back buffer for updates.
            bitmap.Lock();

            // Get a pointer to the back buffer.
            nint pBackBuffer = bitmap.BackBuffer;

            // Find the address of the pixel to draw.
            pBackBuffer += row * bitmap.BackBufferStride;
            pBackBuffer += column * 4;

            // Compute the pixel's color.
            int colorData = color.A << 24; // A
            colorData |= color.R << 16; // R
            colorData |= color.G << 8; // G
            colorData |= color.B << 0; // B

            // Assign the color data to the pixel.
            //*((int*)pBackBuffer) = colorData;
            Marshal.WriteInt32(pBackBuffer, colorData);

            // Specify the area of the BitmapSource that changed.
            bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
        }
        finally
        {
            // Release the back buffer and make it available for display.
            bitmap.Unlock();
        }
    }


    //public void DrawRectangle(WriteableBitmap writeableBitmap, int left, int top, int width, int height, System.Windows.Media.Color color)
    //{
    //    // Compute the pixel's color
    //    int colorData = color.R << 16; // R
    //    colorData |= color.G << 8; // G
    //    colorData |= color.B << 0; // B
    //    int bpp = writeableBitmap.Format.BitsPerPixel / 8;

    //    for (int y = 0; y < height; y++)
    //    {
    //        // Get a pointer to the back buffer
    //        nint pBackBuffer = writeableBitmap.BackBuffer;

    //        // Find the address of the pixel to draw
    //        pBackBuffer += (top + y) * writeableBitmap.BackBufferStride;
    //        pBackBuffer += left * bpp;

    //        for (int x = 0; x < width; x++)
    //        {
    //            // Assign the color data to the pixel
    //            Marshal.WriteInt32(pBackBuffer, colorData);

    //            // Increment the address of the pixel to draw
    //            pBackBuffer += bpp;
    //        }
    //    }

    //    writeableBitmap.AddDirtyRect(new Int32Rect(left, top, width, height));
    //}

    //public PickResult Pick(double x, double y)
    //{
    //    var distanceFromCenter = Math.Sqrt((x - CenterX) * (x - CenterX) + (y - CenterY) * (y - CenterY));
    //    var sqrt3 = Math.Sqrt(3);
    //    if (distanceFromCenter > OuterRadius)
    //    {
    //        // Outside
    //        return new PickResult { Area = Area.Outside };
    //    }
    //    else if (distanceFromCenter > InnerRadius)
    //    {
    //        // Wheel
    //        var angle = Math.Atan2(y - CenterY, x - CenterX) + Math.PI / 2;
    //        if (angle < 0) angle += 2 * Math.PI;
    //        var hue = angle;
    //        return new PickResult { Area = Area.Wheel, Hue = hue };
    //    }
    //    else
    //    {
    //        // Inside
    //        var x1 = (x - CenterX) * 1.0 / InnerRadius;
    //        var y1 = (y - CenterY) * 1.0 / InnerRadius;
    //        if (0 * x1 + 2 * y1 > 1) return new PickResult { Area = Area.Outside };
    //        else if (sqrt3 * x1 + (-1) * y1 > 1) return new PickResult { Area = Area.Outside };
    //        else if (-sqrt3 * x1 + (-1) * y1 > 1) return new PickResult { Area = Area.Outside };
    //        else
    //        {
    //            // Triangle
    //            var sat = (1 - 2 * y1) / (sqrt3 * x1 - y1 + 2);
    //            var val = (sqrt3 * x1 - y1 + 2) / 3;

    //            return new PickResult { Area = Area.Triangle, Sat = sat, Val = val };
    //        }
    //    }
    //}



}
