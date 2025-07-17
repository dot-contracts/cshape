using System;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;
using Windows.UI;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public sealed partial class NxNumPad : NxControlBase
    {
        public static readonly DependencyProperty ValueProperty         = DependencyProperty.Register(nameof(Value),         typeof(string), typeof(NxNumPad), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MaxLengthProperty     = DependencyProperty.Register(nameof(MaxLength),     typeof(int),    typeof(NxNumPad), new PropertyMetadata(0));
        public static readonly DependencyProperty AutoClearProperty     = DependencyProperty.Register(nameof(AutoClear),     typeof(bool),   typeof(NxNumPad), new PropertyMetadata(true));
        public static readonly DependencyProperty AllowDecimalProperty  = DependencyProperty.Register(nameof(AllowDecimal),  typeof(bool),   typeof(NxNumPad), new PropertyMetadata(false));
        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.Register(nameof(AllowNegative), typeof(bool),   typeof(NxNumPad), new PropertyMetadata(false));
        public static readonly DependencyProperty FormatHintProperty    = DependencyProperty.Register(nameof(FormatHint),    typeof(string), typeof(NxNumPad), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ButtonHeightProperty  = DependencyProperty.Register(nameof(ButtonHeight),  typeof(double), typeof(NxNumPad), new PropertyMetadata(40.0));
        public static readonly DependencyProperty ButtonWidthProperty   = DependencyProperty.Register(nameof(ButtonWidth),   typeof(double), typeof(NxNumPad), new PropertyMetadata(40.0));
        public static readonly DependencyProperty ShowDisplayProperty   = DependencyProperty.Register(nameof(ShowDisplay),   typeof(bool),   typeof(NxNumPad), new PropertyMetadata(true));
        public static readonly DependencyProperty ShowOKButtonProperty  = DependencyProperty.Register(nameof(ShowOKButton),  typeof(bool),   typeof(NxNumPad), new PropertyMetadata(true));

        public string Value         { get => (string)GetValue(ValueProperty);         set => SetValue(ValueProperty, value); }
        public int    MaxLength     { get => (int)GetValue(MaxLengthProperty);        set => SetValue(MaxLengthProperty, value); }
        public bool   AutoClear     { get => (bool)GetValue(AutoClearProperty);       set => SetValue(AutoClearProperty, value); }
        public bool   AllowDecimal  { get => (bool)GetValue(AllowDecimalProperty);    set => SetValue(AllowDecimalProperty, value); }
        public bool   AllowNegative { get => (bool)GetValue(AllowNegativeProperty);   set => SetValue(AllowNegativeProperty, value); }
        public string FormatHint    { get => (string)GetValue(FormatHintProperty);    set => SetValue(FormatHintProperty, value); }
        public double ButtonHeight  { get => (double)GetValue(ButtonHeightProperty);  set => SetValue(ButtonHeightProperty, value); }
        public double ButtonWidth   { get => (double)GetValue(ButtonWidthProperty);   set => SetValue(ButtonWidthProperty, value); }
        public bool   ShowDisplay   { get => (bool)GetValue(ShowDisplayProperty);     set => SetValue(ShowDisplayProperty, value); }
        public bool   ShowOKButton  { get => (bool)GetValue(ShowOKButtonProperty);    set => SetValue(ShowOKButtonProperty, value); }

        public NxNumPad()
        {
            this.DefaultStyleKey = typeof(NxNumPad);

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("lblDisplay") is NxLabel display)
                display.Visibility = ShowDisplay ? Visibility.Visible : Visibility.Collapsed;

            NxButton but0 = (NxButton)GetTemplateChild("btn0"); but0.OnClicked += (s, e) => AppendDigit("0");
            NxButton but1 = (NxButton)GetTemplateChild("btn1"); but1.OnClicked += (s, e) => AppendDigit("1");
            NxButton but2 = (NxButton)GetTemplateChild("btn2"); but2.OnClicked += (s, e) => AppendDigit("2");
            NxButton but3 = (NxButton)GetTemplateChild("btn3"); but3.OnClicked += (s, e) => AppendDigit("3");
            NxButton but4 = (NxButton)GetTemplateChild("btn4"); but4.OnClicked += (s, e) => AppendDigit("4");
            NxButton but5 = (NxButton)GetTemplateChild("btn5"); but5.OnClicked += (s, e) => AppendDigit("5");
            NxButton but6 = (NxButton)GetTemplateChild("btn6"); but6.OnClicked += (s, e) => AppendDigit("6");
            NxButton but7 = (NxButton)GetTemplateChild("btn7"); but7.OnClicked += (s, e) => AppendDigit("7");
            NxButton but8 = (NxButton)GetTemplateChild("btn8"); but8.OnClicked += (s, e) => AppendDigit("8");
            NxButton but9 = (NxButton)GetTemplateChild("btn9"); but9.OnClicked += (s, e) => AppendDigit("9");

            NxButton dotBtn = (NxButton)GetTemplateChild("btnDot");
            if (dotBtn != null)
            {
                if (AllowDecimal) dotBtn.OnClicked += (s, e) => AppendSymbol(".");
                else              dotBtn.Visibility = Visibility.Collapsed;
            }

            AttachHandler("btnClear", () => Value = "");
            AttachHandler("btnBack", () =>
            {
                if (!string.IsNullOrEmpty(Value))
                    Value = Value.Substring(0, Value.Length - 1);
            });

            NxButton okBtn = (NxButton)GetTemplateChild("btnOK");
            if (okBtn != null)
            {
                if (!ShowOKButton) okBtn.Visibility = Visibility.Collapsed;
                else
                {
                    okBtn.Height = ButtonHeight;
                    okBtn.Width = ButtonWidth * 3; // Default width for OK button

                    okBtn.OnClicked += (s, e) =>
                    {
                        RaiseOnChanged(Value);
                        if (AutoClear)
                            Value = string.Empty;
                    };

                    // OK button width logic
                    if (AllowDecimal && AllowNegative)        okBtn.Width = ButtonWidth;
                    else if (AllowDecimal || AllowNegative)   okBtn.Width = ButtonWidth * 2;
                    else                                      okBtn.Width = ButtonWidth * 3;
                }
            }

            if (GetTemplateChild("btnNeg") is NxButton negBtn)
            {
                if (!AllowNegative) negBtn.Visibility = Visibility.Collapsed;
                else
                {
                    negBtn.OnClicked += (s, e) =>
                    {
                        Value = Value.StartsWith("-") ? Value.TrimStart('-') : "-" + Value;
                        RaiseOnChange(Value);
                        UpdateDisplay();
                    };
                }
            }

            UpdateDisplay();
            ApplyThemeDefaults();
        }

        private void ApplyThemeDefaults()
        {

        }

        private void AppendDigit(string digit)
        {
            if (MaxLength > 0 && Value.Length >= MaxLength)
            {
                ShowErrorState();
                return;
            }

            Value += digit;

            if (RaiseOnChange(Value))
            {
                if (MaxLength > 0 && Value.Length == MaxLength)
                {
                    RaiseOnChanged(Value);
                    if (AutoClear)
                        Value = string.Empty;
                }
                else RaiseOnChanged(Value);
            }
        }

        private void AppendSymbol(string symbol)
        {
            if (symbol == "." && Value.Contains(".")) return;

            Value += symbol;

            UpdateDisplay();
        }

        private void AttachHandler(string name, Action action)
        {
            if (GetTemplateChild(name) is NxButton btn)
                btn.OnClicked += (s, e) => { action(); UpdateDisplay(); };
        }

        private void UpdateDisplay()
        {
            if (RaiseOnChange(Value))
                RaiseOnChanged(Value);
        }

        private async void ShowErrorState()
        {
            if (GetTemplateChild("lblDisplay") is NxLabel textBox)
            {
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
                textBox.Background = new SolidColorBrush(Colors.MistyRose);

                await ShakeDisplay(textBox);
                await Task.Delay(400);

                textBox.BorderBrush = (Brush)Application.Current.Resources["NxBorderBrush"];
                textBox.Background = null;
            }
        }

        private async Task ShakeDisplay(Control control)
        {
            var transform = new TranslateTransform();
            control.RenderTransform = transform;

            for (int i = 0; i < 4; i++)
            {
                transform.X = (i % 2 == 0) ? -10 : 10;
                await Task.Delay(50);
            }

            transform.X = 0;
            control.RenderTransform = null;
        }
    }
}
