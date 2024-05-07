using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PieViewer.Views.Controls
{
    /// <summary>
    /// A Slider which enable IsMoveToPointEnabled and start to drag the thumb
    /// </summary>
    public class MoveToPointAndDragSlider : Slider
    {
        private Thumb? _thumb;

        public MoveToPointAndDragSlider()
        {
            IsMoveToPointEnabled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_thumb is not null)
                _thumb.MouseEnter -= OnThumbMouseEnter;

            if (GetTemplateChild("Thumb") is Thumb thumb)
            {
                _thumb = thumb;
                _thumb.MouseEnter += OnThumbMouseEnter;
            }
        }

        private void OnThumbMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _thumb is not null)
            {
                // the left button is pressed on mouse enter
                // so the thumb must have been moved under the mouse
                // in response to a click on the track.
                // Generate a MouseLeftButtonDown event.
                MouseButtonEventArgs args = new(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                _thumb.RaiseEvent(args);
            }
        }
    }
}

