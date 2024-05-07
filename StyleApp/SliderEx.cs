using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StyleApp
{
    internal class SliderEx : Slider
    {


        //public bool ClipTrackBackgroundOn
        //{
        //    get { return (bool)GetValue(ClipTrackBackgroundOnProperty); }
        //    set { SetValue(ClipTrackBackgroundOnProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ClipTrackBackgroundOn.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ClipTrackBackgroundOnProperty =
        //    DependencyProperty.Register("ClipTrackBackgroundOn", typeof(bool), typeof(ownerclass), new PropertyMetadata(0));





        public Brush TrackBackground
        {
            get { return (Brush)GetValue(TrackBackgroundProperty); }
            set { SetValue(TrackBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBackgroundProperty =
            DependencyProperty.Register(nameof(TrackBackground), typeof(Brush), typeof(SliderEx), new PropertyMetadata(null));




        public Brush TrackBorderBrush
        {
            get { return (Brush)GetValue(TrackBorderBrushProperty); }
            set { SetValue(TrackBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBorderBrushProperty =
            DependencyProperty.Register(nameof(TrackBorderBrush), typeof(Brush), typeof(SliderEx), new PropertyMetadata(null));



    }
}
