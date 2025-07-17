using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nexus.common.control
{
    public partial class NxCalendar : NxControlBase
    {
        private CalendarView? _calendar;
        private int isEnterKey = 0;

        #region Dependency Properties

        public static readonly DependencyProperty ValueProperty                      = DependencyProperty.Register( nameof(Value),                      typeof(DateTime),            typeof(NxCalendar), new PropertyMetadata(DateTime.MinValue, OnValueChanged));
        public static readonly DependencyProperty PromptProperty                     = DependencyProperty.Register( nameof(Prompt),                     typeof(string),              typeof(NxCalendar), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty PromptFontFamilyProperty           = DependencyProperty.Register( nameof(PromptFontFamily),           typeof(FontFamily),          typeof(NxCalendar), new PropertyMetadata(default(FontFamily)));
        public static readonly DependencyProperty PromptFontSizeProperty             = DependencyProperty.Register( nameof(PromptFontSize),             typeof(double),              typeof(NxCalendar), new PropertyMetadata(12.0));
        public static readonly DependencyProperty PromptMarginProperty               = DependencyProperty.Register( nameof(PromptMargin),               typeof(Thickness),           typeof(NxCalendar), new PropertyMetadata(default(Thickness)));
        public static readonly DependencyProperty MaxPromptWidthProperty             = DependencyProperty.Register( nameof(MaxPromptWidth),             typeof(double),              typeof(NxCalendar), new PropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty PromptHorizontalAlignmentProperty  = DependencyProperty.Register( nameof(PromptHorizontalAlignment),  typeof(HorizontalAlignment), typeof(NxCalendar), new PropertyMetadata(HorizontalAlignment.Left));
        public static readonly DependencyProperty PromptVerticalAlignmentProperty    = DependencyProperty.Register( nameof(PromptVerticalAlignment),    typeof(VerticalAlignment),   typeof(NxCalendar), new PropertyMetadata(VerticalAlignment.Center));

        public static readonly DependencyProperty MinDateProperty                    = DependencyProperty.Register( nameof(MinDate),                    typeof(DateTime),            typeof(NxCalendar), new PropertyMetadata(DateTime.MinValue));
        public static readonly DependencyProperty MaxDateProperty                    = DependencyProperty.Register( nameof(MaxDate),                    typeof(DateTime),            typeof(NxCalendar), new PropertyMetadata(DateTime.MaxValue));

        public DateTime            Value                      { get => (DateTime)           GetValue(ValueProperty);                      set => SetValue(ValueProperty, value);  }
        public string              Prompt                     { get => (string)             GetValue(PromptProperty);                     set => SetValue(PromptProperty, value); }
        public FontFamily          PromptFontFamily           { get => (FontFamily)         GetValue(PromptFontFamilyProperty);           set => SetValue(PromptFontFamilyProperty, value); }
        public double              PromptFontSize             { get => (double)             GetValue(PromptFontSizeProperty);             set => SetValue(PromptFontSizeProperty, value); }
        public Thickness           PromptMargin               { get => (Thickness)          GetValue(PromptMarginProperty);               set => SetValue(PromptMarginProperty, value); }
        public double              MaxPromptWidth             { get => (double)             GetValue(MaxPromptWidthProperty);             set => SetValue(MaxPromptWidthProperty, value); }
        public HorizontalAlignment PromptHorizontalAlignment  { get => (HorizontalAlignment)GetValue(PromptHorizontalAlignmentProperty);  set => SetValue(PromptHorizontalAlignmentProperty, value); }
        public VerticalAlignment   PromptVerticalAlignment    { get => (VerticalAlignment)  GetValue(PromptVerticalAlignmentProperty);    set => SetValue(PromptVerticalAlignmentProperty, value); }

        public DateTime            MinDate                    { get => (DateTime)           GetValue(MinDateProperty);                    set => SetValue(MinDateProperty, value); }
        public DateTime            MaxDate                    { get => (DateTime)           GetValue(MaxDateProperty);                    set => SetValue(MaxDateProperty, value); }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NxCalendar control && control._calendar != null)
            {
                //try
                //{
                //    if (helpers.IsDate(e.NewValue.ToString()))
                //    {
                //        DateTime dt = (DateTime)e.NewValue;
                //        if (dt.Year>1900)
                //        {
                //            control._calendar.SelectedDates.Clear();
                //            control._calendar.SelectedDates.Add(new DateTimeOffset((DateTime)e.NewValue));

                //            //control._calendar.SetDisplayDate(new DateTimeOffset((DateTime)e.NewValue));
                //        }
                //    }
                //}
                //catch { }
            }
        }

        #endregion

        public NxCalendar()
        {
            this.DefaultStyleKey = typeof(NxCalendar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _calendar = GetTemplateChild("PART_Calendar") as CalendarView;

            if (_calendar != null)
            {
                _calendar.SelectedDatesChanged += Calendar_DateChanged;
                _calendar.CalendarViewDayItemChanging += MyCalendar_CalendarViewDayItemChanging;
                _calendar.Tapped += Calendar_Tapped;
                _calendar.KeyDown += Calendar_KeyDown;

                try
                {
                    _calendar.SelectedDates.Clear();
                    _calendar.SelectedDates.Add(new DateTimeOffset(Value));
                    _calendar.SetDisplayDate(new DateTimeOffset((DateTime)Value));
                }
                catch
                {
                    _calendar.SelectedDates.Add(new DateTimeOffset(DateTime.Now));
                }
            }


        }

        private void Calendar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                isEnterKey++;
                if (isEnterKey > 1)
                {
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                    isEnterKey = 0;
                }
            }
        }

        private void Calendar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Optional: handle tap logic here
        }

        private void Calendar_DateChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            var selectedDate = sender.SelectedDates.FirstOrDefault();

            if (selectedDate != null)
            {
                if (selectedDate.LocalDateTime.Date != Value.Date)
                {
                    Value = selectedDate.LocalDateTime;

                    if (RaiseOnChange(Value.ToString("MMM dd, yyyy")))
                        RaiseOnChanged(Value.ToString("MMM dd, yyyy"));
                }
            }
        }

        private void MyCalendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(MyCalendar_CalendarViewDayItemChanging);
            }
            else if (args.Phase == 1)
            {
                var itemDate = args.Item.Date;
                var selectedDate = sender.SelectedDates.FirstOrDefault();

                if (itemDate == selectedDate)
                {
                    args.Item.Background = new SolidColorBrush(Colors.LightGreen); // Your desired color
                }
                else
                {
                    args.Item.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
    }
}