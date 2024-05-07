using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PieViewer.Views.Controls;

public enum CurrentColor
{
    Primary,
    Secondary,
}

internal class ColorSelector : Control
{
    public Color? PrimaryColor
    {
        get { return (Color?)GetValue(PrimaryColorProperty); }
        set { SetValue(PrimaryColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PrimaryColorProperty =
        DependencyProperty.Register(nameof(PrimaryColor), typeof(Color?), typeof(ColorSelector), 
            new PropertyMetadata(default(Color?)));


    public Color? SecondaryColor
    {
        get { return (Color?)GetValue(SecondaryColorProperty); }
        set { SetValue(SecondaryColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SecondaryColorProperty =
        DependencyProperty.Register(nameof(SecondaryColor), typeof(Color?), typeof(ColorSelector),
            new PropertyMetadata(default(Color?)));




    void Test()
    {
    }
}
