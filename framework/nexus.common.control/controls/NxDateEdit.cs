using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

using nexus.common;
using nexus.common.control;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nexus.common.control
{
    public enum Months {January = 1,February = 2,March = 3,April = 4,May = 5,June = 6,July = 7,August = 8,September = 9,October = 10,November = 11,December = 12}

    public partial class NxDateEdit : NxControlBase, IControl
    {

        #region Attribute

        public enum PromptVisibilityState  {Visible,Hidden}
        public static readonly string[] Months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "Novemeber", "December" };

        readonly Dictionary<KeyValuePair<int, int>, DayCell[]> daysArrays = new Dictionary<KeyValuePair<int, int>, DayCell[]>();

        private static DayCell[] mListOfDays;

        private static int mcurrentlyViewedYear;
        private static int mcurrentlyViewedMonth;
        #endregion

        private        bool isValid      = false;  public     bool IsValid     {  get { return isValid; } set { isValid = value; if (Editor != null) Editor.IsValid = value; } }
        private        bool isValidated  = false;  public     bool IsValidated {  get { return isValidated; } set { isValidated = value; } }
        private        bool isValidating = false;  public     bool IsValidating { get { return isValidating; } set { isValidating = value; } }
        private        bool isTouchMode  = false;  public     bool IsTouchMode {  get { return isTouchMode; } set { isTouchMode = value; } }
        private static bool isChanged    = false;  public     bool IsChanged {    get { return isChanged; } set { isChanged = value; } }

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxDateEdit), new PropertyMetadata(null));
        //public static readonly      DependencyProperty PromptFontWeightProperty          = DependencyProperty.Register("PromptFontWeight",          typeof(FontWeight),            typeof(NxDateEdit));
        //public static readonly      DependencyProperty PromptForeColorProperty           = DependencyProperty.Register("PromptForeColor",           typeof(Brush),                 typeof(NxDateEdit), new PropertyMetadata(null));
        //public static readonly      DependencyProperty PromptBackColorProperty           = DependencyProperty.Register("PromptBackColor",           typeof(Brush),                 typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxDateEdit), new PropertyMetadata(null));

        public     string                Prompt                     { get { return (string)                 GetValue(PromptProperty); }                    set { SetValue(PromptProperty,                    value); } }
        public     double                PromptWidth                { get { return (double)                 GetValue(PromptWidthProperty); }               set { SetValue(PromptWidthProperty,               value); } }
        public     FontFamily            PromptFontFamily           { get { return (FontFamily)             GetValue(PromptFontFamilyProperty); }          set { SetValue(PromptFontFamilyProperty,          value); } }
        public     double                PromptFontSize             { get { return (double)                 GetValue(PromptFontSizeProperty); }            set { SetValue(PromptFontSizeProperty,            value); } }
        //public     FontWeight            PromptFontWeight           { get { return (FontWeight)             GetValue(PromptFontWeightProperty); }          set { SetValue(PromptFontWeightProperty,          value); } }
        //public     Brush                 PromptForeColor            { get { return (Brush)                  GetValue(PromptForeColorProperty); }           set { SetValue(PromptForeColorProperty,           value); } }
        //public     Brush                 PromptBackColor            { get { return (Brush)                  GetValue(PromptBackColorProperty); }           set { SetValue(PromptBackColorProperty,           value); } }
        public     Thickness             PromptMargin               { get { return (Thickness)              GetValue(PromptMarginProperty); }              set { SetValue(PromptMarginProperty,              value); } }
        public     double                MaxPromptWidth             { get { return (double)                 GetValue(MaxPromptWidthProperty); }            set { SetValue(MaxPromptWidthProperty,            value); } }
        public     HorizontalAlignment   PromptHorizontalAlignment  { get { return (HorizontalAlignment)    GetValue(PromptHorizontalAlignmentProperty); } set { SetValue(PromptHorizontalAlignmentProperty, value); } }
        public     VerticalAlignment     PromptVerticalAlignment    { get { return (VerticalAlignment)      GetValue(PromptVerticalAlignmentProperty); }   set { SetValue(PromptVerticalAlignmentProperty,   value); } }
        #endregion
        

        #region Editor Dependency Property
        public static readonly new DependencyProperty IsReadOnlyProperty               = DependencyProperty.Register("IsReadOnly",               typeof(bool),                    typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty IsRequiredProperty               = DependencyProperty.Register("IsRequired",               typeof(bool),                    typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty SuppressNavigateProperty         = DependencyProperty.Register("SuppressNavigate",         typeof(bool),                    typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyFontFamilyProperty          = DependencyProperty.Register("ReplyFontFamily",          typeof(FontFamily),              typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyFontSizeProperty            = DependencyProperty.Register("ReplyFontSize",            typeof(double),                  typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyRequiredTextProperty        = DependencyProperty.Register("ReplyRequiredText",        typeof(string),                  typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyInvalidPictureProperty      = DependencyProperty.Register("ReplyInvalidPicture",      typeof(NxImage.Pictures),        typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyValidPictureProperty        = DependencyProperty.Register("ReplyValidPicture",        typeof(NxImage.Pictures),        typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty MaxTextLengthProperty            = DependencyProperty.Register("MaxTextLength",            typeof(int),                     typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty MaskTypeProperty                 = DependencyProperty.Register("MaskType",                 typeof(MaskTypes),               typeof(NxDateEdit), new PropertyMetadata(null)); 
        public static readonly     DependencyProperty MaskProperty                     = DependencyProperty.Register("Mask",                     typeof(string),                  typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ChangeEventTypeProperty          = DependencyProperty.Register("ChangeEventType",          typeof(NxText.ChangeEventTypes), typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyHorizontalAlignmentProperty = DependencyProperty.Register("ReplyHorizontalAlignment", typeof(HorizontalAlignment),     typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyVerticalAlignmentProperty   = DependencyProperty.Register("ReplyVerticalAlignment",   typeof(VerticalAlignment),       typeof(NxDateEdit), new PropertyMetadata(null));

        public new  bool                    IsReadOnly               { get { return (bool)                    GetValue(IsReadOnlyProperty); }               set { SetValue(IsReadOnlyProperty,               value); Resize(); } }
        public      bool                    IsRequired               { get { return (bool)                    GetValue(IsRequiredProperty); }               set { SetValue(IsRequiredProperty,               value); } }
        public      bool                    SuppressNavigate         { get { return (bool)                    GetValue(SuppressNavigateProperty); }         set { SetValue(SuppressNavigateProperty,         value); } }
        public      FontFamily              ReplyFontFamily          { get { return (FontFamily)              GetValue(ReplyFontFamilyProperty); }          set { SetValue(ReplyFontFamilyProperty,          value); } }
        public      double                  ReplyFontSize            { get { return (double)                  GetValue(ReplyFontSizeProperty); }            set { SetValue(ReplyFontSizeProperty,            value); } }
        public      string                  ReplyRequiredText        { get { return (string)                  GetValue(ReplyRequiredTextProperty); }        set { SetValue(ReplyRequiredTextProperty,        value); } }
        public      NxImage.Pictures        ReplyInvalidPicture      { get { return (NxImage.Pictures)        GetValue(ReplyInvalidPictureProperty);}       set { SetValue(ReplyInvalidPictureProperty,      value); } }
        public      NxImage.Pictures        ReplyValidPicture        { get { return (NxImage.Pictures)        GetValue(ReplyValidPictureProperty);}         set { SetValue(ReplyValidPictureProperty,        value); } }
        public      int                     MaxTextLength            { get { return (int)                     GetValue(MaxTextLengthProperty); }            set { SetValue(MaxTextLengthProperty,            value); } }
        public      MaskTypes               MaskType                 { get { return (MaskTypes)               GetValue(MaskTypeProperty); }                 set { SetValue(MaskTypeProperty,                 value); } }
        public      string                  Mask                     { get { return (string)                  GetValue(MaskProperty); }                     set { SetValue(MaskProperty,                     value); } }
        public      NxText.ChangeEventTypes ChangeEventType          { get { return (NxText.ChangeEventTypes) GetValue(ChangeEventTypeProperty); }          set { SetValue(ChangeEventTypeProperty,          value); } }
        public      HorizontalAlignment     ReplyHorizontalAlignment { get { return (HorizontalAlignment)     GetValue(ReplyHorizontalAlignmentProperty); } set { SetValue(ReplyHorizontalAlignmentProperty, value); } }
        public      VerticalAlignment       ReplyVerticalAlignment   { get { return (VerticalAlignment)       GetValue(ReplyVerticalAlignmentProperty); }   set { SetValue(ReplyVerticalAlignmentProperty,   value); } }
        
        public Brush PromptBackColor { get => PromptBack; set => PromptBack = value; }
        public Brush PromptForeColor { get => PromptFore; set => PromptFore = value; }
        public Brush ReplyBack    { get => ReplyBack;    set => ReplyBack  = value; }
        public Brush ReplyFore    { get => ReplyFore;    set => ReplyFore  = value; }
        public Brush ReplyInValid { get => ReplyInValid; set => ReplyInValid = value; }
        public Brush ReplyValid   { get => ReplyValid;   set => ReplyValid = value; }

        
        #endregion

        #region Event
        public delegate void OnProcessKeyEventHandler (string Key);                                                     public event OnProcessKeyEventHandler  OnProcessKey;

        #endregion

        #region Value Dependency Property
        public static readonly DependencyProperty MinDateProperty      = DependencyProperty.RegisterAttached("MinDate", typeof(DateTime), typeof(NxDateEdit), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(NxDateEdit), new PropertyMetadata(DateTime.Now));
        public static readonly DependencyProperty MaxDateProperty      = DependencyProperty.RegisterAttached("MaxDate", typeof(DateTime), typeof(NxDateEdit), new PropertyMetadata(DateTime.MaxValue, 
                                                                            delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                                                                            {
                                                                                NxDateEdit dateEdit = (NxDateEdit)sender;
                                                                                if (dateEdit.MaxDate.Ticks < dateEdit.SelectedDate.Ticks) dateEdit.SelectedDate = dateEdit.MaxDate;
                                                                                dateEdit.SetListOfDays();
                                                                            }));

        public static readonly DependencyProperty MinAgeProperty = DependencyProperty.RegisterAttached("MinAge", typeof(int), typeof(NxDateEdit), new PropertyMetadata(8));
        public static readonly DependencyProperty MaxAgeProperty = DependencyProperty.RegisterAttached("MaxAge", typeof(int), typeof(NxDateEdit), new PropertyMetadata(120) );

        public DateTime SelectedDate { get { return (DateTime)GetValue(SelectedDateProperty); }  set { SetValue(SelectedDateProperty, value); } }
        public DateTime MinDate      { get { return (DateTime)GetValue(MinDateProperty); }       set { SetValue(MinDateProperty, value); } }
        public DateTime MaxDate      { get { return (DateTime)GetValue(MaxDateProperty); }       set { SetValue(MaxDateProperty, value); } }
        public int      MinAge       { get { return (int)GetValue(MinAgeProperty); }             set { SetValue(MinAgeProperty, value); } }
        public int      MaxAge       { get { return (int)GetValue(MaxAgeProperty); }             set { SetValue(MaxAgeProperty, value); } }

        private string mValue = "";
        public  string Value
        { 
            get { return mValue; }       
            set 
            { 
                DateTime dout;
                if (DateTime.TryParse(value, out dout))
                {
                    mValue = value;
                    if (Editor   != null) Editor.Value    = mValue;
                    if (Calendar != null) Calendar.Value  = dout;
                }
            } 
        }

        #endregion

        #region Property

        public double ValueDbl { get; set; } = 0;
        protected NxText     Editor   { get { return base.GetTemplateChild("editor")   as NxText; } }
        protected NxLabel    Label    { get { return base.GetTemplateChild("label")    as NxLabel; } }
        protected NxImage    Button   { get { return base.GetTemplateChild("button")   as NxImage; } }
        protected NxCalendar Calendar { get { return base.GetTemplateChild("calendar") as NxCalendar; } }
        protected Grid       Dropdown { get { return base.GetTemplateChild("DropDown") as Grid; } }
        public static IEnumerable<string> MonthsList
        {
            get
            {
                return Months;
            }

        }

        public static DayCell[] DayList { get { return mListOfDays; } }

        public static string CurrentMonth { get { return ((Months)mcurrentlyViewedMonth).ToString(); } }

        public static string CurrentYear { get { return mcurrentlyViewedYear.ToString(); } }

        public void Focus() { }

        public bool ProcessKey(string key, bool sendKey) { return false; }
        #endregion

        #region Constructor

        public NxDateEdit()
        {
            DefaultStyleKey = typeof(NxDateEdit);

            if (Tag == null) Tag = string.Empty;

            this.IsTabStop = false;

            mcurrentlyViewedMonth = DateTime.Now.Month;
            mcurrentlyViewedYear  = DateTime.Now.Year;

            SetListOfDays();

            //this.KeyDown  += new KeyEventHandler(NxDateEdit_KeyDown);
        }

        public void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Editor != null)
            {
                Editor.OnChanged    += Editor_OnChanged;
                Editor.OnProcessKey += Editor_OnProcessKey;
                Editor.Value      = mValue;
            }

            if (Button != null)
                Button.OnClicked += Button_OnClick;

            if (Calendar != null)
            {
                DateTime dout;
                if (DateTime.TryParse(mValue, out dout))
                {
                    if (Calendar != null) Calendar.Value = dout;
                }
                Calendar.OnChanged += Calendar_OnChanged;
            }

            base.SizeChanged += NxDateEdit_SizeChanged;
            Resize();
        }

        private void Editor_OnClicked(string Tag, System.Drawing.Rectangle ScreenRect)
        {
            RaiseOnClicked(true);
        }

        private void Editor_OnChanged(object? sender, ChangedEventArgs e)
        {

            Array delims = "-|/|.".Split('|');
            for (int i = 0; i < delims.GetLength(0) - 1; i++)
            {
                char delim = Convert.ToChar(delims.GetValue(i).ToString());
                if (Value.Contains(delim.ToString()))
                {
                    Array vars = (e.Value + delim + delim + delim).Split(delim);
                    string yr = vars.GetValue(2).ToString();
                    if (!string.IsNullOrEmpty(yr))
                    {
                        int yri = 0;
                        if (int.TryParse(yr, out yri))
                        {
                            if (yr.Length < 3)
                            {
                                if (((2000 + yri) - DateTime.Now.Year) > -10)
                                    yri = yri + 1900;
                                else
                                    yri = yri + 2000;

                                Value = vars.GetValue(0).ToString() + "/" + vars.GetValue(1).ToString() + "/" + yri.ToString();
                            }
                            DateTime dt = Convert.ToDateTime(Value);
                            //IsValid = (DateTime.Now.Subtract(dt).TotalDays > (365 * 10));
                        }
                    }
                    break;
                }

            }

            mValue = Value;


            if (helpers.IsDate(mValue))
            {
                if (RaiseOnChange(mValue))
                {
                    Editor.Value = mValue;
                    RaiseOnChanged(mValue);
                }
            }
        }
        private void Editor_OnProcessKey(string Key)
        {
            OnProcessKey?.Invoke(Key);
        }

        private void Calendar_OnChanged(object? sender, ChangedEventArgs e)
        {
            mValue = e.Value;
            Editor.Value = mValue;

            if (helpers.IsDate(mValue))
            {
                if (RaiseOnChange(mValue))
                {
                    Editor.Value = mValue;
                    RaiseOnChanged(mValue);
                }
            }

            //DispatcherQueue.TryEnqueue(() => IsDropDownOpen = false);
            Editor.SetFocus();
        }
        void NxDateEdit_SizeChanged(object sender, SizeChangedEventArgs e) { Resize(); }
        public void Resize()
        {
            try
            {
                if (Editor != null && Label != null && Button != null )
                {
                    Label.Height  = ActualHeight;
                    Label.Width   = PromptWidth;

                    Editor.Height = ActualHeight;
                    Editor.Width = (ActualWidth - Label.Width - (!IsReadOnly ? Button.Width : 0));

                    //Dropdown.Margin = new Thickness( Label.Width, 0, 0, 0); 

                }
            }
            catch (Exception x)
            { }
        }
        public void SetFocus()
        {
            if (Editor != null) Editor.SetFocus();
        }
        public bool Validate()
        {
            return true;
        }

        void Button_OnClick(string Tag)
        {
            if (IsReadOnly)
                return;

            //if (!IsDropDownOpen)
            //{
            //    mValue = Editor?.Value;

            //    if (DateTime.TryParse(mValue, out DateTime parsedDate))
            //        Calendar.Value = parsedDate;

            //    DispatcherQueue.TryEnqueue(() =>
            //    {
            //        IsDropDownOpen = true;
            //        Calendar?.Focus(FocusState.Programmatic);
            //    });
            //}
            //else
            //{
            //    DispatcherQueue.TryEnqueue(() => IsDropDownOpen = false);
            //}
        }


        #endregion


        #region Public Methods

        public void SetListOfDays()
        {
            try
            {
                int numberOfDaysFromPreviousMonth = (int)GetDayOfWeek(mcurrentlyViewedYear, mcurrentlyViewedMonth, 1);

                DayCell[] newDaysTemp = GetDaysOfMonth(mcurrentlyViewedMonth, mcurrentlyViewedYear, MinDate, MaxDate);

                int numberOfDaysFromNextMonth = 6 - (int)GetDayOfWeek(mcurrentlyViewedYear, mcurrentlyViewedMonth, newDaysTemp[newDaysTemp.Length - 1].DayNumber);
                DayCell[] newDays = new DayCell[newDaysTemp.Length + numberOfDaysFromNextMonth];

                int monthToGetNext, yearToGetNext;

                MoveMonthForward(mcurrentlyViewedMonth, mcurrentlyViewedYear, out monthToGetNext, out yearToGetNext);

                DayCell[] nextDays = GetDaysOfMonth(monthToGetNext, yearToGetNext, MinDate, MaxDate);

                newDaysTemp.CopyTo(newDays, 0);

                Array.Copy(nextDays, 0, newDays, newDaysTemp.Length, newDays.Length - newDaysTemp.Length);

                DayCell[] listOfDays = new DayCell[numberOfDaysFromPreviousMonth + newDays.Length];

                int monthToGetPrevious;
                int yearTogetPrevious;
                //move one month back
                MoveMonthBack(mcurrentlyViewedMonth, mcurrentlyViewedYear, out monthToGetPrevious, out yearTogetPrevious);
                DayCell[] oldDays = GetDaysOfMonth(monthToGetPrevious, yearTogetPrevious, MinDate, MaxDate);//get the previous month
                Array.Copy(oldDays, oldDays.Length - numberOfDaysFromPreviousMonth, listOfDays, 0, numberOfDaysFromPreviousMonth);
                Array.Copy(newDays, 0, listOfDays, numberOfDaysFromPreviousMonth, newDays.Length);

                mListOfDays = listOfDays;

            }
            catch (Exception x)
            { }

        }

        public void Clear()
        {
            mValue = string.Empty;
            if (Editor != null) Editor.Value = string.Empty;
            if (Calendar != null)
            {
                //Calendar.SelectedDates.Clear();
                //Calendar.UpdateCalendar();
               // Calendar.DisplayDate = DateTime.Now;
            }
            mcurrentlyViewedMonth = DateTime.Now.Month;
            mcurrentlyViewedYear = DateTime.Now.Year;
          

        }

        #endregion


        #region Helper Methods

        public string ValidateAge(string Value)
        {
            string ret = "InValid";

            DateTime dout;
            if (DateTime.TryParse(mValue, out dout))
            {
                ret = "Valid";
                if (MinAge != 0)
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - dout.Year;
                    if (dout > today.AddYears(-age)) age--;
                    if (age < MinAge) ret = "InvalidMin";
                }
                if (MaxAge != 0)
                {
                    if (DateTime.Now.Subtract(dout).TotalDays > (365 * MaxAge)) 
                        ret = "InvalidMax";
                }
            }
            return ret;
        }


        public DayCell[] GetDaysOfMonth(int month, int year, DateTime minDate, DateTime maxDate)
        {
            KeyValuePair<int, int> key = new KeyValuePair<int, int>(month, year);
            int daysCount = DateTime.DaysInMonth(year, month);

            DayCell[] days = null;

            if (daysArrays.ContainsKey(key))
            {
                days = daysArrays[key];
                foreach (DayCell item in days)
                    item.IsEnabled = IsDateWithinRange(minDate, maxDate, item);
            }
            else
            {
                days = new DayCell[daysCount];

                for (int i = 0; i < days.Length; i++)
                {
                    DayCell item = new DayCell(i + 1, month, year);
                    item.IsEnabled = IsDateWithinRange(minDate, maxDate, item);
                    days[i] = item;
                }

                daysArrays[key] = days;//cache the array
            }

            return days;
        }

        public static bool IsDateWithinRange(DateTime minDate, DateTime maxDate, DayCell cell)
        {
            long ticks = new DateTime(cell.YearNumber, cell.MonthNumber, cell.DayNumber).Ticks;
            return ticks >= minDate.Ticks && ticks <= maxDate.Ticks;
        }

        public static DayOfWeek GetDayOfWeek(int year, int month, int day)
        {
            return new DateTime(year, month, day).DayOfWeek;
        }


        public static void MoveMonthForward(int currentMonth, int currentYear, out int monthToGetNext, out int yearTogetNext)
        {
            monthToGetNext = currentMonth;
            yearTogetNext = currentYear;
            if (monthToGetNext < 12)
                monthToGetNext++;
            else//move a year forward
            {
                yearTogetNext++;
                monthToGetNext = 1;
            }
        }

        public static void MoveMonthBack(int currentMonth, int currentYear, out int monthToGetPrevious, out int yearToGetPrevious)
        {
            monthToGetPrevious = currentMonth;
            yearToGetPrevious = currentYear;
            if (monthToGetPrevious > 1)
                monthToGetPrevious--;
            else // move one year down
            {
                yearToGetPrevious--;
                monthToGetPrevious = 12;
            }
        }


        #endregion

    }


    public class DayCell : INotifyPropertyChanged
    {
        readonly int dayNumber;
        readonly int monthNumber;
        readonly int yearNumber;
        bool isEnabled = true;

        public int DayNumber { get { return dayNumber; } }

        public int MonthNumber { get { return monthNumber; } }

        public int YearNumber { get { return yearNumber; } }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }


        public DayCell(int day, int month, int year)
        {
            dayNumber = day;
            monthNumber = month;
            yearNumber = year;
        }

        #region INotifyPropertyChanged Members


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    //public class CellSizeConverter : IValueConverter
    //{
    //    const int daysToFitHorizontal = 7;
    //    const double minimumValue = 10;//the minum size to return

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        double valuePassed = (double)value;

    //        if (parameter != null && !Double.IsNaN(valuePassed))
    //        {
    //            if (parameter.ToString() == "widthCell")
    //            {
    //                return Math.Max(valuePassed / daysToFitHorizontal, minimumValue) - 2;
    //            }

    //            if (parameter.ToString() == "widthCellContainer")
    //            {
    //                return Math.Max(valuePassed - 10, minimumValue);
    //            }
    //        }
    //        return 20.0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotSupportedException("This is a one-way value converter. ConvertBack method is not supported.");
    //    }
    //    #endregion
    //}

   
}
