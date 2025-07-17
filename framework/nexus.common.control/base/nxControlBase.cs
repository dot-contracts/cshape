using Windows.Foundation.Collections;
using Windows.Foundation;
using Windows.UI;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Collections.Generic;
using System.Threading.Channels;
using System;

using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxControlBase : ContentControl, IControl
    {
        private bool   mAccessSet = false;
        private string mActionCode = "";    public string ActionCode {  get { return mActionCode; }  set {  if (!string.IsNullOrEmpty(value)) {  mAccessSet = true; mActionCode = value;  } } }
        private bool   mIsChanged  = false; public bool IsChanged { get { return mIsChanged; } set { mIsChanged = value; } }//  if (OnChange != null)   OnChange("", mIsChanged,mIsChanged); } }

        public string  FunctionId   { get; set; } = string.Empty;
        public string  DataContext  { get; set; } = string.Empty;
        public string  GroupName    { get; set; } = string.Empty;
        public string  Tag          { get; set; } = string.Empty;
        public string  Value        { get; set; } = string.Empty;
        public double  ValueDbl     { get; set; } = 0;
        public bool    Enabled      { get; set; } = true;

        private bool   hasFocus     = false;
        public  bool   SuppressNav  { get; set; } = false;
        public  bool   IsValidated  { get; set; } = false;
        public  bool   IsValidating { get; set; } = false;
        public  bool   IsTouchMode  { get; set; } = false;
        public  bool   IsRequired   { get; set; } = false;
        public  bool   IsEditable   { get; set; } = false;
        public  bool   IsReadOnly   { get; set; } = false;
        public  bool   isHovering = false;

        private bool   _isSelected;
        public  event  EventHandler<bool>? IsSelectedChanged;
        public  bool   IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnIsSelectedChanged();
                }
            }
        }
        protected virtual void OnIsSelectedChanged()
        {
            IsSelectedChanged?.Invoke(this, _isSelected);
        }


        private Border _outerBorder;

        public NxTheme Theme { get; set; }
        public AnimationTypes  AnimationType { get; set; } = AnimationTypes.None;

        public static readonly DependencyProperty ButtonForeProperty        = DependencyProperty.Register(nameof(ButtonFore),         typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ButtonBackProperty        = DependencyProperty.Register(nameof(ButtonBack),         typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ButtonPointerOverProperty = DependencyProperty.Register(nameof(ButtonPointerOver),  typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ButtonPressedProperty     = DependencyProperty.Register(nameof(ButtonPressed),      typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty PromptBackProperty    = DependencyProperty.Register(nameof(PromptBack),    typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty PromptForeProperty    = DependencyProperty.Register(nameof(PromptFore),    typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ReplyBackProperty     = DependencyProperty.Register(nameof(ReplyBack),     typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ReplyForeProperty     = DependencyProperty.Register(nameof(ReplyFore),     typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ReplyRequiredProperty = DependencyProperty.Register(nameof(ReplyRequired), typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ReplyValidProperty    = DependencyProperty.Register(nameof(ReplyValid),    typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ReplyInValidProperty  = DependencyProperty.Register(nameof(ReplyInValid),  typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty MenuForeProperty      = DependencyProperty.Register(nameof(MenuFore),      typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty MenuBackProperty      = DependencyProperty.Register(nameof(MenuBack),      typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty BorderProperty        = DependencyProperty.Register(nameof(Border),        typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty AccentProperty        = DependencyProperty.Register(nameof(Accent),        typeof(Brush), typeof(NxControlBase), new PropertyMetadata(default(Brush)));

        protected Brush ButtonFore        { get => (Brush)GetValue(ButtonForeProperty);        set => SetThemeBrush(ButtonForeProperty,        value); }
        protected Brush ButtonBack        { get => (Brush)GetValue(ButtonBackProperty);        set => SetThemeBrush(ButtonBackProperty,        value); }
        protected Brush ButtonPointerOver { get => (Brush)GetValue(ButtonPointerOverProperty); set => SetThemeBrush(ButtonPointerOverProperty, value); }
        protected Brush ButtonPressed     { get => (Brush)GetValue(ButtonPressedProperty);     set => SetThemeBrush(ButtonPressedProperty,     value); }
        protected Brush PromptBack        { get => (Brush)GetValue(PromptBackProperty);        set => SetThemeBrush(PromptBackProperty, value); }
        protected Brush PromptFore        { get => (Brush)GetValue(PromptForeProperty);        set => SetThemeBrush(PromptForeProperty, value); }
        protected Brush ReplyBack         { get => (Brush)GetValue(ReplyBackProperty);         set => SetThemeBrush(ReplyBackProperty,  value); }
        protected Brush ReplyFore         { get => (Brush)GetValue(ReplyForeProperty);         set => SetThemeBrush(ReplyForeProperty,  value); }
        protected Brush ReplyRequired     { get => (Brush)GetValue(ReplyRequiredProperty);     set => SetThemeBrush(ReplyRequiredProperty, value); }
        protected Brush ReplyValid        { get => (Brush)GetValue(ReplyValidProperty);        set => SetThemeBrush(ReplyValidProperty,    value); }
        protected Brush ReplyInValid      { get => (Brush)GetValue(ReplyInValidProperty);      set => SetThemeBrush(ReplyInValidProperty,  value); }
        protected Brush MenuFore          { get => (Brush)GetValue(MenuForeProperty);          set => SetThemeBrush(MenuForeProperty,   value); }
        protected Brush MenuBack          { get => (Brush)GetValue(MenuBackProperty);          set => SetThemeBrush(MenuBackProperty,   value); }
        protected Brush Border            { get => (Brush)GetValue(BorderProperty);            set => SetThemeBrush(BorderProperty,     value); }
        protected Brush Accent            { get => (Brush)GetValue(AccentProperty);            set => SetThemeBrush(AccentProperty,     value); }

        public event Action<NxTheme>                 OnThemeChange;
        public event EventHandler<object>?           PreviewMouseDown;
        public event EventHandler<string>?           OnProcessKey;
        public event EventHandler<KeyDownEventArgs>? OnProcessKeyDown;
        public event EventHandler<ChangeEventArgs>?  OnChange;
        public event EventHandler<ChangedEventArgs>? OnChanged;
        public event EventHandler<ClickedEventArgs>? OnClicked;

        public void Focus() { }
        public void SetFocus() { }
        public bool Validate() { return false; }
        public bool ProcessKey(string key, bool sendKey) { return false; }

        public NxControlBase()
        {
            ApplyThemeDefaults();
            this.Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //this.PointerEntered += _PointerEntered;
            //this.PointerExited  += _PointerExited;

            _outerBorder = GetTemplateChild("OuterBorder") as Border;

            ApplyThemeDefaults();

            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
       }

        private static readonly Dictionary<DependencyProperty, Func<object>> ThemeMappings = new()
        {
            { ButtonForeProperty,        () => NxThemeManager.Current.ButtonFore.Brush },
            { ButtonBackProperty,        () => NxThemeManager.Current.ButtonBack.Brush },
            { ButtonPointerOverProperty, () => NxThemeManager.Current.ButtonPointerOver.Brush },
            { ButtonPressedProperty,     () => NxThemeManager.Current.ButtonPressed.Brush },
            { PromptBackProperty,        () => NxThemeManager.Current.PromptBack.Brush },
            { PromptForeProperty,        () => NxThemeManager.Current.PromptFore.Brush },
            { ReplyBackProperty,         () => NxThemeManager.Current.ReplyBack.Brush },
            { ReplyForeProperty,         () => NxThemeManager.Current.ReplyFore.Brush },
            { ReplyRequiredProperty,     () => NxThemeManager.Current.ReplyRequired.Brush },
            { ReplyValidProperty,        () => NxThemeManager.Current.ReplyValid.Brush },
            { ReplyInValidProperty,      () => NxThemeManager.Current.ReplyInValid.Brush },
            { MenuForeProperty,          () => NxThemeManager.Current.MenuFore.Brush },
            { MenuBackProperty,          () => NxThemeManager.Current.MenuBack.Brush },
            { BorderProperty,            () => NxThemeManager.Current.Border.Brush },
            { AccentProperty,            () => NxThemeManager.Current.Accent.Brush },
        };

        private void ApplyThemeDefaults()
        {
            Theme = NxThemeManager.Current;
            OnThemeChange?.Invoke(Theme);

            foreach (var mapping in ThemeMappings)
            {
                ApplyThemeIfUnset(mapping.Key, mapping.Value());
            }
        }

        private void ApplyThemeIfUnset(DependencyProperty property, object value)
        {
            if (ReadLocalValue(property) == DependencyProperty.UnsetValue)
            {
                SetValue(property, value);
            }
        }

        private void SetThemeBrush(DependencyProperty property, object value)
        {
            if (value is Brush brush)
            {
                SetValue(property, brush);
            }
            else if (value is Color colorValue)
            {
                SetValue(property, new SolidColorBrush(colorValue));
            }
            else if (value is string colorName)
            {
                try
                {
                    var parsedColor = NxColors.FromName(colorName);
                    SetValue(property, new SolidColorBrush(parsedColor));
                }
                catch
                {
                    SetValue(property, new SolidColorBrush(Colors.Transparent)); // fallback
                }
            }
            else
            {
                SetValue(property, new SolidColorBrush(Colors.Transparent));
            }
        }

        public bool IsAccessible()
        {
            bool IsAcc = true;// (mAccessSet ? nexusUserIdentity.Instance.isActivityAccessible(this.nexusActionCode) : true);
            return IsAcc;
        }

        public void SetVisibility(VisiStates Visible)
        {
            switch (Visible)
            {
                case VisiStates.Visible:   this.Opacity = 1;   this.IsHitTestVisible = true;  this.Visibility = Visibility.Visible;   break;
                case VisiStates.Hidden:    this.Opacity = .25; this.IsHitTestVisible = false; this.Visibility = Visibility.Visible;   break;
                case VisiStates.Collapsed: this.Opacity = 1;   this.IsHitTestVisible = false; this.Visibility = Visibility.Collapsed; break;
                default: break;
            }
        }

        public static WinRect ScreenRect(FrameworkElement element)
        {
            var transform = element.TransformToVisual(null);
            WinPoint topLeft = transform.TransformPoint(new WinPoint(0, 0));

            return new WinRect((double)topLeft.X, (double)topLeft.Y, (double)element.ActualWidth, (double)element.ActualHeight);
        }

        //public System.Drawing.Point GetDialogLocation(Object Form) { return new System.Drawing.Point(); }//   helpers.GetDialogLocation(Form, this); }


        public void RaiseOnClicked(bool Value)
        {
            OnClicked?.Invoke(this, new ClickedEventArgs(Tag?.ToString() ?? Name, Value, ScreenRect(this)));
        }

        public bool RaiseOnChange(string value)
        {
            var args       = new ChangeEventArgs(Tag ?? Name, value, value);

            OnChange?.Invoke(this, args);

            if (args.Valid)
                IsValidated = true;

            return args.Valid;
        }

        public void RaiseOnChanged(string Value)
        {
            OnChanged?.Invoke(this, new ChangedEventArgs((string.IsNullOrEmpty(Tag) ? Name : Tag.ToString()), Value, Value, ScreenRect(this)));
        }


        protected virtual Brush GetDefaultBackgroundBrush()
        {
            if (this is NxText || this is NxTextEdit || this is NxCombo || this is NxComboEdit )  return Theme.ReplyBack.Brush;
            if (this is NxButton)                                                                 return Theme.Accent.Brush;

            return Theme.PanelBack.Brush; // default fallback
        }

        public virtual void AnimateBackground(Windows.UI.Color toColor, TimeSpan duration)
        {
            AnimateBrush(this, Control.BackgroundProperty, toColor, duration);
        }

        // the shared implementation
        protected void AnimateBrush(DependencyObject target, DependencyProperty brushDp, Windows.UI.Color toColor, TimeSpan duration)
        {
            // get or create a SolidColorBrush on the DP
            var existing = target.GetValue(brushDp) as SolidColorBrush;
            var brush = existing ?? new SolidColorBrush(Colors.Transparent);
            if (existing == null)
                target.SetValue(brushDp, brush);

            // build the color animation
            var animation = new ColorAnimation
            {
                To = toColor,
                Duration = new Duration(duration),
                EnableDependentAnimation = true
            };

            // target the brush itself
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, "(Border.Background).(SolidColorBrush.Color)");

            var sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }


        #region IDisposable Implementation
        // Track whether Dispose has been called.
        private bool disposed = false;

        // Called once per instance to dispose of resources.
        // A derived class must not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Actual method that performs the resource disposal
        // Each derived class is responsible for its pecular disposal requirements
        // Don't forget to then call the class base dispose method
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                try
                {
                    // If disposing equals true, dispose all managed resources
                    if (disposing)
                    {
                        // Dispose managed resources
                        // TODO: Do the disposal
                    }

                    // Dispose unmanaged resources
                    // TODO: Do the disposal
                }
                finally
                {
                    // Note disposing has been done.
                    disposed = true;
                    // base.Dispose( disposing );  // use for derived classes
                }
            }
        }

        // Use C# destructor for finalization code.
        // This destructor will run only if the Dispose method does not get called.
        ~NxControlBase()
        {
            Dispose(false);      // Dispose of unmanaged resources
        }
        #endregion


    }
}
