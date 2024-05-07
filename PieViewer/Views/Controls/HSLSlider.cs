using PieViewer.Core;
using PieViewer.Core.Color;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PieViewer.Views.Controls;

public sealed class HSLSlider : ColorSlider
{
    public HSLSliderType HSLSliderType
    {
        get { return (HSLSliderType)GetValue(HSLSliderTypeProperty); }
        set { SetValue(HSLSliderTypeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HSLSliderType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HSLSliderTypeProperty =
        DependencyProperty.Register(nameof(HSLSliderType), typeof(HSLSliderType), typeof(HSLSlider), 
            new PropertyMetadata(HSLSliderType.Hue, OnHSLSliderTypeChanged));

    private static void OnHSLSliderTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HSLSlider control = (HSLSlider)d;
        control.SetRange();
        control.SetValue();
        if (control.RefreshGradient)
            control.GenerateBackground();
    }



    public HSLAColor HSLA
    {
        get { return (HSLAColor)GetValue(HSLAProperty); }
        set { SetValue(HSLAProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HSLA.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HSLAProperty =
        DependencyProperty.Register(nameof(HSLA), typeof(HSLAColor), typeof(HSLSlider), 
            new PropertyMetadata(default(HSLAColor), ONHSLAChanged));

    private static void ONHSLAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HSLSlider control = (HSLSlider)d;
        control.SetValue();
        if (control.RefreshGradient)
            control.GenerateBackground();
    }


    public HSLSlider()
    {
        SetRange();
        SetValue();
    }


    protected override bool RefreshGradient => HSLSliderType != HSLSliderType.Hue;

    protected override void GenerateBackground()
    {
        _backgroundBrush.GradientStops = HSLSliderType switch
        {
            HSLSliderType.Hue => new GradientStopCollection()
            {
                new GradientStop(GetColorForSelectedArgb(0), 0),
                new GradientStop(GetColorForSelectedArgb(60), 1/6.0),
                new GradientStop(GetColorForSelectedArgb(120), 2/6.0),
                new GradientStop(GetColorForSelectedArgb(180), 0.5),
                new GradientStop(GetColorForSelectedArgb(240), 4/6.0),
                new GradientStop(GetColorForSelectedArgb(300), 5/6.0),
                new GradientStop(GetColorForSelectedArgb(360), 1)
            },
            HSLSliderType.Saturation => new GradientStopCollection
            {
                new GradientStop(GetColorForSelectedArgb(0), 0.0),
                new GradientStop(GetColorForSelectedArgb(255), 1)
            },
            HSLSliderType.Lightness => new GradientStopCollection()
            {
                new GradientStop(GetColorForSelectedArgb(0), 0),
                new GradientStop(GetColorForSelectedArgb(128), 0.5),
                new GradientStop(GetColorForSelectedArgb(255), 1)
            },
        };

        LeftCapColor.Color = _backgroundBrush.GradientStops.First().Color;
        RightCapColor.Color = _backgroundBrush.GradientStops.Last().Color;
    }

    private Color GetColorForSelectedArgb(int value)
    {
        return HSLSliderType switch
        {
            HSLSliderType.Hue => ColorSpaceConversion.HslaToRgba(value, 1.0, 0.5, HSLA.A),
            HSLSliderType.Saturation => ColorSpaceConversion.HslaToRgba(HSLA.H, value / 255.0, HSLA.L, HSLA.A),
            HSLSliderType.Lightness => ColorSpaceConversion.HslaToRgba(HSLA.H, HSLA.S, value / 255.0, HSLA.A),
        };
    }

    private void SetRange()
    {
        switch (HSLSliderType)
        {
            case HSLSliderType.Hue:
                Minimum = 0;
                Maximum = 360;
                SmallChange = 1;
                LargeChange = 10;
                break;
            case HSLSliderType.Saturation:
            case HSLSliderType.Lightness:
                Minimum = 0;
                Maximum = 1;
                SmallChange = 0.05;
                LargeChange = 0.2;
                break;
        }
    }


    protected override void OnValueChanged(double oldValue, double newValue)
    {
        HSLA = HSLSliderType switch
        {
            HSLSliderType.Hue => new(newValue, HSLA.S, HSLA.L, HSLA.A),
            HSLSliderType.Saturation => new(HSLA.H, newValue, HSLA.L, HSLA.A),
            HSLSliderType.Lightness => new(HSLA.H, HSLA.S, newValue, HSLA.A),
        };
    }

    private void SetValue()
    {
        Value = HSLSliderType switch
        {
            HSLSliderType.Hue => HSLA.H,
            HSLSliderType.Saturation => HSLA.S,
            HSLSliderType.Lightness => HSLA.L,
        };
    }
}
