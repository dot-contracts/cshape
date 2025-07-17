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

using nexus.common;
using nexus.common.control;
using nexus.common.control.Themes;


namespace nexus.common.control
{
    public sealed partial class NxLabel : NxControlBase
    {
        #region Attribute

        //private Hyperlink hl;
        #endregion

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptTextAutofitProperty         = DependencyProperty.Register("PromptTextAutofit",         typeof(bool),                  typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMaxFontSizeProperty         = DependencyProperty.Register("PromptMaxFontSize",         typeof(double),                typeof(NxLabel), new PropertyMetadata(null));
        public static readonly      DependencyProperty BorderThicknessProperty           = DependencyProperty.Register(nameof(BorderThickness),     typeof(double),                typeof(NxLabel), new PropertyMetadata(0.0, OnBorderAppearanceChanged));
        public static readonly      DependencyProperty BorderColorProperty               = DependencyProperty.Register(nameof(BorderColor),         typeof(string),                typeof(NxLabel), new PropertyMetadata("NxBorderBrush", OnBorderAppearanceChanged));

        public     string                Prompt                     { get { return (string)                 GetValue(PromptProperty); }                    set { SetValue(PromptProperty,                    value); if (PromptTextAutofit) SetFontSize(); } }
        public     double                PromptWidth                { get { return (double)                 GetValue(PromptWidthProperty); }               set { SetValue(PromptWidthProperty,               value); } }
        public     FontFamily            PromptFontFamily           { get { return (FontFamily)             GetValue(PromptFontFamilyProperty); }          set { SetValue(PromptFontFamilyProperty,          value); } }
        public     double                PromptFontSize             { get { return (double)                 GetValue(PromptFontSizeProperty); }            set { SetValue(PromptFontSizeProperty,            value); } }
        public     Thickness             PromptMargin               { get { return (Thickness)              GetValue(PromptMarginProperty); }              set { SetValue(PromptMarginProperty,              value); } }
        public     double                MaxPromptWidth             { get { return (double)                 GetValue(MaxPromptWidthProperty); }            set { SetValue(MaxPromptWidthProperty,            value); } }
        public     HorizontalAlignment   PromptHorizontalAlignment  { get { return (HorizontalAlignment)    GetValue(PromptHorizontalAlignmentProperty); } set { SetValue(PromptHorizontalAlignmentProperty, value); } }
        public     VerticalAlignment     PromptVerticalAlignment    { get { return (VerticalAlignment)      GetValue(PromptVerticalAlignmentProperty); }   set { SetValue(PromptVerticalAlignmentProperty,   value); } }
        public     TextWrapping          PromptTextWrapping         { get { return (TextWrapping)           GetValue(PromptTextWrappingProperty); }        set { SetValue(PromptTextWrappingProperty,        value); } }
        public     bool                  PromptTextAutofit          { get { return (bool)                   GetValue(PromptTextAutofitProperty); }         set { SetValue(PromptTextAutofitProperty,         value); } }
        public     double                PromptMaxFontSize          { get { return (double)                 GetValue(PromptMaxFontSizeProperty); }         set { SetValue(PromptMaxFontSizeProperty,         value); } }
        public     double                BorderThickness            { get       => (double)                 GetValue(BorderThicknessProperty);             set => SetValue(BorderThicknessProperty, value); }
        public     string                BorderColor                { get       => (string)                 GetValue(BorderColorProperty);                 set => SetValue(BorderColorProperty, value); }

        public Brush PromptBackColor { get => PromptBack; set => PromptBack = value; }
        public Brush PromptForeColor { get => PromptFore; set => PromptFore = value; }

        #endregion

        #region Dependency Property

        public static readonly DependencyProperty IsLinkProperty                    = DependencyProperty.Register("IsLink", typeof(bool),                                   typeof(NxLabel), new PropertyMetadata(false));
        public static readonly DependencyProperty LinkURLProperty                   = DependencyProperty.Register("LinkURL", typeof(string),                                typeof(NxLabel), new PropertyMetadata(string.Empty));

        public bool     IsLink  { get { return (bool)GetValue(IsLinkProperty); }    set { SetValue(IsLinkProperty, value); } }
        public string   LinkURL { get { return (string)GetValue(LinkURLProperty); } set { SetValue(LinkURLProperty, value); } }

        private Border    _border;
        private TextBlock _label;

        #endregion 

        public override void AnimateBackground(Windows.UI.Color toColor, TimeSpan duration) => AnimateBrush(_border, Control.BackgroundProperty, toColor, duration);

        #region Event
        public delegate void OnNavigateEventHandler  (string linkUrl);    public event OnNavigateEventHandler OnNavigate;        
        #endregion

        #region Constructor

        public NxLabel()
        {
            DefaultStyleKey = typeof(NxLabel);

            if (Tag == null) Tag = string.Empty;

            this.IsTabStop      = false;
            this.Loaded        += new RoutedEventHandler(NxLabel_Loaded);
            this.SizeChanged   += NxLabel_SizeChanged;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

            this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _border  = GetTemplateChild("OuterBorder") as Border;
            _label   = GetTemplateChild("TextBlock")   as TextBlock;

            ApplyThemeDefaults();
        }
        private void ApplyThemeDefaults()
        {
            if (_border!=null)
            {
                _label.Foreground       = NxThemeManager.Current.GetThemeBrush("NxPromptFore",  Colors.White);
                _border.Background      = NxThemeManager.Current.GetThemeBrush("NxTransparent", Colors.Black);
                _border.BorderThickness = new Thickness(BorderThickness);
                _border.BorderBrush     = NxThemeManager.Current.GetThemeBrush(BorderColor, Colors.Gray);
                _border.CornerRadius    = new CornerRadius(8); // default rounded radius
            }
        }
        private static void OnBorderAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NxLabel label && label._border != null)
               label.ApplyThemeDefaults();
        }

        #endregion

        #region Private

        private void NxLabel_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLink)
                {
                    //if (!String.IsNullOrEmpty(LinkURL))
                    //{

                    //    hl = new Hyperlink();
                    //    hl.Inlines.Add(this.Prompt);
                    //    hl.NavigateUri = new Uri(this.LinkURL);
                    //    hl.RequestNavigate += new RequestNavigateEventHandler(hl_RequestNavigate);
                    //    this.PromptForeColor = Brushes.Blue;
                    //}
                }
            }
            catch (Exception x)
            { }
        }

        //private void hl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        //{
        //    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        //    e.Handled = true;
        //}

        private void NxLabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PromptTextAutofit) SetFontSize();
        }

        public void SetFontSize()
        {
            _label.Text = Prompt;
            //PromptFontSize = helpers.MakeTextFit(this, PromptMaxFontSize);
            if (_label!=null)
                _label.FontSize = PromptFontSize;
        }

        #endregion

        #region Override

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (IsLink)
            {
                //if (hl != null)
                //{
                //    hl.DoClick();
                //    this.PromptForeColor = new SolidColorBrush(Colors.DarkRed);
                //    if (OnNavigate != null) OnNavigate(this.LinkURL);
                //}
            }

            RaiseOnClicked(true);
            base.OnPointerPressed(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            //if (IsLink) this.Cursor = System.Windows.Input.Cursors.Hand;

            base.OnPointerEntered(e);
        }

        #endregion
    }
}
