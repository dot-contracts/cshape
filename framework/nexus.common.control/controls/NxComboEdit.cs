using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

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

namespace nexus.common.control
{
    public partial class NxComboEdit : NxComboBase, IControl
    {

        private NxComboBase cb = new NxComboBase(); public NxComboBase ComboBase { get { return cb; } set { cb = value; } }

        #region Events & Delegate
        public delegate void OnChangedControlEventHandler(NxTextEdit Ctl,string Tag, string Display, string Value, ref bool Valid); public event OnChangedControlEventHandler OnChangedControl;
        public delegate void OnProcessKeyEventHandler    (string Key);                                                              public event OnProcessKeyEventHandler     OnProcessKey;


        public delegate void OnInValidEventHandler     (string errorMsg);                                 public event OnInValidEventHandler         OnInValid;
        public delegate void OnAddItemEventHandler     (string Value);                                    public event OnAddItemEventHandler         OnAddItem;
        public delegate void GetFieldLayoutEventHandler(string ColumnName, Int32 GridColumn);             public event GetFieldLayoutEventHandler    GetFieldLayout;
        public delegate void OnLoadListRequestEventHandler();                                             public event OnLoadListRequestEventHandler OnLoadListRequest;
        public delegate void OnDispose();

        public delegate void OnExternalDropEventHandler(string Tag, Windows.Foundation.Rect ScreenRect); public event OnExternalDropEventHandler OnExternalDrop;
        #endregion

        #region Variables
        private bool      mIsValidated      = false; public     bool      IsValidated      { get { return mIsValidated; }        set { mIsValidated = value; } }
        private bool      mIsValidating     = false; public     bool      IsValidating     { get { return mIsValidating; }       set { mIsValidating = value; } }
        private bool      mIsTouchMode      = false; public     bool      IsTouchMode      { get { return mIsTouchMode; }        set { mIsTouchMode = value; } }
        private bool      mIsTabEvent       = false; public     bool      IsTabEvent       { get { return mIsTabEvent; }         set { mIsTabEvent = value; } }
                                                     public     bool      AllowMultiSelect { get { return cb.AllowMultiSelect; } set { cb.AllowMultiSelect = value; } }
                                                     public     string    Display          { get { return cb.Display; }          set { cb.Display = value; if (Editor != null) Editor.Value = cb.Display; } }
                                                     public     string    Value            { get { return cb.Value; }            set { cb.Value = value;   if (Editor != null) Editor.Value = cb.Display; } }
                                                     public     string    List             { get { return cb.List; }             set { cb.List = value;  } }

        public double ValueDbl { get; set; } = 0;

        public DateTime DateValue { 
            get {
                DateTime dt= new DateTime();
                DateTime.TryParse(cb.Value, out dt);
                return dt;
            } }

        #endregion

        #region Prompt Dependency Property
        public static readonly      DependencyProperty PromptProperty                    = DependencyProperty.Register("Prompt",                    typeof(string),                typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptWidthProperty               = DependencyProperty.Register("PromptWidth",               typeof(double),                typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontFamilyProperty          = DependencyProperty.Register("PromptFontFamily",          typeof(FontFamily),            typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptFontSizeProperty            = DependencyProperty.Register("PromptFontSize",            typeof(double),                typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptMarginProperty              = DependencyProperty.Register("PromptMargin",              typeof(Thickness),             typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty MaxPromptWidthProperty            = DependencyProperty.Register("MaxPromptWidth",            typeof(double),                typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptHorizontalAlignmentProperty = DependencyProperty.Register("PromptHorizontalAlignment", typeof(HorizontalAlignment),   typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptVerticalAlignmentProperty   = DependencyProperty.Register("PromptVerticalAlignment",   typeof(VerticalAlignment),     typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly      DependencyProperty PromptTextWrappingProperty        = DependencyProperty.Register("PromptTextWrapping",        typeof(TextWrapping),          typeof(NxComboEdit), new PropertyMetadata(null));

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
        public static readonly new DependencyProperty IsReadOnlyProperty               = DependencyProperty.Register("IsReadOnly",               typeof(bool),                    typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty IsRequiredProperty               = DependencyProperty.Register("IsRequired",               typeof(bool),                    typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty SuppressNavigateProperty         = DependencyProperty.Register("SuppressNavigate",         typeof(bool),                    typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyFontFamilyProperty          = DependencyProperty.Register("ReplyFontFamily",          typeof(FontFamily),              typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyFontSizeProperty            = DependencyProperty.Register("ReplyFontSize",            typeof(double),                  typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyRequiredTextProperty        = DependencyProperty.Register("ReplyRequiredText",        typeof(string),                  typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyInvalidPictureProperty      = DependencyProperty.Register("ReplyInvalidPicture",      typeof(NxImage.Pictures),        typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyValidPictureProperty        = DependencyProperty.Register("ReplyValidPicture",        typeof(NxImage.Pictures),        typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty MaxTextLengthProperty            = DependencyProperty.Register("MaxTextLength",            typeof(int),                     typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty MaskTypeProperty                 = DependencyProperty.Register("MaskType",                 typeof(MaskTypes),               typeof(NxComboEdit), new PropertyMetadata(null)); 
        public static readonly     DependencyProperty MaskProperty                     = DependencyProperty.Register("Mask",                     typeof(string),                  typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ChangeEventTypeProperty          = DependencyProperty.Register("ChangeEventType",          typeof(NxText.ChangeEventTypes), typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyHorizontalAlignmentProperty = DependencyProperty.Register("ReplyHorizontalAlignment", typeof(HorizontalAlignment),     typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly     DependencyProperty ReplyVerticalAlignmentProperty   = DependencyProperty.Register("ReplyVerticalAlignment",   typeof(VerticalAlignment),       typeof(NxComboEdit), new PropertyMetadata(null));

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

        public Brush PromptBackColor       { get => PromptBack;   set => PromptBack = value; }
        public Brush PromptForeColor       { get => PromptFore;   set => PromptFore = value; }
        public Brush ReplyBackColor        { get => ReplyBack;    set => ReplyBack  = value; }
        public Brush ReplyForeColor        { get => ReplyFore;    set => ReplyFore  = value; }
        public Brush ReplyInvalidBackColor { get => ReplyInValid; set => ReplyInValid = value; }
        public Brush ReplyValidBackColor   { get => ReplyValid;   set => ReplyValid = value; }


        #endregion

        #region Drop Dependency Property
        public static readonly DependencyProperty DropHorizontalAlignmentProperty = DependencyProperty.Register("DropHorizontalAlignment",  typeof(HorizontalAlignment),   typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly DependencyProperty AutoDropProperty                = DependencyProperty.Register("AutoDrop",                 typeof(bool),                  typeof(NxComboEdit), new PropertyMetadata(null)); 
        public static readonly DependencyProperty DropPictureProperty             = DependencyProperty.Register("DropPicture",              typeof(NxImage.Pictures),      typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly DependencyProperty LoadlistOnDemandProperty        = DependencyProperty.Register("LoadlistOnDemand",         typeof(bool),                  typeof(NxComboEdit), new PropertyMetadata(null)); 
        public static readonly DependencyProperty DropPromptProperty              = DependencyProperty.Register("DropPrompt",               typeof(string),                typeof(NxComboEdit), new PropertyMetadata(null));


        public NxImage.Pictures      DropPicture            { get { return (NxImage.Pictures)      GetValue(DropPictureProperty); }            set { SetValue(DropPictureProperty, value); } }
        public bool                  AutoDrop               { get { return (bool)                  GetValue(AutoDropProperty); }               set { SetValue(AutoDropProperty, value); } }
        public bool                  LoadlistOnDemand       { get { return (bool)                  GetValue(LoadlistOnDemandProperty); }       set { SetValue(LoadlistOnDemandProperty, value); } }
        public string                DropPrompt             { get { return (string)                GetValue(DropPromptProperty); }             set { SetValue(DropPromptProperty, value); } }

        
        public NxComboBase.DropTypes DropType { get { return cb.DropType; } set { cb.DropType = value; Resize(); } }
        #endregion

        #region Dependency Property
        protected NxLabel      Label   { get { return base.GetTemplateChild("label")      as NxLabel; } }
        protected NxText       Editor  { get { return base.GetTemplateChild("editor")     as NxText; } }
        protected NxImage      Button  { get { return base.GetTemplateChild("button")     as NxImage; } }
        protected ProgressBar  ProgBar { get { return base.GetTemplateChild("progbar")    as ProgressBar; } }
        #endregion

        //protected bool ClearOnEmpty
        //{
        //    get { return this.clearOnEmpty; }
        //    set { this.clearOnEmpty = true; }
        //}

        public static readonly DependencyProperty ContentTemplateProperty      = DependencyProperty.Register("ContentTemplate",      typeof(string), typeof(NxComboEdit), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultSelectedIndexProperty = DependencyProperty.Register("DefaultSelectedIndex", typeof(int),    typeof(NxComboEdit), new PropertyMetadata(-1));

        public string ContentTemplate      { get { return (string)GetValue(ContentTemplateProperty); }   set { SetValue(ContentTemplateProperty, value); } }
        public int    DefaultSelectedIndex { get { return (int)GetValue(DefaultSelectedIndexProperty); } set { SetValue(DefaultSelectedIndexProperty, value); } }
              
        public NxComboEdit()
        {
            DefaultStyleKey = typeof(NxComboEdit);

            if (Tag == null) Tag = string.Empty;

            this.IsTabStop = false;

            this.Loaded      += NxComboEdit_Loaded;
            base.SizeChanged += NxComboEdit_SizeChanged;
        }

        #region Override Methods

        public void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Editor != null)
            {
                Editor.ReplyRequiredText = ReplyRequiredText;
                Editor.IsRequired        = IsRequired;
                Editor.Value             = cb.Display;
                Editor.MaskType          = MaskType;

                Editor.OnChanged    += Editor_OnChanged;
                Editor.PreviewKeyUp += new KeyEventHandler(Editor_PreviewKeyUp);
                Editor.OnProcessKey += Editor_OnProcessKey;

                Editor.Value = cb.Display;
            }

            if (Button != null)
            {
                Button.OnClicked += Button_OnClick;
            }

            Resize();
        }

        private void Editor_OnProcessKey(string Key)                                    { OnProcessKey?.Invoke(Key); }

        #endregion


        private void Editor_OnChanged(object? sender, ChangedEventArgs e)
        {
            cb.Display = e.Value;
            RaiseOnChanged(e.Value);
        }

        private void NxComboEdit_Loaded      (object sender, RoutedEventArgs e)      { Resize(); }
        private void NxComboEdit_SizeChanged (object sender, SizeChangedEventArgs e) { Resize(); }
        public void Resize()
        {
            try
            {
                if (Editor != null && Button != null && Label != null)
                {
                    Editor.Height = ActualHeight;
                    Label.Height  = ActualHeight;
                    Label.Width   = PromptWidth;
                    Editor.Width = (ActualWidth - PromptWidth - ((cb.DropType.Equals(NxComboBase.DropTypes.None)) || IsReadOnly   ? 0: Button.Width ));
                    Button.Visibility = (cb.DropType.Equals(NxComboBase.DropTypes.None)                           || IsReadOnly ) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception x)
            { }
        }

        private void Button_OnClick          (string Tag) { if (!IsReadOnly) ShowDrop();  }

        private void Editor_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            string Value = (string)((NxText)(sender)).Editor.Text;

            //mValue = Value;

            if (!IsReadOnly)
            {
                try
                {
                    switch (e.Key)
                    {
                        case VirtualKey.Tab:
                            break;
                        case VirtualKey.Down:
                            ShowDrop();
                            break;
                        case VirtualKey.Enter:
                            cb.HideDrop();
                            break;

                        default:
                            ////// if (this.IsEditable)
                            //// {
                            ////     if (List != null)
                            ////     {
                            ////         if (!string.IsNullOrEmpty(Value)) ResetTimer();
                            ////         else
                            ////             List.ResetList();
                            ////     }
                            //// }
                            break;
                    }

                }
                catch (Exception ex)
                { }

            }
        }

        private void ShowDrop()
        {
            if (cb.DropShown) cb.HideDrop();

            if (LoadlistOnDemand)
            {
                OnLoadListRequest?.Invoke();
                LoadlistOnDemand = false;
            }

            if (cb.DropType.Equals(NxComboBase.DropTypes.External)) { OnExternalDrop?.Invoke(string.IsNullOrEmpty(Tag.ToString()) ? this.Name : Tag.ToString(), dialoghelpers.ScreenRect(this)); }
            else
            {
                if (cb.ShowDrop(dialoghelpers.ScreenRect(this)))
                {
                    Editor.Value = cb.Display;
                    Editor.Validate();
                    if (RaiseOnChange(cb.Value))
                        RaiseOnChanged(cb.Value);
                }
            }

            if (Editor != null) Editor.SetFocus();

        }

        public Windows.Foundation.Rect ScreenRect() { return dialoghelpers.ScreenRect(this); }
        public Windows.Foundation.Point GetDialogLocation(Object Form) { return new Windows.Foundation.Point(); }//   helpers.GetDialogLocation(Form, this); }

        public void SetList(string Data)                                                { cb.SetDataTable(helpers.CreateDataTable(Data)); }
        public void SetDataTable(DataTable Data)                                        { cb.SetDataTable(Data, "Description", "Id");    }
        public void SetDataTable(DataTable Data, string DisplayPath, string ValuePath)  { cb.SetDataTable(Data, DisplayPath, ValuePath); }
        public void SetEnumTable(DataTable Data)                                        { cb.SetDataTable(Data, "ValueDesc", "ValuePK"); }
        public bool HasRows()                                                           { return cb.HasRows(); }


        public bool Validate()
        {
            cb.Value = cb.FindValueFromDisplay(cb.Display);
            cb.Display = cb.FindDisplayFromValue(cb.Value);
            Editor.Value = cb.Display;
            return true;
        }

        public void Focus() { }
        public void SetFocus()
        {
            if (Editor != null) Editor.SetFocus();
        }

        public bool ProcessKey(string Key, bool SendKey)
        {
            if (Editor != null) return Editor.ProcessKey(Key, SendKey); else return true;
        }


        #region Progress

        public void ShowProgress(int maximum)
        {
            DispatcherQueue.TryEnqueue(() => {
                if (ProgBar != null)
                {
                    ProgBar.Value = 0;
                    ProgBar.Maximum = maximum;
                    ProgBar.Visibility = Visibility.Visible;
                }
            });
        }

        public void DoProgress(int value)
        {
            DispatcherQueue.TryEnqueue(() => {
                if (ProgBar != null)
                {
                    if (value > ProgBar.Maximum) ProgBar.Maximum = value + 10;
                    ProgBar.Value = value;
                }
            });
        }

        public void HideProgress()
        {
            DispatcherQueue.TryEnqueue(() => {
                if (ProgBar != null)
                {
                    ProgBar.Visibility = Visibility.Collapsed;
                }
            });
        }
        #endregion




    }
}




