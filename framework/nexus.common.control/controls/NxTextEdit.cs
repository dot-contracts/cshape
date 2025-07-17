
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using nexus.common;
using nexus.common.control;
using nexus.common.control.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.System;

namespace nexus.common.control

{
    public partial class NxTextEdit : NxControlBase, IControl
    {
        private        bool isTouchMode  = false; public bool IsTouchMode   { get { return isTouchMode; }   set { isTouchMode  =  value; } }
        private static bool isChanged    = false; public bool IsChanged     { get { return isChanged; }     set { isChanged    =  value; } }

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxTextEdit), new PropertyMetadata(false));

        public     string                Prompt                     { get { return (string)                 GetValue(PromptProperty); }                    set { SetValue(PromptProperty,                    value); } }
        public     double                PromptWidth                { get { return (double)                 GetValue(PromptWidthProperty); }               set { SetValue(PromptWidthProperty,               value); } }
        public     FontFamily            PromptFontFamily           { get { return (FontFamily)             GetValue(PromptFontFamilyProperty); }          set { SetValue(PromptFontFamilyProperty,          value); } }
        public     double                PromptFontSize             { get { return (double)                 GetValue(PromptFontSizeProperty); }            set { SetValue(PromptFontSizeProperty,            value); } }
        public     Thickness             PromptMargin               { get { return (Thickness)              GetValue(PromptMarginProperty); }              set { SetValue(PromptMarginProperty,              value); } }
        public     double                MaxPromptWidth             { get { return (double)                 GetValue(MaxPromptWidthProperty); }            set { SetValue(MaxPromptWidthProperty,            value); } }
        public     HorizontalAlignment   PromptHorizontalAlignment  { get { return (HorizontalAlignment)    GetValue(PromptHorizontalAlignmentProperty); } set { SetValue(PromptHorizontalAlignmentProperty, value); } }
        public     VerticalAlignment     PromptVerticalAlignment    { get { return (VerticalAlignment)      GetValue(PromptVerticalAlignmentProperty); }   set { SetValue(PromptVerticalAlignmentProperty,   value); } }
        public     TextWrapping          PromptTextWrapping         { get { return (TextWrapping)           GetValue(PromptTextWrappingProperty); }        set { SetValue(PromptTextWrappingProperty,        value); } }
        #endregion

        #region Editor Dependency Property
        public static readonly new DependencyProperty IsReadOnlyProperty               = DependencyProperty.Register("IsReadOnly",               typeof(bool),                    typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty IsRequiredProperty               = DependencyProperty.Register("IsRequired",               typeof(bool),                    typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty SuppressNavigateProperty         = DependencyProperty.Register("SuppressNavigate",         typeof(bool),                    typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyFontFamilyProperty          = DependencyProperty.Register("ReplyFontFamily",          typeof(FontFamily),              typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyFontSizeProperty            = DependencyProperty.Register("ReplyFontSize",            typeof(double),                  typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyRequiredTextProperty        = DependencyProperty.Register("ReplyRequiredText",        typeof(string),                  typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyInvalidPictureProperty      = DependencyProperty.Register("ReplyInvalidPicture",      typeof(NxImage.Pictures),        typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyValidPictureProperty        = DependencyProperty.Register("ReplyValidPicture",        typeof(NxImage.Pictures),        typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty MaxTextLengthProperty            = DependencyProperty.Register("MaxTextLength",            typeof(int),                     typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty MaskTypeProperty                 = DependencyProperty.Register("MaskType",                 typeof(MaskTypes),               typeof(NxTextEdit), new PropertyMetadata(false)); 
        public static readonly     DependencyProperty MaskProperty                     = DependencyProperty.Register("Mask",                     typeof(string),                  typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ChangeEventTypeProperty          = DependencyProperty.Register("ChangeEventType",          typeof(NxText.ChangeEventTypes), typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyHorizontalAlignmentProperty = DependencyProperty.Register("ReplyHorizontalAlignment", typeof(HorizontalAlignment),     typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyVerticalAlignmentProperty   = DependencyProperty.Register("ReplyVerticalAlignment",   typeof(VerticalAlignment),       typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyTextWrappingProperty        = DependencyProperty.Register("ReplyTextWrapping",        typeof(TextWrapping),            typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyTextAutofitProperty         = DependencyProperty.Register("ReplyTextAutofit",         typeof(bool),                    typeof(NxTextEdit), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyMaxFontSizeProperty         = DependencyProperty.Register("ReplyMaxFontSize",         typeof(double),                  typeof(NxTextEdit), new PropertyMetadata(false));


        public new  bool                    IsReadOnly               { get { return (bool)                    GetValue(IsReadOnlyProperty); }               set { SetValue(IsReadOnlyProperty,              value); SetControlStyle(); } }
        public      bool                    IsRequired               { get { return (bool)                    GetValue(IsRequiredProperty); }               set { SetValue(IsRequiredProperty,               value); } }
        public      bool                    SuppressNavigate         { get { return (bool)                    GetValue(SuppressNavigateProperty); }         set { SetValue(SuppressNavigateProperty,         value); } }
        public      FontFamily              ReplyFontFamily          { get { return (FontFamily)              GetValue(ReplyFontFamilyProperty); }          set { SetValue(ReplyFontFamilyProperty,          value); } }
        public      double                  ReplyFontSize            { get { return (double)                  GetValue(ReplyFontSizeProperty); }            set { SetValue(ReplyFontSizeProperty,            value); } }
        public      string                  ReplyRequiredText        { get { return (string)                  GetValue(ReplyRequiredTextProperty); }        set { SetValue(ReplyRequiredTextProperty,        value); } }
        public      NxImage.Pictures        ReplyInvalidPicture      { get { return (NxImage.Pictures)        GetValue(ReplyInvalidPictureProperty);}       set { SetValue(ReplyInvalidPictureProperty,      value); } }
        public      NxImage.Pictures        ReplyValidPicture        { get { return (NxImage.Pictures)        GetValue(ReplyValidPictureProperty);}         set { SetValue(ReplyValidPictureProperty,        value); } }
        public      int                     MaxTextLength            { get { return (int)                     GetValue(MaxTextLengthProperty); }            set { SetValue(MaxTextLengthProperty,            value); } }
        public      bool                    ReplyTextAutofit         { get { return (bool)                    GetValue(ReplyTextAutofitProperty); }         set { SetValue(ReplyTextAutofitProperty,         value); } }
        public      double                  ReplyMaxFontSize         { get { return (double)                  GetValue(ReplyMaxFontSizeProperty); }         set { SetValue(ReplyMaxFontSizeProperty,         value); } }

        public      MaskTypes               MaskType                 

        {
            get
            {
                return (MaskTypes)GetValue(MaskTypeProperty);
            }
            set 
            { 
                SetValue(MaskTypeProperty, value);
                if (Editor != null)
                {
                    Editor.MaskType = value;
                }
            }
        }
        public      string                  Mask                     { get { return (string)                  GetValue(MaskProperty); }                     set { SetValue(MaskProperty,                     value); } }
        public      NxText.ChangeEventTypes ChangeEventType          { get { return (NxText.ChangeEventTypes) GetValue(ChangeEventTypeProperty); }          set { SetValue(ChangeEventTypeProperty,          value); } }
        public      HorizontalAlignment     ReplyHorizontalAlignment { get { return (HorizontalAlignment)     GetValue(ReplyHorizontalAlignmentProperty); } set { SetValue(ReplyHorizontalAlignmentProperty, value); } }
        public      VerticalAlignment       ReplyVerticalAlignment   { get { return (VerticalAlignment)       GetValue(ReplyVerticalAlignmentProperty); }   set { SetValue(ReplyVerticalAlignmentProperty,   value); } }
        public      TextWrapping            ReplyTextWrapping        { get { return (TextWrapping)            GetValue(ReplyTextWrappingProperty); }        set { SetValue(ReplyTextWrappingProperty,        value); } }

        public Brush PromptBackColor       { get => PromptBack;   set => PromptBack = value; }
        public Brush PromptForeColor       { get => PromptFore;   set => PromptFore = value; }
        public Brush ReplyBackColor        { get => ReplyBack;    set => ReplyBack  = value; }
        public Brush ReplyForeColor        { get => ReplyFore;    set => ReplyFore  = value; }
        public Brush ReplyInvalidBackColor { get => ReplyInValid; set => ReplyInValid = value; }
        public Brush ReplyValidBackColor   { get => ReplyValid;   set => ReplyValid = value; }

        #endregion

        #region Value Property
        private string mValue = ""; public string Value { get { if (Editor != null) mValue = Editor.Value; return mValue; } set { mValue = value; if (Editor != null) Editor.Value = mValue; } }
        public  int    ValueInt { get { int    ret = 0; if (Editor != null) ret = Editor.ValueInt; return ret; } }
        public  double ValueDbl { get { double ret = 0; if (Editor != null) ret = Editor.ValueDbl; return ret; } }

        #endregion

        #region Event
        public delegate void OnChangedControlEventHandler(NxTextEdit Ctl,string Tag, string Display, string Value, ref bool Valid); public event OnChangedControlEventHandler OnChangedControl;

        public delegate void OnProcessKeyDownEventHandler (VirtualKey Key, ref bool Handled);                   public event OnProcessKeyDownEventHandler OnProcessKeyDown;
        public delegate void OnProcessKeyUpEventHandler   (VirtualKey Key);                                     public event OnProcessKeyUpEventHandler   OnProcessKeyUp;
        public delegate void OnProcessKeyEventHandler     (string Key);                                         public event OnProcessKeyEventHandler     OnProcessKey;
        public delegate void OnInValidEventHandler        (string errorMsg);                                    public event OnInValidEventHandler        OnInValid;
        public delegate void OnDispose                    ();
        #endregion

        #region Delegate
        public delegate void ShowProgressCB(int Maximum);
        public delegate void DoProgressCB(int Value);
        public delegate void HideProgressCB();
        #endregion

        #region Property
        protected NxText      Editor  { get { return base.GetTemplateChild("editor")  as NxText; } }
        protected NxLabel     Label   { get { return base.GetTemplateChild("label")   as NxLabel; } }
        protected ProgressBar ProgBar { get { return base.GetTemplateChild("ProgBar") as ProgressBar; } }
        protected Border      border  { get { return base.GetTemplateChild("border")  as Border; } }
        #endregion
        public void Focus() { }

        public NxTextEdit()
        {
            DefaultStyleKey = typeof(NxTextEdit);

            if (Tag == null) Tag = string.Empty;

            this.IsTabStop = false;
            //this.Focusable = false;

            this.Loaded      += NxTextEdit_Loaded;
            base.SizeChanged += NxTextEdit_SizeChanged;

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }


        #region Override Methods

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Editor != null) 
            {
                Editor.ReplyRequiredText = ReplyRequiredText;
                Editor.Value             = mValue;
                if (!MaskType.Equals(MaskTypes.None)) Editor.MaskType = MaskType;
                Editor.ReplyMaxFontSize = ReplyMaxFontSize;
                Editor.ReplyTextAutofit = ReplyTextAutofit;

                Editor.OnChanged       += Editor_OnChanged;
                Editor.PreviewKeyUp    += Editor_PreviewKeyUp;
                Editor.PreviewKeyDown  += Editor_PreviewKeyDown;
                Editor.OnProcessKey    += Editor_OnProcessKey;
            }
            Resize();
            ApplyThemeDefaults();


            int hh = GetHashCode();

        }
        private void ApplyThemeDefaults()
        {
            if (Theme != null)
            {
                ReplyForeColor = NxThemeManager.Current.GetThemeBrush("NxReplyFore", Colors.White);
                ReplyBackColor = NxThemeManager.Current.GetThemeBrush("NxReplyBack", Colors.Black);
            }
        }

        private void Editor_OnProcessKey(string Key)                            {
            int hh = GetHashCode();
            OnProcessKey?.Invoke(Key);     }
        private void Editor_PreviewKeyUp(object sender, KeyRoutedEventArgs e)   { OnProcessKeyUp?.Invoke(e.Key); }
        private void Editor_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        { 
            bool Handled = false;
            
            OnProcessKeyDown?.Invoke(e.Key,ref Handled);
            e.Handled = Handled;
        }

        void NxTextEdit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Editor != null)
            {
                Editor.Height = ActualHeight-15;
                if (ActualWidth>0 && ActualWidth > PromptWidth) Editor.Width  = (ActualWidth - PromptWidth);
            }
            if (Label != null)
            {
                Label.Height = ActualHeight;
                Label.Width  = PromptWidth;
            }
            if (border != null)
            {
                border.Height = ActualHeight;
                border.Width  = ActualWidth;
            }
        }

        #endregion

        private void Editor_OnChanged(object? sender, ChangedEventArgs e)
        {
            if (RaiseOnChange(e.Value))
                RaiseOnChanged(e.Value);
        }

        private void NxTextEdit_Loaded(object sender, RoutedEventArgs e) { Resize(); }        

        public void Resize()
        {
            try
            {
                if (Editor != null )
                {
                    Editor.Width  = ActualWidth - PromptWidth;
                    Editor.Height = ActualHeight;
                }
                if (Label != null)
                {
                    Label.Width  = PromptWidth;
                    Label.Height = ActualHeight;
                }
                if (Editor != null && Label != null)
                {
                    Editor.Width = (ActualWidth - Label.Width);
                }
            }
            catch (Exception x)
            { }
        }

        public void SetFocus()
        {
            if (Editor != null ) Editor.SetFocus();
        }

        #region Dispose

        public void Dispose()
        {
            DispatcherQueue.TryEnqueue(_Dispose);
        }

        public void _Dispose()
        {
        
            if (OnInValid != null) OnInValid = null;
            if (OnChangedControl != null) OnChangedControl = null;
        }

        #endregion

        #region Progress

        public void ShowProgress(int Maximum)
        {
            DispatcherQueue.TryEnqueue(() => _ShowProgress(Maximum));
        }
        private async void _ShowProgress(int Maximum)
        {
            try
            {
                ProgBar.Value = 1;
                ProgBar.Maximum = Maximum;
                ProgBar.Visibility = Visibility.Visible;
                await Task.Yield();
            }
            catch (Exception ex) { }
        }
        public void DoProgress(int Value)
        {
            DispatcherQueue.TryEnqueue(() => _DoProgress(Value));
        }
        private async void _DoProgress(int Value)
        {
            try
               {ProgBar.Value = Value;}
            catch
            {
                ProgBar.Maximum = Value + 10;
                ProgBar.Value = Value;
            }
            if (ProgBar.Visibility != Visibility.Visible)
            {ProgBar.Visibility = Visibility.Visible;}
            await Task.Yield();
        }
        public void HideProgress()
        {
            DispatcherQueue.TryEnqueue(_HideProgress);
        }

        private async void _HideProgress()
        {
            ProgBar.Visibility = Visibility.Collapsed;
            await Task.Yield(); // Yield to UI thread to allow visual update
        }
        #endregion

        public bool ProcessKey(string key, bool Send)
        {
            bool ret = false;
            if (Editor != null ) ret=Editor.ProcessKey(key, Send);
            mValue = Editor.Value;
            return ret;
        }

        public bool Validate()
        {
            return true;
        }

        private void SetControlStyle()
        {
            try
            {
                if (Editor != null ) Editor.IsReadOnly = IsReadOnly;

                if (border != null)
                {
                    border.Background      = IsReadOnly ? new SolidColorBrush(Colors.White) :  null;
                    border.Opacity         = IsReadOnly ? 0.65 : 1.0;
                    border.BorderThickness = IsReadOnly ? new Thickness(2) : new Thickness(4);
                }
                this.IsTabStop = IsReadOnly ? false : true;
            }
            catch (Exception ex)
            {
            }
        }

   


    }
}
