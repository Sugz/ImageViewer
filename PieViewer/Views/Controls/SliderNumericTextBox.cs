using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PieViewer.Views.Controls
{
    internal class SliderNumericTextBox : HeaderedContentControl
    {

        private Slider? _PART_Slider;
        private TextBox? _PART_TextBox;
        private ToggleButton? _PART_Toggle;



        public string? StringFormat { get; set; } = "{0:p0}";


        #region Minimum

        /// <summary>
        /// 
        /// </summary>
        [Description(""), Category("")]
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        // DependencyProperty for Minimum
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(SliderNumericTextBox),
            new FrameworkPropertyMetadata(0d, OnMinimumChanged, OnCoerceMinimum)
        );

        private static object OnCoerceMinimum(DependencyObject d, object baseValue)
        {
            SliderNumericTextBox control = (SliderNumericTextBox)d;
            double value = (double)baseValue;
            if(value >= control.Maximum)
                return control.Maximum;
            return value;
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ZoomFactorChangedEventArgs args = new ZoomFactorChangedEventArgs((double)e.OldValue, (double)e.NewValue);
            //((ZoomAndPan)d).OnZoomFactorChanged(args);
        }

        #endregion Minimum 

        #region Maximum

        /// <summary>
        /// 
        /// </summary>
        [Description(""), Category("")]
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        // DependencyProperty for Maximum
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(SliderNumericTextBox),
            new FrameworkPropertyMetadata(1d, OnMaximumChanged, OnCoerceMaximum)
        );

        private static object OnCoerceMaximum(DependencyObject d, object baseValue)
        {
            SliderNumericTextBox control = (SliderNumericTextBox)d;
            double value = (double)baseValue;
            if (value <= control.Minimum)
                return control.Minimum;
            return value;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ZoomFactorChangedEventArgs args = new ZoomFactorChangedEventArgs((double)e.OldValue, (double)e.NewValue);
            //((ZoomAndPan)d).OnZoomFactorChanged(args);
        }

        #endregion Maximum 

        #region Value

        /// <summary>
        /// 
        /// </summary>
        [Description(""), Category("")]
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // DependencyProperty for Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(SliderNumericTextBox),
            new FrameworkPropertyMetadata(1d, OnValueChanged, OnCoerceValue)
        );

        private static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            SliderNumericTextBox control = (SliderNumericTextBox)d;
            double value = (double)baseValue;
            if (value <= control.Minimum)
                return control.Minimum;
            if (value >= control.Maximum)
                return control.Maximum;
            return value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SliderNumericTextBox control = (SliderNumericTextBox)d;
            double value = (double)e.NewValue;

            if (control._PART_TextBox is not null)
                //control._PART_TextBox.Text = $"{value} %";
                control.SetTextBoxText();
        }

        #endregion Value 

        #region IsDropDownOpen

        /// <summary>
        /// 
        /// </summary>
        [Description(""), Category("")]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        // DependencyProperty for IsDropDownOpen
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen", 
            typeof(bool), 
            typeof(SliderNumericTextBox), 
            new FrameworkPropertyMetadata(false));

        #endregion IsDropDownOpen 


        #region Constructor

        static SliderNumericTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderNumericTextBox), new FrameworkPropertyMetadata(typeof(SliderNumericTextBox)));
        }

        #endregion Constructor


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_Slider") is Slider slider)
            {
                _PART_Slider = slider;
            }

            if (GetTemplateChild("PART_TextBox") is TextBox textbox)
            {
                _PART_TextBox = textbox;
                
                _PART_TextBox.LostFocus += OnTextBoxLostFocus;
                _PART_TextBox.KeyUp += OnTextBoxKeyUp;
                _PART_TextBox.GotKeyboardFocus += OnTextBoxGotKeyboardFocus;
                //_PART_TextBox.LostMouseCapture += OnTextBoxLostMouseCapture;
                //_PART_TextBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;

                SetTextBoxText();
            }

            if (GetTemplateChild("PART_Toggle") is ToggleButton toggle)
            {
                _PART_Toggle = toggle;
            }
        }

        private void OnTextBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ValidateTextBoxText();
                Focus();
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ValidateTextBoxText();
        }


        private void ValidateTextBoxText()
        {
            if (_PART_TextBox is not null)
            {
                try
                {
                    string trimText = Regex.Replace(_PART_TextBox.Text, @"[^\d.,]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                    trimText = Regex.Replace(trimText, @",", ".", RegexOptions.None, TimeSpan.FromSeconds(1.5));

                    if (trimText == "")
                    {
                        //_PART_TextBox.Text = $"{Value} %";
                        SetTextBoxText();
                        return;
                    }

                    if (double.TryParse(trimText, out double value))
                        Value = value;
                    else
                        //_PART_TextBox.Text = $"{Value} %";
                        SetTextBoxText();
                }
                // If we timeout when replacing invalid characters,
                // we should return Empty.
                catch (RegexMatchTimeoutException)
                {
                    //_PART_TextBox.Text = $"{Value} %";
                    SetTextBoxText();
                }
            }
        }



        private void SetTextBoxText()
        {
            if (StringFormat is not null)
                _PART_TextBox.Text = string.Format(StringFormat, Value);
            else
                _PART_TextBox.Text = Value.ToString();
        }


        private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Fixes issue when clicking cut/copy/paste in context menu
            if (_PART_TextBox!.SelectionLength == 0)
                _PART_TextBox.SelectAll();
        }

        private void OnTextBoxLostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (_PART_TextBox!.SelectionLength == 0)
                _PART_TextBox.SelectAll();

            // further clicks will not select all
            _PART_TextBox.LostMouseCapture -= OnTextBoxLostMouseCapture;
        }

        private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // once we've left the TextBox, return the select all behavior
            _PART_TextBox!.LostMouseCapture += OnTextBoxLostMouseCapture;
        }
    }
}
