using PieViewer.Core.Color;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PieViewer.Views.Controls
{
    internal sealed class HSLColorPicker : Control
    {
        private Ellipse? _PART_HCursor;
        private Ellipse? _PART_SLCursor;
        private Polygon? _PART_HTriangle;
        private Polygon? _PART_HTriangleOverlay;
        private Image? _PART_HueCircle;
        private readonly RotateTransform _triangleRotation = new();

        private readonly int _halfSize;
        private readonly int _halfSizeSquared;
        private readonly int _innerRadius;
        private readonly int _innerRadiusSquared;
        private readonly double _middleRadius;
        private double _triangleSide;
        private double _triangleHeight;
        private Point[] _triangle;
        private Point _center;
        
        private readonly double _degreeToRadian = Math.PI / 180;
        private readonly double _radianToDegree = 180 / Math.PI;
        private const double _cursorHalfSize = 6d;

        private bool _isHCursorCaptured = false;
        private bool _isSLCursorCaptured = false;

        private double _hueRadians = 0;

        #region Dependency Properties

        #region Size

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            private set { SetValue(SizePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey SizePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Size),
            typeof(int),
            typeof(HSLColorPicker),
            new FrameworkPropertyMetadata(150, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty SizeProperty = SizePropertyKey.DependencyProperty;

        #endregion Size


        /*

        #region Hue

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
            nameof(Hue), 
            typeof(double), 
            typeof(HSLColorPicker),
            new FrameworkPropertyMetadata(0d, OnHueChanged));

        private static void OnHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;
            control.ARGB = HSLToRGB(control.Hue, control.Saturation, control.Lightness, control.Alpha);
            control._hueRadians = (double)e.NewValue * control._degreeToRadian;
            control.SetHCursorPosition();
            control.SetSLCursorPosition();

            if (control._PART_HTriangle is not null)
            {
                control._PART_HTriangle.Fill = new SolidColorBrush(HSLToRGB(control.Hue, 1, 0.5, 1));
                control._triangleRotation.Angle = control.Hue;
            }
        }

        #endregion Hue

        #region Saturation

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Saturation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
            nameof(Saturation), 
            typeof(double), 
            typeof(HSLColorPicker), 
            new FrameworkPropertyMetadata(1d, OnSaturationChanged));

        private static void OnSaturationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;
            if (!control._isSLCursorCaptured)
                control.SetSLCursorPosition();
            control.ARGB = HSLToRGB(control.Hue, control.Saturation, control.Lightness, control.Alpha);
        }

        #endregion Saturation

        #region Lightness

        public double Lightness
        {
            get { return (double)GetValue(LightnessProperty); }
            set { SetValue(LightnessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lightness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LightnessProperty = DependencyProperty.Register(
            nameof(Lightness), 
            typeof(double), 
            typeof(HSLColorPicker), 
            new FrameworkPropertyMetadata(0.5d, OnLightnessChanged));

        private static void OnLightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;
            if (!control._isSLCursorCaptured)
                control.SetSLCursorPosition();
            control.ARGB = HSLToRGB(control.Hue, control.Saturation, control.Lightness, control.Alpha);
        }

        #endregion Lightness

        #region Alpha

        public double Alpha
        {
            get { return (double)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Alpha.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register(
            nameof(Alpha),
            typeof(double), 
            typeof(HSLColorPicker), 
            new FrameworkPropertyMetadata(1d, OnAlphaChanged));

        private static void OnAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;
            control.ARGB = HSLToRGB(control.Hue, control.Saturation, control.Lightness, control.Alpha);
        }

        #endregion Alpha


        */



        public HSLAColor HSLA
        {
            get { return (HSLAColor)GetValue(HSLAProperty); }
            set { SetValue(HSLAProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HSLA.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HSLAProperty =
            DependencyProperty.Register("HSLA", typeof(HSLAColor), typeof(HSLColorPicker), 
                new PropertyMetadata(new HSLAColor(), OnHSLAChanged));

        private static void OnHSLAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;
            HSLAColor hsla = (HSLAColor)e.NewValue;
            control.ARGB = ColorSpaceConversion.HslaToRgba(hsla);
            control._hueRadians = hsla.H * control._degreeToRadian;
            control.SetHCursorPosition();
            control.SetSLCursorPosition();

            if (control._PART_HTriangle is not null)
            {
                control._PART_HTriangle.Fill = new SolidColorBrush(ColorSpaceConversion.HslaToRgba(hsla.H, 1, 0.5, 1));
                control._triangleRotation.Angle = hsla.H;
            }
        }





        #region ARGB

        public Color ARGB
        {
            get { return (Color)GetValue(ARGBProperty); }
            set { SetValue(ARGBProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RGBA.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ARGBProperty = DependencyProperty.Register(
            nameof(ARGB), 
            typeof(Color), 
            typeof(HSLColorPicker), 
            new PropertyMetadata(default(Color), OnRGBAChanged));

        private static void OnRGBAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HSLColorPicker control = (HSLColorPicker)d;


        }

        #endregion ARGB

        #endregion Dependency Properties


        #region Constructor

        static HSLColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HSLColorPicker), new FrameworkPropertyMetadata(typeof(HSLColorPicker)));
        }

        public HSLColorPicker()
        {
            _halfSize = Size / 2;
            _halfSizeSquared = _halfSize * _halfSize;
            _innerRadius = _halfSize - 16;
            _innerRadiusSquared = _innerRadius * _innerRadius;
            _middleRadius = (_innerRadius + _halfSize) / 2;
            _center = new(_halfSize, _halfSize);

            SetTrianglePoints();

            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;
        }

        #endregion Constructor


        #region Initialisation

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_HueCircle") is Image hueCircle)
            {
                _PART_HueCircle = hueCircle;
                _PART_HueCircle.MouseLeftButtonDown += OnCircleMouseLeftButtonDown;
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

            if (GetTemplateChild("PART_HTriangle") is Polygon hTriangle && GetTemplateChild("PART_HTriangleOverlay") is Polygon hTriangleOverlay)
            {
                _PART_HTriangle = hTriangle;
                _PART_HTriangleOverlay = hTriangleOverlay;

                _PART_HTriangle.Points = new PointCollection(_triangle);
                _PART_HTriangleOverlay.Points = _PART_HTriangle.Points;

                Canvas.SetLeft(_PART_HTriangle, _halfSize);
                Canvas.SetTop(_PART_HTriangle, _halfSize);
                Canvas.SetLeft(_PART_HTriangleOverlay, _halfSize);
                Canvas.SetTop(_PART_HTriangleOverlay, _halfSize);

                _PART_HTriangle.MouseLeftButtonDown += OnTriangleMouseLeftButtonDown;

                _PART_HTriangle.Fill = new SolidColorBrush(HSLToRGB(HSLA.H, 1, 0.5, 1));

                _PART_HTriangle.RenderTransform = _triangleRotation;
                _PART_HTriangleOverlay.RenderTransform = _triangleRotation;
            }
        }

        private void SetTrianglePoints()
        {
            Point top = new(0, -_innerRadius);

            var rotatedX = Math.Cos(120 * _degreeToRadian) * top.X - Math.Sin(120 * _degreeToRadian) * top.Y;
            var rotatedY = Math.Sin(120 * _degreeToRadian) * top.X + Math.Cos(120 * _degreeToRadian) * top.Y;

            Point right = new(rotatedX, rotatedY);
            Point left = new(-rotatedX, rotatedY);

            _triangleSide = (right - left).Length;
            _triangleHeight = _triangleSide * Math.Sqrt(3) / 2;
            _triangle = new Point[] { left, top, right };
        }


        #endregion Initialisation


        #region Mouse Events

        private void OnCircleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(_PART_HueCircle);
            if (IsPointInHueCircle(p))
            {
                _isHCursorCaptured = true;
                SetHueFromPositionToCircle(p);
            }
        }

        private void OnTriangleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSLCursorCaptured = true;
            Point p = e.GetPosition(_PART_HTriangle);
            SetSLfromPositionToTriangle(p);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isHCursorCaptured = false;
            _isSLCursorCaptured = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isSLCursorCaptured)
            {
                Point p = e.GetPosition(_PART_HTriangle);
                SetSLfromPositionToTriangle(p);
                return;
            }

            if (_isHCursorCaptured)
            {
                Point p = e.GetPosition(_PART_HueCircle);
                SetHueFromPositionToCircle(p);
            }
        }

        #endregion Mouse Events


        private bool IsPointInHueCircle(Point point)
        {
            double distanceSquared = (_center - point).LengthSquared;
            return distanceSquared < _halfSizeSquared && distanceSquared > _innerRadiusSquared;
        }

        private void SetSLfromPositionToTriangle(Point p)
        {
            double distanceX = p.X - _triangle[0].X;
            double distanceY = _triangle[0].Y - p.Y;
            double saturation;
            double lightness;

            if (!_PART_HTriangle!.IsMouseOver)
            {
                if (distanceY < 0)
                {
                    saturation = distanceX <= 0 || distanceX >= _triangleSide ? 1 : 0;
                    lightness = distanceX / _triangleSide;
                }
                else
                {
                    saturation = 1;

                    double distanceYOverTriangleHeightHalf = distanceY / _triangleHeight / 2;
                    double capped = Math.Min(distanceYOverTriangleHeightHalf, 0.5);
                    if (distanceX <= _triangleSide / 2)
                        lightness = capped;
                    else
                        lightness = Math.Min(1 - capped, 1);
                }
            }
            else
            {
                lightness = distanceX / _triangleSide;
                double maxSaturationFromLightness = _triangleHeight * (-Math.Abs(lightness * 2 - 1) + 1);
                saturation = distanceY / maxSaturationFromLightness;
            }

            //Lightness = Math.Max(0, Math.Min(1, lightness));
            //Saturation = Math.Max(0, Math.Min(1, saturation));
            HSLA = new(HSLA.H, saturation, lightness, HSLA.A);

            SetSLCursorPosition();
        }

        private void SetHueFromPositionToCircle(Point p)
        {
            _hueRadians = Math.Atan2(p.Y - _halfSize, p.X - _halfSize) + Math.PI / 2;
            if (_hueRadians < 0)
                _hueRadians += 2 * Math.PI;
            //Hue = _hueRadians * _radianToDegree;
            HSLA = new(_hueRadians * _radianToDegree, HSLA.S, HSLA.L, HSLA.A);
        }

        private void SetSLCursorPosition()
        {
            if (_PART_SLCursor is not null)
            {
                double x = _triangle[0].X;
                double y = _triangle[0].Y;

                //x += _triangleSide * Lightness;
                //y -= _triangleHeight * (-Math.Abs(Lightness * 2 - 1) + 1) * Saturation;
                x += _triangleSide * HSLA.L;
                y -= _triangleHeight * (-Math.Abs(HSLA.L * 2 - 1) + 1) * HSLA.S;

                double rotatedX = Math.Cos(_hueRadians) * x - Math.Sin(_hueRadians) * y;
                double rotatedY = Math.Sin(_hueRadians) * x + Math.Cos(_hueRadians) * y;


                Canvas.SetLeft(_PART_SLCursor, rotatedX + _halfSize - _cursorHalfSize);
                Canvas.SetTop(_PART_SLCursor, rotatedY + _halfSize - _cursorHalfSize);
            }
        }

        private void SetHCursorPosition()
        {
            double x = _halfSize + _middleRadius * Math.Sin(_hueRadians) - _cursorHalfSize;
            double y = _halfSize - _middleRadius * Math.Cos(_hueRadians) - _cursorHalfSize;
            Canvas.SetLeft(_PART_HCursor, x);
            Canvas.SetTop(_PART_HCursor, y);
        }

        private static Color HSLToRGB(double hue, double saturation, double lightness, double alpha)
        {
            Color color;
            byte _alpha = (byte)(alpha * 255);

            if (saturation == 0)
            {
                byte l = (byte)(lightness * 255);
                color = Color.FromArgb(_alpha, l, l, l);
            }
                
            else
            {
                double min, max;
                double h = hue / 360;
                max = lightness < 0.5 ? lightness * (1 + saturation) : (lightness + saturation) - (lightness * saturation);
                min = (lightness * 2d) - max;

                color = Color.FromArgb(
                    _alpha, 
                    (byte)(255 * RGBChannelFromHue(min, max, h + 1 / 3d)),
                    (byte)(255 * RGBChannelFromHue(min, max, h)),
                    (byte)(255 * RGBChannelFromHue(min, max, h - 1 / 3d))
                );
            }

            return color;
        }

        private static double RGBChannelFromHue(double m1, double m2, double h)
        {
            h = (h + 1d) % 1d;
            if (h < 0) h += 1;
            if (h * 6 < 1) return m1 + (m2 - m1) * 6 * h;
            else if (h * 2 < 1) return m2;
            else if (h * 3 < 2) return m1 + (m2 - m1) * 6 * (2d / 3d - h);
            else return m1;
        }
    }
}
