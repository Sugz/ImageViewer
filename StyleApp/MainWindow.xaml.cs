using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StyleApp
{

    public struct HSLColor
    {
        public double Hue;
        public double Saturation;
        public double Lightness;

        public HSLColor()
        {
            
        }

        public HSLColor(double hue, double saturation, double lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }
    }



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //LinearGradientBrush _gradient;
        //GradientStop _lightnessGradientMidColor;


        public MainWindow()
        {
            InitializeComponent();
            

            //_lightnessGradientMidColor = new(Color, 0.5);
            //GradientStopCollection gradientStops = new()
            //{
            //    new GradientStop(HSLToRGB(0, 1, 0, 1), 0),
            //    new GradientStop(HSLToRGB(0, 1, 0.5, 1), 0.5),
            //    new GradientStop(HSLToRGB(0, 1, 1, 1), 1)
            //};
            //_gradient = new(gradientStops, new Point(0, 0.5), new Point(1, 0.5));
            //LightnessSlider.TrackBackground = _gradient;

            //Color = Color.FromArgb(255, 255, 0, 0);

            //RedSlider.ValueChanged += RedSlider_ValueChanged;
        }

        /*
        private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color = Color.FromArgb(255, (byte)RedSlider.Value, 0, 0);
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(MainWindow), new FrameworkPropertyMetadata(default(Color), OnColorChanged));

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindow window = (MainWindow)d;
            //window._gradient.GradientStops[0].Color = HSLToRGB(FromRGB(window.Color.R, window.Color.G, window.Color.B), 1, 0, 1);
            //window._gradient.GradientStops[1].Color = window.Color;
            //window._gradient.GradientStops[2].Color = HSLToRGB(FromRGB(window.Color.R, window.Color.G, window.Color.B), 1, 1, 1);
            //window.LightnessSlider.TrackBackground = window._gradient;

            HSLColor hsl = FromRGB(window.Color);
            window._gradient.GradientStops[0].Color = HSLToRGB(hsl.Hue, hsl.Saturation, 0, 1);
            window._gradient.GradientStops[1].Color = HSLToRGB(hsl.Hue, hsl.Saturation, 0.5, 1);
            window._gradient.GradientStops[2].Color = HSLToRGB(hsl.Hue, hsl.Saturation, 1, 1);
            window.LightnessSlider.TrackBackground = window._gradient;

            window.LightnessSlider.Value = hsl.Lightness;
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



        public static HSLColor FromRGB(Color color)
        {
            double _R = (color.R / 255f);
            double _G = (color.G / 255f);
            double _B = (color.B / 255f);

            double _Min = Math.Min(Math.Min(_R, _G), _B);
            double _Max = Math.Max(Math.Max(_R, _G), _B);
            double _Delta = _Max - _Min;

            double H = 0;
            double S = 0;
            double L = (double)((_Max + _Min) / 2.0f);

            if (_Delta != 0)
            {
                if (L < 0.5f)
                {
                    S = (double)(_Delta / (_Max + _Min));
                }
                else
                {
                    S = (double)(_Delta / (2.0f - _Max - _Min));
                }


                if (_R == _Max)
                {
                    H = (_G - _B) / _Delta;
                }
                else if (_G == _Max)
                {
                    H = 2f + (_B - _R) / _Delta;
                }
                else if (_B == _Max)
                {
                    H = 4f + (_R - _G) / _Delta;
                }
            }

            return new HSLColor(H, S, L);
        }

        */
    }




    


}
