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




    public double Hue
    {
        get { return (double)GetValue(HueProperty); }
        set { SetValue(HueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HueProperty =
        DependencyProperty.Register(nameof(Hue), typeof(double), typeof(HSVColorPicker), new PropertyMetadata(0d));

    public double Saturation
    {
        get { return (double)GetValue(SaturationProperty); }
        set { SetValue(SaturationProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Saturation.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SaturationProperty =
        DependencyProperty.Register(nameof(Saturation), typeof(double), typeof(HSVColorPicker), new PropertyMetadata(1d));

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(HSVColorPicker), new PropertyMetadata(1d));




    //public double Hue { get; set; } = 1;
    //public double Saturation { get; set; } = 0.5;
    //public double Value { get; set; } = 0.5;
    //public double Alpha { get; set; } = 1;
    public Color RGBA { get; set; }


    //double hue = 0;
    //double sat = 1;
    //double val = 1;


    
    private Image? _PART_Image;
    private Ellipse? _PART_HCursor;
    private Ellipse? _PART_SLCursor;

    private Rect _rect;
    private int _halfSize;
    private int _halfSizeSquared;
    private int _innerRadius;
    private int _innerRadiusSquared;
    private Point _center;
    private double _sqrt3;

    private double _cursorHalhSize = 6;
    private double _middleRadius;


    private int _triangleScale;
    private int _sizeScale;
    private int _halfSizeScaled;
    private int _innerRadiusScaled;


    private bool _isCircleCaptured;
    private bool _isTriangleCaptured;


    private WriteableBitmap _triangleBitmap;




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


        _triangleBitmap = new WriteableBitmap(_sizeScale, _sizeScale, 96d, 96d, PixelFormats.Bgra32, null);

        MouseLeftButtonDown += HSVColorPicker_MouseLeftButtonDown;
        MouseLeftButtonUp += HSVColorPicker_MouseLeftButtonUp;
        MouseMove += HSVColorPicker_MouseMove;
    }

    #endregion Constructor


    #region Mouse Events

    private void HSVColorPicker_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Point p = e.GetPosition(_PART_Image);
        if (IsPointInHueCircle(p, _center, _halfSizeSquared, _innerRadiusSquared))
        {
            _isCircleCaptured = true;
            double angle = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
            if (angle < 0)
                angle += 2 * Math.PI;
            Hue = angle;
            SetHCursorPosition();
            DrawTriangle();
            return;
        }


        //var rotatedX = Math.Cos(-Hue) * (x - _halfSizeScaled) - Math.Sin(-Hue) * (y - _halfSizeScaled) + _halfSizeScaled;
        //var rotatedY = Math.Sin(-Hue) * (x - _halfSizeScaled) + Math.Cos(-Hue) * (y - _halfSizeScaled) + _halfSizeScaled;


        double x1 = (p.X - _halfSize) * 1.0 / _innerRadius;
        double y1 = (p.Y - _halfSize) * 1.0 / _innerRadius;
        if (IsPointInTriangle(p, x1, y1))
        {
            _isTriangleCaptured = true;
            Saturation = (1 - 2 * y1) / (_sqrt3 * x1 - y1 + 2);
            Value = (_sqrt3 * x1 - y1 + 2) / 3;
            SetSLCursorPosition();
        }
    }

    private void HSVColorPicker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isCircleCaptured = false;
        _isTriangleCaptured = false;
    }

    private void HSVColorPicker_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Point p = e.GetPosition(_PART_Image);
        if (_isCircleCaptured)
        {
            double angle = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
            if (angle < 0)
                angle += 2 * Math.PI;
            Hue = angle;
            SetHCursorPosition();
            DrawTriangle();
            return;
        }

        double x1 = (p.X - _halfSize) * 1.0 / _innerRadius;
        double y1 = (p.Y - _halfSize) * 1.0 / _innerRadius;
        if (_isTriangleCaptured && IsPointInTriangle(p, x1, y1))
        {
            
            Saturation = (1 - 2 * y1) / (_sqrt3 * x1 - y1 + 2);
            Value = (_sqrt3 * x1 - y1 + 2) / 3;
            SetSLCursorPosition();
            Debug.WriteLine(Saturation);
            Debug.WriteLine(Value);
            Debug.WriteLine("");
        }
    }

    #endregion Mouse Events



    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_Image") is Image image)
        {
            _PART_Image = image;
            DrawTriangle();
            _PART_Image.Source = _triangleBitmap;
        }
        if (GetTemplateChild("PART_HCursor") is Ellipse hCursor)
        {
            _PART_HCursor = hCursor;
            SetHCursorPosition();
        }
        if (GetTemplateChild("PART_SLCursor") is Ellipse sLCursor)
        {
            _PART_SLCursor = sLCursor;
            SetSLCursorPosition();
        }
    }


    private void SetHCursorPosition()
    {
        double x = _halfSize + _middleRadius * Math.Sin(Hue) - _cursorHalhSize;
        double y = _halfSize - _middleRadius * Math.Cos(Hue) - _cursorHalhSize;
        Canvas.SetLeft(_PART_HCursor, x);
        Canvas.SetTop(_PART_HCursor, y);
    }

    private void SetSLCursorPosition()
    {
        double x = _halfSize + _innerRadius * (2 * Value - Saturation * Value - 1) * _sqrt3 / 2 - _cursorHalhSize;
        double y = _halfSize + _innerRadius * (1 - 3 * Saturation * Value) / 2 - _cursorHalhSize;
        Canvas.SetLeft(_PART_SLCursor, x);
        Canvas.SetTop(_PART_SLCursor, y);
    }


    private bool IsPointInHueCircle(Point point, Point center, int halfSizeSquared, int innerRadiusSquared)
    {
        double distanceSquared = (center - point).LengthSquared;
        return distanceSquared < halfSizeSquared && distanceSquared > innerRadiusSquared;
    }


    private bool IsPointInTriangle(Point p, double x1, double y1)
    {
        //return !(2 * y1 > 1 || _sqrt3 * x1 + (-1) * y1 > 1 || -_sqrt3 * x1 + (-1) * y1 > 1);
        //if (2 * y1 > 1) return false;
        //if (_sqrt3 * x1 + (-1) * y1 > 1) return false;
        //if (-_sqrt3 * x1 + (-1) * y1 > 1) return false;

        if (2 * y1 > 1) return false;
        if (_sqrt3 * x1 + -y1 > 1) return false;
        if (-_sqrt3 * x1 + -y1 > 1) return false;

        return true;
    }


    private void DrawTriangle()
    {
        _triangleBitmap.Clear();
        int stride = _triangleBitmap.BackBufferStride;
        int bufferSize = _sizeScale * stride;
        byte[] pixels = new byte[bufferSize];
        int i = 0;
        for (int y = 0; y < _sizeScale; y++)
        {
            for (int x = 0; x < _sizeScale; x++)
            {
                //var rotatedX = Math.Cos(-Hue) * (x - _halfSizeScaled) - Math.Sin(-Hue) * (y - _halfSizeScaled) + _halfSizeScaled;
                //var rotatedY = Math.Sin(-Hue) * (x - _halfSizeScaled) + Math.Cos(-Hue) * (y - _halfSizeScaled) + _halfSizeScaled;
                int rotatedX = x;
                int rotatedY = y;

                Point point = new(rotatedX, rotatedY);
                double x1 = (rotatedX - _halfSizeScaled) * 1.0 / _innerRadiusScaled;
                double y1 = (rotatedY - _halfSizeScaled) * 1.0 / _innerRadiusScaled;

                Color color;
                if (IsPointInTriangle(point, x1, y1))
                {
                    double sat = (1 - 2 * y1) / (_sqrt3 * x1 - y1 + 2);
                    double val = (_sqrt3 * x1 - y1 + 2) / 3;

                    //Debug.WriteLine(sat);

                    color = HSV(Hue, sat, val, 1);
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

        _triangleBitmap.FromByteArray(pixels, 0, bufferSize);
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
