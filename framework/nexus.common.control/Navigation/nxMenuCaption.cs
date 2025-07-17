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
    public partial class NxMenuCaption : NxControlBase
    {
        #region Attribute

        private bool                 isReadonly        = false;                        public bool                  IsReadOnly        { get { return isReadonly; }        set { isReadonly = value;     } }
        private bool                 pictureVisible    = false;                        public bool                  PictureVisible    { get { return pictureVisible; }    set { pictureVisible = value; } }
        private bool hasWidth = false;

        #endregion

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptForeColorProperty           = DependencyProperty.Register("PromptForeColor",           typeof(Brush),                 typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptBackColorProperty           = DependencyProperty.Register("PromptBackColor",           typeof(Brush),                 typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxMenuCaption), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxMenuCaption), new PropertyMetadata(null));

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

        private double              pictureWidth      = 25;                        public double              PictureWidth               { get { return pictureWidth; }      set { pictureWidth = value;      setPictureMargins(); } }
        private double              pictureHeight     = 25;                        public double              PictureHeight              { get { return pictureHeight; }     set { pictureHeight = value;     setPictureMargins(); } }
        private HorizontalAlignment pictureHorizontal = HorizontalAlignment.Right; public HorizontalAlignment PictureHorizontalAlignment { get { return pictureHorizontal; } set { pictureHorizontal = value; setPictureMargins(); } }
        private VerticalAlignment   pictureVertical   = VerticalAlignment.Bottom;  public VerticalAlignment   PictureVerticalAlignment   { get { return pictureVertical; }   set { pictureVertical = value;   setPictureMargins(); } }
        private NxImage.Pictures    picture           = NxImage.Pictures.tick;     public NxImage.Pictures    Picture                    { get { return picture; }           set { picture = value; if (Image != null) Image.Picture = picture; } }

        public static readonly DependencyProperty InnerBorderRadiusProperty     = DependencyProperty.Register("InnerBorderRadius", typeof(CornerRadius), typeof(NxMenuCaption),  new PropertyMetadata(default(CornerRadius), new PropertyChangedCallback(OnInnerBorderRadiusChanged)));
        public static readonly DependencyProperty BorderRadiusProperty          = DependencyProperty.Register("BorderRadius",      typeof(CornerRadius), typeof(NxMenuCaption),  new PropertyMetadata(default(CornerRadius), new PropertyChangedCallback(OnBorderRadiusChanged)) );
        public static readonly DependencyProperty BorderColorProperty           = DependencyProperty.Register("BorderColor",       typeof(Brush),        typeof(NxMenuCaption),  new PropertyMetadata(null));


        public CornerRadius        InnerBorderRadius           { get { return (CornerRadius)        GetValue(InnerBorderRadiusProperty); }          set { SetValue(InnerBorderRadiusProperty, value); } }
        public CornerRadius        BorderRadius                { get { return (CornerRadius)        GetValue(BorderRadiusProperty); }               set { SetValue(BorderRadiusProperty, value); } }
        public Brush               BorderColor                 { get { return (Brush)               GetValue(BorderColorProperty);}                 set { SetValue(BorderColorProperty,value);}}


        private static void     OnBorderRadiusChanged       (DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NxMenuCaption buttonChrome = obj as NxMenuCaption;
            if (buttonChrome != null)
                buttonChrome.OnBorderRadiusChanged((CornerRadius)e.OldValue, (CornerRadius)e.NewValue);
        }
        protected virtual void  OnBorderRadiusChanged       (CornerRadius oldValue, CornerRadius newValue)
        {
            //InnerBorderRadius to be one less than the CornerRadius
            CornerRadius newInnerBorderRadius = new CornerRadius(Math.Max(0, newValue.TopLeft - 1),
                                                                 Math.Max(0, newValue.TopRight - 1),
                                                                 Math.Max(0, newValue.BottomRight - 1),
                                                                 Math.Max(0, newValue.BottomLeft - 1));

            InnerBorderRadius = newInnerBorderRadius;
        }
        private static void     OnInnerBorderRadiusChanged  (DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NxMenuCaption button = o as NxMenuCaption;
            if (button != null)
                button.OnInnerBorderRadiusChanged((CornerRadius)e.OldValue, (CornerRadius)e.NewValue);
        }
        protected virtual void  OnInnerBorderRadiusChanged  (CornerRadius oldValue, CornerRadius newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }


        #endregion 

        #region Event
        public delegate void OnClickEventHandler     (object Tag);        public event OnClickEventHandler    OnClick;
        public delegate void OnNavigateEventHandler  (string linkUrl);    public event OnNavigateEventHandler OnNavigate;        
        public delegate void OnDispose();
        #endregion

        #region Property
        protected NxImage Image { get { return GetTemplateChild("picture") as NxImage; } }
        protected TextBox Label { get { return GetTemplateChild("label") as TextBox; } }
        protected Border  Inner { get { return GetTemplateChild("inner") as Border; } }
        protected Border  Outer { get { return GetTemplateChild("outer") as Border; } }
        #endregion


        #region Constructor

        public NxMenuCaption()
        {

            DefaultStyleKey = typeof(NxMenuCaption);

            IsTabStop  = false;

            PointerPressed += NxMenuCaption_PointerPressed;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }
        private void ApplyThemeDefaults()
        {
            if (Outer != null)
            {
                Outer.BorderBrush = NxThemeManager.Current.GetThemeBrush("NxMenuCaptionBack", Colors.White);
                Label.Foreground  = NxThemeManager.Current.GetThemeBrush("NxMenuCaptionFore", Colors.LightGray);
                Label.Background  = NxThemeManager.Current.GetThemeBrush("NxMenuCaptionBack", Colors.LightGray);
                Outer.Background  = NxThemeManager.Current.GetThemeBrush("NxMenuCaptionBack", Colors.LightGray);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            setPictureMargins();
        }

        #endregion

        #region Private

        private void setPictureMargins()
        {
            try
            {
                if (ActualWidth==0) return;

                hasWidth=true;
                if (Image != null)
                {
                    Image.Picture = picture;
                    Image.Width   = pictureWidth;
                    Image.Height  = pictureHeight;
                    Image.Visibility = pictureVisible ? Visibility.Visible : Visibility.Collapsed;

                    Thickness Thickness = Image.Margin;

                    double twidth  = ActualWidth  > 0 ? ActualWidth  : Width > 0 ? Width : 0;
                    double theight = ActualHeight > 0 ? ActualHeight : Height > 0 ? Height : 0;
                    double pwidth  = Image.ActualWidth  > 0 ? Image.ActualWidth  : Image.Width > 0 ? Image.Width : 0;
                    double pheight = Image.ActualHeight > 0 ? Image.ActualHeight : Image.Height > 0 ? Image.Height : 0;

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

                    Image.Margin = Thickness;
                }
            }
            catch (Exception ex)
            {
            }
        }


        #endregion

        #region Override

        private void NxMenuCaption_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (OnClick != null) OnClick(Prompt);
        }

        #endregion
    }
}
