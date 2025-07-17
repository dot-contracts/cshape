using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using nexus.common.control;
using nexus.common.control.Themes;

using nexus;
using nexus.common.control;

namespace nexus.common.control
{
    public partial class NxMenuItem : NxControlBase 
    {
        private Storyboard sbClickMe;
        private bool isFocused  = true;
        private bool isHovering = false;
        private bool hasWidth   = false;
        private double maxLoops;

        public string Assembly   { get; set; }
        public string Entry      { get; set; }
        public string SnapIn     { get; set; }
        public int    ModuleID   { get; set; }
        public int    FunctionID { get; set; }

        private bool                 isReadonly        = false;                        public bool                  IsReadOnly        { get { return isReadonly; }        set { isReadonly = value;     } }
        private bool                 pictureVisible    = false;                        public bool                  PictureVisible    { get { return pictureVisible; }    set { pictureVisible = value; } }

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptForeColorProperty           = DependencyProperty.Register("PromptForeColor",           typeof(Brush),                 typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptBackColorProperty           = DependencyProperty.Register("PromptBackColor",           typeof(Brush),                 typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxMenuItem), new PropertyMetadata(null));

        public     string                Prompt                     { get { return (string)                 GetValue(PromptProperty); }                    set { SetValue(PromptProperty,                    value); } }
        public     double                PromptWidth                { get { return (double)                 GetValue(PromptWidthProperty); }               set { SetValue(PromptWidthProperty,               value); } }
        public     FontFamily            PromptFontFamily           { get { return (FontFamily)             GetValue(PromptFontFamilyProperty); }          set { SetValue(PromptFontFamilyProperty,          value); } }
        public     double                PromptFontSize             { get { return (double)                 GetValue(PromptFontSizeProperty); }            set { SetValue(PromptFontSizeProperty,            value); } }
        public     Brush                 PromptForeColor            { get { return (Brush)                  GetValue(PromptForeColorProperty); }           set { SetValue(PromptForeColorProperty,           value); } }
        public     Brush                 PromptBackColor            { get { return (Brush)                  GetValue(PromptBackColorProperty); }           set { SetValue(PromptBackColorProperty,           value); } }
        public     Thickness             PromptMargin               { get { return (Thickness)              GetValue(PromptMarginProperty); }              set { SetValue(PromptMarginProperty,              value); } }
        public     double                MaxPromptWidth             { get { return (double)                 GetValue(MaxPromptWidthProperty); }            set { SetValue(MaxPromptWidthProperty,            value); } }
        public     HorizontalAlignment   PromptHorizontalAlignment  { get { return (HorizontalAlignment)    GetValue(PromptHorizontalAlignmentProperty); } set { SetValue(PromptHorizontalAlignmentProperty, value); } }
        public     VerticalAlignment     PromptVerticalAlignment    { get { return (VerticalAlignment)      GetValue(PromptVerticalAlignmentProperty); }   set { SetValue(PromptVerticalAlignmentProperty,   value); } }
        public     TextWrapping          PromptTextWrapping         { get { return (TextWrapping)           GetValue(PromptTextWrappingProperty); }        set { SetValue(PromptTextWrappingProperty,        value); } }
        

        #endregion

        #region Dependency Property
        public static readonly DependencyProperty ActionCodeProperty            = DependencyProperty.Register("ActionCode",        typeof(string),       typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly DependencyProperty InnerBorderRadiusProperty     = DependencyProperty.Register("InnerBorderRadius", typeof(CornerRadius), typeof(NxMenuItem), new PropertyMetadata(default(CornerRadius), new PropertyChangedCallback(OnInnerBorderRadiusChanged)));
        public static readonly DependencyProperty BorderColorProperty           = DependencyProperty.Register("BorderColor",       typeof(Brush),        typeof(NxMenuItem), new PropertyMetadata(null));
        public static readonly DependencyProperty BorderRadiusProperty          = DependencyProperty.Register("BorderRadius",      typeof(CornerRadius), typeof(NxMenuItem), new PropertyMetadata(default(CornerRadius), new PropertyChangedCallback(OnBorderRadiusChanged)) );
        public static readonly DependencyProperty IsCheckedProperty             = DependencyProperty.Register("IsChecked",         typeof(bool),         typeof(NxMenuItem), new PropertyMetadata(null));

        private double              pictureWidth      = 25;                        public double              PictureWidth               { get { return pictureWidth; }      set { pictureWidth = value;      setPictureMargins(); } }
        private double              pictureHeight     = 25;                        public double              PictureHeight              { get { return pictureHeight; }     set { pictureHeight = value;     setPictureMargins(); } }
        private HorizontalAlignment pictureHorizontal = HorizontalAlignment.Right; public HorizontalAlignment PictureHorizontalAlignment { get { return pictureHorizontal; } set { pictureHorizontal = value; setPictureMargins(); } }
        private VerticalAlignment   pictureVertical   = VerticalAlignment.Bottom;  public VerticalAlignment   PictureVerticalAlignment   { get { return pictureVertical; }   set { pictureVertical = value;   setPictureMargins(); } }
        private NxImage.Pictures    picture           = NxImage.Pictures.tick;     public NxImage.Pictures    Picture                    { get { return picture; }           set { picture = value; if (_image != null) _image.Picture = picture; } }

        public Brush               BorderColor                 { get { return (Brush)               GetValue(BorderColorProperty);}                 set { SetValue(BorderColorProperty,value);}}
        public CornerRadius        InnerBorderRadius           { get { return (CornerRadius)        GetValue(InnerBorderRadiusProperty); }          set { SetValue(InnerBorderRadiusProperty, value); } }

        public string              ActionCode                  { get { return (string)              GetValue(ActionCodeProperty); }                 set { SetValue(ActionCodeProperty, value); } }
      
        public bool                IsChecked                   { get { return (bool)                GetValue(IsCheckedProperty); }                  set { SetValue(IsCheckedProperty, value); } }

        public delegate void ShowProgressCB(int Maximum);

        private   static  void  OnBorderRadiusChanged       (DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NxMenuItem buttonChrome = obj as NxMenuItem;
            if (buttonChrome != null)
                buttonChrome.OnBorderRadiusChanged((CornerRadius)e.OldValue, (CornerRadius)e.NewValue);
        }
        protected virtual void  OnBorderRadiusChanged       (CornerRadius oldValue, CornerRadius newValue)
        {
            //_innerBorderRadius to be one less than the CornerRadius
            CornerRadius newInnerBorderRadius = new CornerRadius(Math.Max(0, newValue.TopLeft - 1),
                                                                 Math.Max(0, newValue.TopRight - 1),
                                                                 Math.Max(0, newValue.BottomRight - 1),
                                                                 Math.Max(0, newValue.BottomLeft - 1));

            InnerBorderRadius = newInnerBorderRadius;
        }
        private   static  void  OnInnerBorderRadiusChanged  (DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NxMenuItem button = o as NxMenuItem;
            if (button != null)
                button.OnInnerBorderRadiusChanged((CornerRadius)e.OldValue, (CornerRadius)e.NewValue);
        }
        protected virtual void  OnInnerBorderRadiusChanged  (CornerRadius oldValue, CornerRadius newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }


        #endregion

        #region Property
        private Border    _inner;
        private Border    _outer;
        private NxImage   _image;
        private TextBlock _label;
        #endregion

        #region Event
        public event  OnMenuItemClickedEventHandler OnMenuItemClicked;  public delegate void OnMenuItemClickedEventHandler (NxMenuItem MenuItem,string Prompt, string Assembly, string Entry, string SnapIn, int ModuleId, int FunctionID); 
        public event  OnClickedEventHandler         OnClicked;          public delegate void OnClickedEventHandler         (string Tag, WinRect ScreenRect);
        #endregion

        #region Constructor

        public NxMenuItem()
        {
            DefaultStyleKey    = typeof(NxMenuItem);

            PointerReleased += NxMenuItem_PointerReleased;
            PointerPressed += NxMenuItem_PointerPressed;
            PointerEntered += nxMenuItem_MouseEnter;
            PointerExited  += nxMenuItem_MouseLeave;
            LostFocus      += nxMenuItem_LostFocus;
            KeyUp          += nxMenuItem_KeyUp;

            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }
        private void ApplyThemeDefaults()
        {
            if (_label != null)
            {
                _label.Foreground  = NxThemeManager.Current.GetThemeBrush("NxMenuItemFore",  Colors.LightGray);
                _outer.Background  = NxThemeManager.Current.GetThemeBrush("NxMenuItemBack",  Colors.LightGray);
                _outer.BorderBrush = NxThemeManager.Current.GetThemeBrush("NxMenuDivider", Colors.LightGray);
                _inner.Background  = NxThemeManager.Current.GetThemeBrush("NxMenuItemBack",  Colors.LightGray);
                _inner.BorderBrush = NxThemeManager.Current.GetThemeBrush("NxMenuDivider", Colors.LightGray);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _inner  = GetTemplateChild("inner")   as Border;
            _outer  = GetTemplateChild("outer")   as Border;
            _image  = GetTemplateChild("picture") as NxImage;
            _label  = GetTemplateChild("label")   as TextBlock;
        }

        #endregion

        #region Private Methods

        void nxMenuItem_MouseEnter (object sender, PointerRoutedEventArgs e)  { setFocusState(true); isHovering = true;  }
        //void nxMenuItem_GotFocus   (object sender, RoutedEventArgs e)         { e.Handled = true;   }
        void nxMenuItem_MouseLeave (object sender, PointerRoutedEventArgs e)  { setFocusState(false); isHovering = false; }
        void nxMenuItem_LostFocus  (object sender, RoutedEventArgs e)         { setFocusState(false); isFocused = true; }

        public void setFocusState(bool State)
        {
            if (!isReadonly)
            {
                if (_outer  != null)
                {
                    _inner.Background = NxThemeManager.Current.GetThemeBrush(State ? "NxMenuItemHover" : "NxMenuItemBack", Colors.LightGray);
                    _outer.Background = NxThemeManager.Current.GetThemeBrush(State ? "NxMenuItemHover" : "NxMenuItemBack", Colors.LightGray);
                }
                if (State) isHovering = true;
            }
        }

        private void NxMenuItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!isReadonly)
            {
                if (isHovering) 
                {

                    _inner.Background = NxThemeManager.Current.GetThemeBrush("NxMenuItemPressed", Colors.LightGray);
                    _outer.Background = NxThemeManager.Current.GetThemeBrush("NxMenuItemPressed", Colors.LightGray);

                    IsChecked = !IsChecked;
                    OnClicked?.Invoke(string.IsNullOrEmpty(Tag.ToString()) ? Name : Tag.ToString(), ScreenRect());
                    OnMenuItemClicked?.Invoke(this, Prompt, Assembly, Entry, SnapIn, ModuleID, FunctionID);
                }
            }
        }
        private void NxMenuItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _inner.Background = NxThemeManager.Current.GetThemeBrush("NxMenuItemHover", Colors.LightGray);
            _outer.Background = NxThemeManager.Current.GetThemeBrush("NxMenuItemHover", Colors.LightGray);
        }


        void nxMenuItem_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (!isReadonly )
            {
                try
                {
                    switch (e.Key)
                    {
                        case VirtualKey.Enter:
                        case VirtualKey.Tab:
                            isFocused = !isFocused;
                            if (OnClicked != null && isFocused) OnClicked(string.IsNullOrEmpty(Tag.ToString()) ? Name : Tag.ToString(), ScreenRect());
                            //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            //    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                            //else
                            //    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                            //e.Handled = true;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex) { string m = ex.Message; }
            }
            else
            {
                e.Handled = true;
            }

        }

        private void setPictureMargins()
        {
            try
            {
                if (ActualWidth==0) return;

                hasWidth=true;
                if (_image != null)
                {
                    _image.Picture = picture;
                    _image.Width   = pictureWidth;
                    _image.Height  = pictureHeight;
                    _image.Visibility = pictureVisible ? Visibility.Visible : Visibility.Collapsed;

                    Thickness Thickness = _image.Margin;

                    double twidth  = ActualWidth   > 0 ? ActualWidth  : Width > 0 ? Width : 0;
                    double theight = ActualHeight  > 0 ? ActualHeight : Height > 0 ? Height : 0;
                    double pwidth  = _image.ActualWidth  > 0 ? _image.ActualWidth  : _image.Width > 0 ? _image.Width : 0;
                    double pheight = _image.ActualHeight > 0 ? _image.ActualHeight : _image.Height > 0 ? _image.Height : 0;

                    switch (pictureHorizontal)
                    {
                        case HorizontalAlignment.Center:
                            Thickness.Left =  twidth / 2 -  pwidth / 2;
                            Thickness.Right = twidth / 2 -  pwidth / 2;
                            break;
                        case HorizontalAlignment.Left:
                            Thickness.Left = 0;
                            Thickness.Right = twidth - pwidth;
                            break;
                        case HorizontalAlignment.Right:
                            Thickness.Left = twidth - pwidth;
                            Thickness.Right = 0;
                            break;
                        case HorizontalAlignment.Stretch:
                            break;
                    }

                    switch (pictureVertical)
                    {
                        case VerticalAlignment.Top:
                            Thickness.Top    = 0;
                            Thickness.Bottom = theight - pheight;
                            break;
                        case VerticalAlignment.Center:
                            Thickness.Top    = theight / 2 - pheight / 2;
                            Thickness.Bottom = theight / 2 - pheight / 2;
                            break;
                        case VerticalAlignment.Bottom:
                            Thickness.Top    = theight - pheight;
                            Thickness.Bottom = 0;
                            break;
                        case VerticalAlignment.Stretch:
                            Thickness.Top    = theight - pheight;
                            Thickness.Bottom = 0;
                            break;
                    }

                    _image.Margin = Thickness;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion


        public WinRect ScreenRect()                     { return dialoghelpers.ScreenRect(this); }
        //public System.Drawing.Point    GetDialogLocation(Object Form)   { return dialogs.GetDialogLocation(Form, this); }

        public void OnStartLoad(int MaxLoops)
        {
            maxLoops = MaxLoops;
            if (_outer != null)
            {
                _outer.Visibility = Visibility.Visible;
                _outer.Margin = new Thickness(0);
            }
        }
        public void OnProgress(int Progress)
        {
            DispatcherQueue.TryEnqueue(() => _OnProgress(Progress));
        }
        private void _OnProgress(int Progress)
        {
            if (_outer != null)
            {
                double right = ActualWidth - Progress / maxLoops * ActualWidth;
                _outer.Margin = new Thickness(0, 0, right , 0);
            }
           
        }
        public void OnLoaded ()
        {
            if (_outer != null)
            {
                _outer.Visibility = Visibility.Collapsed;
            }
        }



    }
}
