using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PieViewer.Views.Controls
{
    public abstract class ColorSlider : MoveToPointAndDragSlider
    {

        protected readonly LinearGradientBrush _backgroundBrush = new();
        private SolidColorBrush _leftCapColor = new ();
        private SolidColorBrush _rightCapColor = new ();
        protected virtual bool RefreshGradient => true;

        
        public SolidColorBrush LeftCapColor
        {
            get => _leftCapColor;
            set => _leftCapColor = value;
        }

        
        public SolidColorBrush RightCapColor
        {
            get => _rightCapColor;
            set => _rightCapColor = value;
        }

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public override void EndInit()
        {
            base.EndInit();
            Background = _backgroundBrush;
            GenerateBackground();
        }

        protected abstract void GenerateBackground();
    }
}
