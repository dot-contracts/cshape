
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using nexus.common.control.Themes;
using System;
using Windows.Foundation;
using Windows.UI;

namespace nexus.common.control
{
    public partial class NxButton : NxControlBase
    {
        private int    ProgMax       = 100;
        private int    ProgValue     = 1;

        private Border      _outerBorder;
        private NxImage     _image;
        private TextBlock   _label;
        private ProgressBar _progBar;
        private Rectangle   _underscore;

        #region Dependency Properties

        public static readonly DependencyProperty ButtonTypeProperty                 = DependencyProperty.Register(nameof(ButtonType),                 typeof(ButtonTypes),         typeof(NxButton), new PropertyMetadata(ButtonTypes.Normal,         OnVisualPropertyChanged));
        public static readonly DependencyProperty BorderTypeProperty                 = DependencyProperty.Register(nameof(BorderType),                 typeof(BorderTypes),         typeof(NxButton), new PropertyMetadata(BorderTypes.Normal,         OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureProperty                    = DependencyProperty.Register(nameof(Picture),                    typeof(NxImage.Pictures),    typeof(NxButton), new PropertyMetadata(NxImage.Pictures.tick,      OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureVisibleProperty             = DependencyProperty.Register(nameof(PictureVisible),             typeof(bool),                typeof(NxButton), new PropertyMetadata(false,                      OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureWidthProperty               = DependencyProperty.Register(nameof(PictureWidth),               typeof(double),              typeof(NxButton), new PropertyMetadata(25.0,                       OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureHeightProperty              = DependencyProperty.Register(nameof(PictureHeight),              typeof(double),              typeof(NxButton), new PropertyMetadata(25.0,                       OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureHorizontalAlignmentProperty = DependencyProperty.Register(nameof(PictureHorizontalAlignment), typeof(HorizontalAlignment), typeof(NxButton), new PropertyMetadata(HorizontalAlignment.Center, OnVisualPropertyChanged));
        public static readonly DependencyProperty PictureVerticalAlignmentProperty   = DependencyProperty.Register(nameof(PictureVerticalAlignment),   typeof(VerticalAlignment),   typeof(NxButton), new PropertyMetadata(VerticalAlignment.Center,   OnVisualPropertyChanged));

        public static readonly DependencyProperty PromptProperty                     = DependencyProperty.Register(nameof(Prompt),                     typeof(string),              typeof(NxButton), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty PromptWidthProperty                = DependencyProperty.Register(nameof(PromptWidth),                typeof(double),              typeof(NxButton), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty PromptFontFamilyProperty           = DependencyProperty.Register(nameof(PromptFontFamily),           typeof(FontFamily),          typeof(NxButton), new PropertyMetadata(default(FontFamily)));
        public static readonly DependencyProperty PromptFontSizeProperty             = DependencyProperty.Register(nameof(PromptFontSize),             typeof(double),              typeof(NxButton), new PropertyMetadata(12.0));
        public static readonly DependencyProperty PromptMarginProperty               = DependencyProperty.Register(nameof(PromptMargin),               typeof(Thickness),           typeof(NxButton), new PropertyMetadata(default(Thickness)));
        public static readonly DependencyProperty MaxPromptWidthProperty             = DependencyProperty.Register(nameof(MaxPromptWidth),             typeof(double),              typeof(NxButton), new PropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty PromptHorizontalAlignmentProperty  = DependencyProperty.Register(nameof(PromptHorizontalAlignment),  typeof(HorizontalAlignment), typeof(NxButton), new PropertyMetadata(HorizontalAlignment.Left));
        public static readonly DependencyProperty PromptVerticalAlignmentProperty    = DependencyProperty.Register(nameof(PromptVerticalAlignment),    typeof(VerticalAlignment),   typeof(NxButton), new PropertyMetadata(VerticalAlignment.Center));
        public static readonly DependencyProperty PromptTextWrappingProperty         = DependencyProperty.Register(nameof(PromptTextWrapping),         typeof(TextWrapping),        typeof(NxButton), new PropertyMetadata(TextWrapping.NoWrap));
        public static readonly DependencyProperty PromptTextAutofitProperty          = DependencyProperty.Register(nameof(PromptTextAutofit),          typeof(bool),                typeof(NxButton), new PropertyMetadata(false));
        public static readonly DependencyProperty PromptMaxFontSizeProperty          = DependencyProperty.Register(nameof(PromptMaxFontSize),          typeof(double),              typeof(NxButton), new PropertyMetadata(0d));

        public static readonly DependencyProperty ProgressProperty                   = DependencyProperty.Register(nameof(Progress),                   typeof(double),              typeof(NxButton), new PropertyMetadata(0d));
        public static readonly DependencyProperty IsCountdownActiveProperty          = DependencyProperty.Register(nameof(IsCountdownActive),          typeof(bool),                typeof(NxButton), new PropertyMetadata(false));

        #endregion

        #region CLR Wrappers

        public ButtonTypes         ButtonType                 { get => (ButtonTypes)         GetValue(ButtonTypeProperty);                 set => SetValue(ButtonTypeProperty, value); }
        public BorderTypes         BorderType                 { get => (BorderTypes)         GetValue(BorderTypeProperty);                 set => SetValue(BorderTypeProperty, value); }
        public NxImage.Pictures    Picture                    { get => (NxImage.Pictures)    GetValue(PictureProperty);                    set => SetValue(PictureProperty, value); }
        public bool                PictureVisible             { get => (bool)                GetValue(PictureVisibleProperty);             set => SetValue(PictureVisibleProperty, value); }
        public double              PictureWidth               { get => (double)              GetValue(PictureWidthProperty);               set => SetValue(PictureWidthProperty, value); }
        public double              PictureHeight              { get => (double)              GetValue(PictureHeightProperty);              set => SetValue(PictureHeightProperty, value); }
        public HorizontalAlignment PictureHorizontalAlignment { get => (HorizontalAlignment) GetValue(PictureHorizontalAlignmentProperty); set => SetValue(PictureHorizontalAlignmentProperty, value); }
        public VerticalAlignment   PictureVerticalAlignment   { get => (VerticalAlignment)   GetValue(PictureVerticalAlignmentProperty);   set => SetValue(PictureVerticalAlignmentProperty, value); }

        public string              Prompt                     { get => (string)              GetValue(PromptProperty);                     set => SetValue(PromptProperty, value); }
        public double              PromptWidth                { get => (double)              GetValue(PromptWidthProperty);                set => SetValue(PromptWidthProperty, value); }
        public FontFamily          PromptFontFamily           { get => (FontFamily)          GetValue(PromptFontFamilyProperty);           set => SetValue(PromptFontFamilyProperty, value); }
        public double              PromptFontSize             { get => (double)              GetValue(PromptFontSizeProperty);             set => SetValue(PromptFontSizeProperty, value); }
        public Thickness           PromptMargin               { get => (Thickness)           GetValue(PromptMarginProperty);               set => SetValue(PromptMarginProperty, value); }
        public double              MaxPromptWidth             { get => (double)              GetValue(MaxPromptWidthProperty);             set => SetValue(MaxPromptWidthProperty, value); }
        public HorizontalAlignment PromptHorizontalAlignment  { get => (HorizontalAlignment) GetValue(PromptHorizontalAlignmentProperty);  set => SetValue(PromptHorizontalAlignmentProperty, value); }
        public VerticalAlignment   PromptVerticalAlignment    { get => (VerticalAlignment)   GetValue(PromptVerticalAlignmentProperty);    set => SetValue(PromptVerticalAlignmentProperty, value); }
        public TextWrapping        PromptTextWrapping         { get => (TextWrapping)        GetValue(PromptTextWrappingProperty);         set => SetValue(PromptTextWrappingProperty, value); }
        public bool                PromptTextAutofit          { get => (bool)                GetValue(PromptTextAutofitProperty);          set => SetValue(PromptTextAutofitProperty, value); }
        public double              PromptMaxFontSize          { get => (double)              GetValue(PromptMaxFontSizeProperty);          set => SetValue(PromptMaxFontSizeProperty, value); }

        public double              Progress                   { get => (double)              GetValue(ProgressProperty);                   set => SetValue(ProgressProperty, value); }
        public bool                IsCountdownActive          { get => (bool)                GetValue(IsCountdownActiveProperty);          set => SetValue(IsCountdownActiveProperty, value); }

        #endregion


        public NxButton()
        {
            DefaultStyleKey = typeof(NxButton);
            //PointerPressed  += NxButton_Click;
            SizeChanged     += (s, e) => SetVisualState();

            base.IsSelectedChanged += SelectedChanged;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _outerBorder = GetTemplateChild("OuterBorder")  as Border;
            _image       = GetTemplateChild("Image")        as NxImage;
            _label       = GetTemplateChild("Label")        as TextBlock;
            _progBar     = GetTemplateChild("ProgBar")      as ProgressBar;
            _underscore  = GetTemplateChild("UnderScore")   as Rectangle;

            if (_outerBorder != null)
            {
                PointerEntered  += _outerBorder_PointerEntered;
                PointerExited   += _outerBorder_PointerExited;
                PointerPressed  += _outerBorder_PointerPressed;
                PointerReleased += _outerBorder_PointerReleased;

                SetSelected(IsSelected);
            }

            SetVisualState();
            ApplyThemeDefaults();
        }

        private void ApplyThemeDefaults()
        {
            if (_outerBorder != null)
            {
                _label.Foreground            = NxThemeManager.Current.GetThemeBrush("NxButtonFore", Colors.Black);
                _outerBorder.BorderBrush     = NxThemeManager.Current.GetThemeBrush("NxButtonBack", Colors.White);
                _outerBorder.Background      = NxThemeManager.Current.GetThemeBrush("NxButtonBack", Colors.White);
                _outerBorder.BorderThickness = new Thickness(1.0);
            }

            if (_progBar != null)
            {
                _progBar.Background = NxThemeManager.Current.GetThemeBrush("NxProgressTrack", Colors.Black);
                _progBar.Foreground = NxThemeManager.Current.GetThemeBrush("NxProgressBar",   Colors.Black);
            }
        }

        private void _outerBorder_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (isHovering)
            {
                var pointerPos = e.GetCurrentPoint(_outerBorder).Position;
                var withinBounds = pointerPos.X >= 0 && pointerPos.X <= _outerBorder.ActualWidth &&
                                   pointerPos.Y >= 0 && pointerPos.Y <= _outerBorder.ActualHeight;

                Windows.UI.Color color = withinBounds  ?  NxThemeManager.Current.GetThemeColor("NxButtonPointerOver", Colors.LightGray)
                                                       :  NxThemeManager.Current.GetThemeColor("NxButtonBack",        Colors.White);

                AnimateBackground(color, TimeSpan.FromMilliseconds(200));

                RaiseOnClicked(true);
                isHovering = false;
            }
        }

        private void _outerBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Color color = NxThemeManager.Current.GetThemeColor("NxButtonPointerOver", Colors.LightGray);
            AnimateBackground(color, TimeSpan.FromMilliseconds(200));
        }

        private void _outerBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Color color = NxThemeManager.Current.GetThemeColor("NxButtonPressed", Colors.Gainsboro);
            AnimateBackground(color, TimeSpan.FromMilliseconds(200));
            isHovering = true;
        }

        private void _outerBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Color color = NxThemeManager.Current.GetThemeColor("NxButtonBack", Colors.White);
            AnimateBackground(color, TimeSpan.FromMilliseconds(200));

            _outerBorder.BorderThickness = IsSelected ? new Thickness(2.0) : new Thickness(1.0);
            _outerBorder.BorderBrush     = IsSelected ? NxThemeManager.Current.GetThemeColor("NxButtonPressed", Colors.LightGray)
                                                      : NxThemeManager.Current.GetThemeColor("NxButtonBack", Colors.White);

        }

        public override void AnimateBackground(Windows.UI.Color toColor, TimeSpan duration) => AnimateBrush(_outerBorder, Control.BackgroundProperty, toColor, duration);

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NxButton)d).SetVisualState();
        }

        private void SetVisualState()
        {
            if (_image != null)
            {
                _image.Picture = Picture;
                _image.Width   = PictureWidth;
                _image.Height  = PictureHeight;

                if (PictureVisible)
                {
                    double totalWidth  = ActualWidth         > 0 ? ActualWidth : Width;
                    double totalHeight = ActualHeight        > 0 ? ActualHeight : Height;
                    double iconWidth   = _image.ActualWidth  > 0 ? _image.ActualWidth  : _image.Width;
                    double iconHeight  = _image.ActualHeight > 0 ? _image.ActualHeight : _image.Height;

                    var margin = new Thickness();

                    switch (PictureHorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:    margin.Left = 0;                              break;
                        case HorizontalAlignment.Center:  margin.Left = (totalWidth - iconWidth) / 2;   break;
                        case HorizontalAlignment.Right:   margin.Left = totalWidth - iconWidth;         break;
                        case HorizontalAlignment.Stretch: margin.Left = 0;                              break;
                    }

                    switch (PictureVerticalAlignment)
                    {
                        case VerticalAlignment.Top:       margin.Top = 0;                               break;
                        case VerticalAlignment.Center:    margin.Top = (totalHeight - iconHeight) / 2; break;
                        case VerticalAlignment.Bottom:    margin.Top = totalHeight - iconHeight;        break;
                        case VerticalAlignment.Stretch:   margin.Top = 0;                               break;
                    }

                    margin.Left = Math.Max(0, margin.Left);
                    margin.Top  = Math.Max(0, margin.Top);

                    _image.Margin              = margin;
                    _image.HorizontalAlignment = HorizontalAlignment.Left;
                    _image.VerticalAlignment   = VerticalAlignment.Top;
                    _image.Visibility          = Visibility.Visible;
                }
                else
                {
                    _image.Visibility = Visibility.Collapsed;
                }
            }

            if (_outerBorder != null)
            {
                _outerBorder.CornerRadius = ButtonType == ButtonTypes.Tab ? new CornerRadius(0, 10, 0, 0) : new CornerRadius(5, 5, 5, 5);

                Brush brush = BorderType switch
                {
                    BorderTypes.OK     or BorderTypes.Save   => new SolidColorBrush(Colors.Green),
                    BorderTypes.Cancel or BorderTypes.Delete => new SolidColorBrush(Colors.Red),
                    BorderTypes.Edit                         => new SolidColorBrush(Colors.Blue),
                    BorderTypes.New                          => new SolidColorBrush(Colors.SeaGreen),
                    _                                        => Application.Current.Resources["NxBorder"] as Brush ?? new SolidColorBrush(Colors.Black)
                };

                _outerBorder.BorderBrush = brush;
            }

            VisualStateManager.GoToState(this, IsCountdownActive ? "CountdownActive" : "CountdownInactive", true);

            if (_progBar != null)
                HideProgress();
        }

        public void ShowProgress(int maximum)
        {
            ProgMax = maximum;
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_progBar != null)
                {
                    _progBar.Value      = 0;
                    _progBar.Maximum    = ProgMax;
                    _progBar.Visibility = Visibility.Visible;
                }
            });
        }

        public void DoProgress(int value)
        {
            ProgValue = value;
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_progBar != null)
                {
                    if (ProgValue > ProgMax)
                    {
                        ProgMax          = ProgValue + 10;
                        _progBar.Maximum = ProgMax;
                    }
                    _progBar.Value = ProgValue;
                }
            });
        }

        public void HideProgress()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_progBar != null)
                    _progBar.Visibility = Visibility.Collapsed;
            });
        }

        private void SelectedChanged(object? sender, bool e)
        {
            if (sender is NxButton btn)
            {
                SetSelected(btn.IsSelected);
            }
        }

        public void SetSelected(bool Selected)
        {
            IsSelected = Selected;

            VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Normal", true);

            if (_outerBorder == null) return;

            _outerBorder.BorderThickness = IsSelected ? new Thickness(2.0) : new Thickness(1.0);
            _outerBorder.BorderBrush     = IsSelected ? NxThemeManager.Current.GetThemeColor("NxButtonPressed", Colors.LightGray)
                                                      : NxThemeManager.Current.GetThemeColor("NxButtonBack", Colors.White);
        }

    }
}
