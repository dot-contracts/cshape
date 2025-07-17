using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public sealed partial class NxCheck : NxControlBase
    {
        private int isEnterKey = 0;

        public static readonly DependencyProperty PromptProperty                     = DependencyProperty.Register(nameof(Prompt),                     typeof(string),              typeof(NxCheck), new PropertyMetadata(null));
        public static readonly DependencyProperty PromptWidthProperty                = DependencyProperty.Register(nameof(PromptWidth),                typeof(double),              typeof(NxCheck), new PropertyMetadata(40.0));
        public static readonly DependencyProperty PictureTrueProperty                = DependencyProperty.Register(nameof(PictureTrue),                typeof(NxImage.Pictures),    typeof(NxCheck), new PropertyMetadata(NxImage.Pictures.tick,     OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureFalseProperty               = DependencyProperty.Register(nameof(PictureFalse),               typeof(NxImage.Pictures),    typeof(NxCheck), new PropertyMetadata(NxImage.Pictures.cross,    OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureWidthProperty               = DependencyProperty.Register(nameof(PictureWidth),               typeof(double),              typeof(NxCheck), new PropertyMetadata(25.0,                      OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureHeightProperty              = DependencyProperty.Register(nameof(PictureHeight),              typeof(double),              typeof(NxCheck), new PropertyMetadata(25.0,                      OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureHorizontalAlignmentProperty = DependencyProperty.Register(nameof(PictureHorizontalAlignment), typeof(HorizontalAlignment), typeof(NxCheck), new PropertyMetadata(HorizontalAlignment.Right, OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureVerticalAlignmentProperty   = DependencyProperty.Register(nameof(PictureVerticalAlignment),   typeof(VerticalAlignment),   typeof(NxCheck), new PropertyMetadata(VerticalAlignment.Bottom,  OnVisualPropertyChanged));
        public static readonly DependencyProperty PromptHorizontalAlignmentProperty  = DependencyProperty.Register(nameof(PromptHorizontalAlignment),  typeof(HorizontalAlignment), typeof(NxCheck), new PropertyMetadata(HorizontalAlignment.Left,  OnVisualPropertyChanged));
        public static readonly DependencyProperty PromptVerticalAlignmentProperty    = DependencyProperty.Register(nameof(PromptVerticalAlignment),    typeof(VerticalAlignment),   typeof(NxCheck), new PropertyMetadata(VerticalAlignment.Top,     OnVisualPropertyChanged));
        public static readonly DependencyProperty PromptFontFamilyProperty           = DependencyProperty.Register(nameof(PromptFontFamily),           typeof(FontFamily),          typeof(NxCheck), new PropertyMetadata(new FontFamily("Arial")));
        public static readonly DependencyProperty PromptFontSizeProperty             = DependencyProperty.Register(nameof(PromptFontSize),             typeof(double),              typeof(NxCheck), new PropertyMetadata(12.0));
        public static readonly DependencyProperty IsReadOnlyProperty                 = DependencyProperty.Register(nameof(IsReadOnly),                 typeof(bool),                typeof(NxCheck), new PropertyMetadata(false));

        public string              Prompt                     { get => (string)             GetValue(PromptProperty);                     set => SetValue(PromptProperty, value); }
        public double              PromptWidth                { get => (double)             GetValue(PromptWidthProperty);                set => SetValue(PromptWidthProperty, value); } 
        public NxImage.Pictures    PictureTrue                { get => (NxImage.Pictures)   GetValue(PictureTrueProperty);                set => SetValue(PictureTrueProperty, value); }
        public NxImage.Pictures    PictureFalse               { get => (NxImage.Pictures)   GetValue(PictureFalseProperty);               set => SetValue(PictureFalseProperty, value); }
        public double              PictureWidth               { get => (double)             GetValue(PictureWidthProperty);               set => SetValue(PictureWidthProperty, value); }
        public double              PictureHeight              { get => (double)             GetValue(PictureHeightProperty);              set => SetValue(PictureHeightProperty, value); }
        public HorizontalAlignment PromptHorizontalAlignment  { get => (HorizontalAlignment)GetValue(PromptHorizontalAlignmentProperty);  set => SetValue(PromptHorizontalAlignmentProperty, value); }
        public VerticalAlignment   PromptVerticalAlignment    { get => (VerticalAlignment)  GetValue(PromptVerticalAlignmentProperty);    set => SetValue(PromptVerticalAlignmentProperty, value); }
        public FontFamily          PromptFontFamily           { get => (FontFamily)         GetValue(PromptFontFamilyProperty);           set => SetValue(PromptFontFamilyProperty, value); }
        public double              PromptFontSize             { get => (double)             GetValue(PromptFontSizeProperty);             set => SetValue(PromptFontSizeProperty, value); }
        public VerticalAlignment   PictureVerticalAlignment   { get => (VerticalAlignment)  GetValue(PictureVerticalAlignmentProperty);   set => SetValue(PictureVerticalAlignmentProperty, value); }
        public HorizontalAlignment PictureHorizontalAlignment { get => (HorizontalAlignment)GetValue(PictureHorizontalAlignmentProperty); set => SetValue(PictureHorizontalAlignmentProperty, value); }
        public bool                IsReadOnly                 { get => (bool)               GetValue(IsReadOnlyProperty);                 set => SetValue(IsReadOnlyProperty, value); }

        public Brush PromptBackColor { get => PromptBack; set => PromptBack = value; }
        public Brush PromptForeColor { get => PromptFore; set => PromptFore = value; }

        protected NxImage Button { get { return base.GetTemplateChild("button") as NxImage; } }
        protected NxLabel Label  { get { return base.GetTemplateChild("label")  as NxLabel; } }


        public NxCheck()
        {
            this.DefaultStyleKey = typeof(NxCheck);
            this.IsTabStop = true;
            this.KeyDown += NxCheck_KeyDown;
            this.PointerPressed += NxCheck_Click;
            this.SizeChanged += (s, e) => UpdatePicture();

            this.IsSelectedChanged += (s, selected) =>
            {
                UpdatePicture();
            };

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        private void ApplyThemeDefaults()
        {
            //this.PromptBackColor = new SolidColorBrush(NxThemeManager.Current.GetThemeColor("NxPromptBack", Colors.Black));
            this.PromptForeColor = new SolidColorBrush(NxThemeManager.Current.GetThemeColor("NxPromptFore", Colors.White));
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void NxCheck_Click(object sender, PointerRoutedEventArgs e)
        {
            ToggleChecked();
        }

        private void NxCheck_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Space:
                    ToggleChecked();
                    break;
                case VirtualKey.Enter:
                    isEnterKey++;
                    if (isEnterKey > 1)
                    {
                        isEnterKey = 0;
                    }
                    //OnClicked.Invoke(Tag?.ToString() ?? Name, helpers.ScreenRect(this));
                    break;
            }
        }

        public void SetChecked(bool check)
        {
            IsSelected = check;
            UpdatePicture();
        }

        public void ToggleChecked()
        {
            SetChecked(!IsSelected);
            bool valid = true;

            RaiseOnChanged(IsSelected.ToString());
            RaiseOnChange(IsSelected.ToString());
            RaiseOnClicked(IsSelected);
        }

        private void UpdatePicture()
        {
            if (Button != null)
            {
                Button.ImageWidth = PictureWidth;
                Button.ImageHeight = PictureHeight;
                Button.Picture = IsSelected ? PictureTrue : PictureFalse;
            }
            if (Label != null)
            {
                Label.Height = ActualHeight;
                Label.Width  = PromptWidth;
            }
        }

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NxCheck)d).SetVisualState();
        }

        private void SetVisualState()
        {


        }
    }
}
