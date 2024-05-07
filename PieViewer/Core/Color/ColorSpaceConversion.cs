using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PieViewer.Core.Color;

public static class ColorSpaceConversion
{
    public static (double, double, double) RgbToHsl(double r, double g, double b)
    {
        double h, s, l;

        double min = Math.Min(Math.Min(r, g), b);
        double max = Math.Max(Math.Max(r, g), b);
        double delta = max - min;
        l = (max + min) / 2;

        if (max == 0)
        {
            //pure black
            return (-1, -1, 0);
        }

        if (delta == 0)
        {
            //gray
            return (-1, 0, l);
        }

        //magic
        s = l <= 0.5 ? delta / (max + min) : delta / (2 - max - min);

        if (r == max)
            h = (g - b) / 6 / delta;
        else if (g == max)
            h = 1.0f / 3 + (b - r) / 6 / delta;
        else
            h = 2.0f / 3 + (r - g) / 6 / delta;

        if (h < 0)
            h += 1;
        if (h > 1)
            h -= 1;

        h *= 360;

        return (h, s, l);
    }


    /// <summary>
    /// Converts HSL to RGB
    /// </summary>
    /// <param name="h">Hue, 0-360</param>
    /// <param name="s">Saturation, 0-1</param>
    /// <param name="l">Lightness, 0-1</param>
    /// <returns>Values (0-1) in order: R, G, B</returns>
    public static System.Windows.Media.Color HslaToRgba(double h, double s, double l, double a)
    {
        int hueCircleSegment = (int)(h / 60);
        double circleSegmentFraction = (h - (60 * hueCircleSegment)) / 60;

        double maxRGB = l < 0.5 ? l * (1 + s) : l + s - l * s;
        double minRGB = 2 * l - maxRGB;
        double delta = maxRGB - minRGB;

        return hueCircleSegment switch
        {
            0 => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)(maxRGB * 255), (byte)((delta * circleSegmentFraction + minRGB) * 255), (byte)(minRGB * 255)),
            1 => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)((delta * (1 - circleSegmentFraction) + minRGB) * 255), (byte)(maxRGB * 255), (byte)(minRGB * 255)),
            2 => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)(minRGB * 255), (byte)(maxRGB * 255), (byte)((delta * circleSegmentFraction + minRGB) * 255)),
            3 => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)(minRGB * 255), (byte)((delta * (1 - circleSegmentFraction) + minRGB) * 255), (byte)(maxRGB * 255)),
            4 => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)((delta * circleSegmentFraction + minRGB) * 255), (byte)(minRGB * 255), (byte)(maxRGB * 255)),
            _ => System.Windows.Media.Color.FromArgb((byte)(a * 255), (byte)(maxRGB * 255), (byte)(minRGB * 255), (byte)((delta * (1 - circleSegmentFraction) + minRGB) * 255))
        };
    }


    public static System.Windows.Media.Color HslaToRgba(HSLAColor hsla)
    {
        return HslaToRgba(hsla.H, hsla.S, hsla.L, hsla.A);
    }
}
