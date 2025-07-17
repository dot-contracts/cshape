using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using nexus.common.control.Themes;
using System;
using Windows.System;
using static nexus.common.control.NxImage;

namespace nexus.common.control
{
    public partial class NxTime : NxControlBase
    {
        private static bool isChanged = true;

        private bool isReadonly = false; public bool IsReadOnly { get => isReadonly; set => isReadonly = value; }
        private bool mShowButtons = true;
        private bool mIsEnabled = true;
        private DateTime mCurrTime = DateTime.Now;
        private string mCurTime = "";

        private int mnHour;
        private int mnMinute;
        private int mnMin10;
        private int mnMin1;
        private string mnAMPM = "";

        private string mValue = "";
        static string mOriginalValue = string.Empty;


        private List<NxButton> _hourButtons = new();
        private List<NxButton> _min1Buttons = new();
        private List<NxButton> _min10Buttons = new();
        private List<NxButton> _ampmButtons = new();

        private NxLabel _label;
        private StackPanel _buttonsPanel;

        public static readonly DependencyProperty ShowButtonsProperty = DependencyProperty.Register(nameof(ShowButtons), typeof(bool), typeof(NxTime), new PropertyMetadata(false, OnVisualPropertyChanged));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(NxTime), new PropertyMetadata(null, OnValueChanged));

        public bool ShowButtons { get => (bool)GetValue(ShowButtonsProperty); set => SetValue(ShowButtonsProperty, value); }


        public string Value
        {
            get => mValue;
            set
            {
                mOriginalValue = value;
                BreakTime(value);
            }
        }

        public static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NxTime control && e.NewValue is string newValue)
            {
                isChanged = (newValue != mOriginalValue);
                mOriginalValue = newValue;
            }
        }


        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NxTime)d).SetVisualState();
        }

        private void SetVisualState()
        {
            if (_buttonsPanel != null)
                _buttonsPanel.Visibility = ShowButtons ? Visibility.Visible : Visibility.Collapsed;
        }


        public delegate void OnCanceledEventHandler();
        public event OnCanceledEventHandler OnCanceled;

        public NxTime()
        {
            this.DefaultStyleKey = typeof(NxTime);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _label = GetTemplateChild("Result") as NxLabel;
            _buttonsPanel = GetTemplateChild("ButtonPanel") as StackPanel;

            for (int i = 0; i <= 12; i++) HookButton("hb" + i);
            for (int i = 0; i <= 9; i++) HookButton("mm" + i);
            for (int i = 0; i <= 50; i += 10) HookButton("mg" + i.ToString("00"));

            HookButton("btAM");
            HookButton("btPM");
            HookButton("btOK");
            HookButton("btCancel");

            BreakTime(string.IsNullOrEmpty(mOriginalValue) ? DateTime.Now.ToString("hh:mm t") : mOriginalValue);

            this.SizeChanged += NxTime_SizeChanged;
        }

        private void HookButton(string name)
        {
            if (GetTemplateChild(name) is NxButton button)
            {
                button.OnClicked += ButtonClicked;
                if (name.StartsWith("hb")) _hourButtons.Add(button);
                else if (name.StartsWith("mm")) _min1Buttons.Add(button);
                else if (name.StartsWith("mg")) _min10Buttons.Add(button);
                else if (name == "btAM" || name == "btPM") _ampmButtons.Add(button);

            }
        }

        public new bool IsEnabled
        {
            get => mIsEnabled;
            set
            {
                if (mIsEnabled != value)
                {
                    mIsEnabled = value;
                    BreakTime(mCurTime);
                }
            }
        }

        private void RaiseOnChanged()
        {
            if (DateTime.TryParse(mCurTime, out DateTime dt))
            {
                mValue = dt.ToString("HH:mm");
                if (RaiseOnChange(mValue))
                    RaiseOnChanged(mValue);
            }
        }


        private void btOK_Click(string Tag, string Prompt, Windows.Foundation.Rect ScreenRect)
        {
            RaiseOnChanged();
        }

        private void btCancel_Click(string Tag, string Prompt, Windows.Foundation.Rect ScreenRect)
        {
            OnCanceled?.Invoke();
        }

        private void ButtonClicked(object? sender, ClickedEventArgs e)
        {
            NxButton button = (NxButton)sender;
            switch (button.Name)
            {
                case "btOK": RaiseOnChanged(); break;
                case "btCancel": OnCanceled?.Invoke(); break;
                default: TimeButton_Click(button.Name); break;
            }
        }

        private void TimeButton_Click(string Name)
        {
            string buton = Name.Substring(0, 2);
            string value = Name.Substring(2, Name.Length - 2);


            if (value == "AM" || value == "PM")
            {
                mnAMPM = value;
                MarkSelectedBorder(_ampmButtons, Name);
            }
            else if (buton == "hb")
            {
                mnHour = helpers.ToInt(value);
                MarkSelectedBorder(_hourButtons, Name);
            }
            else if (buton == "mg")
            {
                mnMin10 = helpers.ToInt(value) / 10;
                MarkSelectedBorder(_min10Buttons, Name);
            }
            else if (buton == "mm")
            {
                mnMin1 = helpers.ToInt(value);
                MarkSelectedBorder(_min1Buttons, Name);
            }

            mCurTime = mnHour.ToString() + ":" + (mnMin10 == 0 ? "0" : "") + Convert.ToString(mnMin10 * 10 + mnMin1) + " " + mnAMPM;

            mValue = mCurTime;

            BreakTime(mCurTime);

            //if (!mShowButtons)
            {
                RaiseOnChanged();
            }
        }


        private void BreakTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time) || !DateTime.TryParse(time, out DateTime parsed))
            {
                parsed = DateTime.Now;
            }

            mCurTime = parsed.ToString("hh:mm tt");
            mnHour = parsed.Hour;
            mnMinute = parsed.Minute;
            mnAMPM = mnHour >= 12 ? "PM" : "AM";

            if (mnHour > 12) mnHour -= 12;
            if (mnHour == 0) mnHour = 12;

            mnMin10 = mnMinute / 10;
            mnMin1 = mnMinute % 10;

            if (_label != null) _label.Prompt = mCurTime;

            // ✅ Update selection states based on parsed values
            MarkSelectedBorder(_ampmButtons,  "bt" + mnAMPM);
            MarkSelectedBorder(_hourButtons,  "hb" + mnHour);
            MarkSelectedBorder(_min10Buttons, "mg" + mnMin10.ToString("00"));
            MarkSelectedBorder(_min1Buttons,  "mm" + mnMin1);
        }

        public void SetFocus() { }

        public DateTime SetDate
        {
            set => BreakTime(value.ToString("HH:mm"));
        }

        private void MarkSelectedBorder(List<NxButton> group, string selectedName)
        {
            foreach (var btn in group)
                btn.IsSelected = btn.Name.Equals(selectedName);
        }

        private void NxTime_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double totalWidth = this.ActualWidth;

            // Get all the named grids
            var hourGrid = GetTemplateChild("HourGrid") as Grid;
            var min10Grid = GetTemplateChild("Min10Grid") as Grid;
            var min1Grid = GetTemplateChild("MinGrid") as Grid;
            var ampmGrid = GetTemplateChild("ampmGrid") as Grid;

            var grids = new[] { hourGrid, min10Grid, min1Grid, ampmGrid };

            // Determine max number of columns across all grids
            int maxCols = grids.Max(g => g?.ColumnDefinitions.Count ?? 0);
            if (maxCols == 0) return;

            // Calculate uniform column width
            double uniformColWidth = totalWidth / maxCols;

            foreach (var grid in grids)
            {
                if (grid == null) continue;

                for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                {
                    grid.ColumnDefinitions[i].Width = new GridLength(uniformColWidth, GridUnitType.Pixel);
                }
            }
        }
    }
}