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
    public sealed partial class NxCombo : NxComboBase 
    {
        #region Events & Delegate
       // public delegate void OnChangedControlEventHandler(NxTextEdit Ctl,string Tag, string Display, string Value, ref bool Valid); public event OnChangedControlEventHandler OnChangedControl;

        public delegate void OnInValidEventHandler     (string errorMsg);                                 public event OnInValidEventHandler         OnInValid;
        public delegate void OnAddItemEventHandler     (string Value);                                    public event OnAddItemEventHandler         OnAddItem;
        public delegate void GetFieldLayoutEventHandler(string ColumnName, Int32 GridColumn);             public event GetFieldLayoutEventHandler    GetFieldLayout;
        public delegate void OnLoadListRequestEventHandler();                                             public event OnLoadListRequestEventHandler OnLoadListRequest;
        public delegate void OnDispose();

        public delegate void ShowProgressCB(int Maximum);
        public delegate void DoProgressCB(int Value, string Message);
        public delegate void HideProgressCB();
        public delegate void OnExternalDropEventHandler(string Tag, Windows.Foundation.Rect ScreenRect); public event OnExternalDropEventHandler OnExternalDrop;
        #endregion

        #region Variables
        private bool      mIsValidating     = false; public     bool      IsValidating     { get { return mIsValidating; }       set { mIsValidating = value; } }
        private bool      mIsTouchMode      = false; public     bool      IsTouchMode      { get { return mIsTouchMode; }        set { mIsTouchMode = value; } }
        private bool      mIsTabEvent       = false; public     bool      IsTabEvent       { get { return mIsTabEvent; }         set { mIsTabEvent = value; } }
                                                     public     string    Display          { get { return base.Display; }        set { base.Display = value; } }


        #endregion

        #region Editor Dependency Property
        public static readonly new DependencyProperty IsReadOnlyProperty               = DependencyProperty.Register("IsReadOnly",               typeof(bool),                    typeof(NxCombo), new PropertyMetadata(false));
        public static readonly     DependencyProperty IsRequiredProperty               = DependencyProperty.Register("IsRequired",               typeof(bool),                    typeof(NxCombo), new PropertyMetadata(true));
        public static readonly     DependencyProperty SuppressNavigateProperty         = DependencyProperty.Register("SuppressNavigate",         typeof(bool),                    typeof(NxCombo), new PropertyMetadata(false));
        public static readonly     DependencyProperty ReplyFontFamilyProperty          = DependencyProperty.Register("ReplyFontFamily",          typeof(FontFamily),              typeof(NxCombo), new PropertyMetadata("Arial"));
        public static readonly     DependencyProperty ReplyFontSizeProperty            = DependencyProperty.Register("ReplyFontSize",            typeof(double),                  typeof(NxCombo), new PropertyMetadata(12));
        public static readonly     DependencyProperty ReplyRequiredTextProperty        = DependencyProperty.Register("ReplyRequiredText",        typeof(string),                  typeof(NxCombo), new PropertyMetadata(String.Empty));
        public static readonly     DependencyProperty ReplyInvalidPictureProperty      = DependencyProperty.Register("ReplyInvalidPicture",      typeof(NxImage.Pictures),        typeof(NxCombo), new PropertyMetadata(NxImage.Pictures.cross));
        public static readonly     DependencyProperty ReplyValidPictureProperty        = DependencyProperty.Register("ReplyValidPicture",        typeof(NxImage.Pictures),        typeof(NxCombo), new PropertyMetadata(NxImage.Pictures.tick));
        public static readonly     DependencyProperty MaxTextLengthProperty            = DependencyProperty.Register("MaxTextLength",            typeof(int),                     typeof(NxCombo), new PropertyMetadata(30));
        public static readonly     DependencyProperty MaskTypeProperty                 = DependencyProperty.Register("MaskType",                 typeof(MaskTypes),               typeof(NxCombo), new PropertyMetadata(MaskTypes.None)); 
        public static readonly     DependencyProperty MaskProperty                     = DependencyProperty.Register("Mask",                     typeof(string),                  typeof(NxCombo), new PropertyMetadata(String.Empty));
        public static readonly     DependencyProperty ChangeEventTypeProperty          = DependencyProperty.Register("ChangeEventType",          typeof(NxText.ChangeEventTypes), typeof(NxCombo), new PropertyMetadata(NxText.ChangeEventTypes.ByKeyPress));
        public static readonly     DependencyProperty ReplyHorizontalAlignmentProperty = DependencyProperty.Register("ReplyHorizontalAlignment", typeof(HorizontalAlignment),     typeof(NxCombo), new PropertyMetadata(HorizontalAlignment.Left));
        public static readonly     DependencyProperty ReplyVerticalAlignmentProperty   = DependencyProperty.Register("ReplyVerticalAlignment",   typeof(VerticalAlignment),       typeof(NxCombo), new PropertyMetadata(VerticalAlignment.Center));

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
        #endregion

        #region Drop Dependency Property
        public static readonly DependencyProperty AutoDropProperty              = DependencyProperty.Register("AutoDrop",              typeof(bool),                  typeof(NxCombo), new PropertyMetadata(null)); 
        public static readonly DependencyProperty DropPictureProperty           = DependencyProperty.Register("DropPicture",           typeof(NxImage.Pictures),      typeof(NxCombo), new PropertyMetadata(null));
        public static readonly DependencyProperty LoadlistOnDemandProperty      = DependencyProperty.Register("LoadlistOnDemand",      typeof(bool),                  typeof(NxCombo), new PropertyMetadata(null)); 
        public static readonly DependencyProperty DropPromptProperty            = DependencyProperty.Register("DropPrompt",            typeof(string),                typeof(NxCombo), new PropertyMetadata(null)); 

        public NxImage.Pictures      DropPicture            { get { return (NxImage.Pictures)      GetValue(DropPictureProperty); }            set { SetValue(DropPictureProperty, value); } }
        public bool                  AutoDrop               { get { return (bool)                  GetValue(AutoDropProperty); }               set { SetValue(AutoDropProperty, value); } }
        public bool                  LoadlistOnDemand       { get { return (bool)                  GetValue(LoadlistOnDemandProperty); }       set { SetValue(LoadlistOnDemandProperty, value); } }
        public string                DropPrompt             { get { return (string)                GetValue(DropPromptProperty); }             set { SetValue(DropPromptProperty, value); } }

        private NxComboBase.DropTypes mDropType;  public NxComboBase.DropTypes DropType { get { return mDropType; }     set { mDropType = value; base.DropType = value; Resize(); } }

        #endregion

        #region Dependency Property
        protected NxText      Editor  { get { return GetTemplateChild("editor")     as NxText; } }
        protected NxImage     Button  { get { return GetTemplateChild("button")     as NxImage; } }
        protected ProgressBar ProgBar { get { return GetTemplateChild("progbar")    as ProgressBar; } }
        #endregion

        //protected bool ClearOnEmpty
        //{
        //    get { return this.clearOnEmpty; }
        //    set { this.clearOnEmpty = true; }
        //}

        public static readonly DependencyProperty ContentTemplateProperty      = DependencyProperty.Register("ContentTemplate",      typeof(string), typeof(NxCombo), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultSelectedIndexProperty = DependencyProperty.Register("DefaultSelectedIndex", typeof(int),    typeof(NxCombo), new PropertyMetadata(-1));

        public string ContentTemplate      { get { return (string)GetValue(ContentTemplateProperty); }   set { SetValue(ContentTemplateProperty, value); } }
        public int    DefaultSelectedIndex { get { return (int)GetValue(DefaultSelectedIndexProperty); } set { SetValue(DefaultSelectedIndexProperty, value); } }

        #region Constructor

        public NxCombo()
        {

            DefaultStyleKey = typeof(NxCombo);

            if (Tag == null) Tag = string.Empty;

            this.Loaded          += new RoutedEventHandler      (Combo_Loaded);
            this.PreviewKeyDown  += new KeyEventHandler         (Combo_PreviewKeyDown);
            SizeChanged          += new SizeChangedEventHandler (ComboBox_SizeChanged);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Editor != null)
            {

                Editor.PreviewKeyUp += new KeyEventHandler(Editor_PreviewKeyUp);
                Editor.OnChanged += Editor_OnChanged;

                //Editor.ReplyRequiredText = ReplyRequiredText;
                //Editor.Value             = Display;
                Editor.MaskType          = MaskType;

                //Editor.Value = Display;
            }

            if (Button != null)
            {
                Button.OnClicked    += Button_OnClick;
            }

        }

        #endregion

        #region Public Methods

        private void Combo_Loaded        (object sender, RoutedEventArgs e)      { Resize(); }
        private void ComboBox_SizeChanged(object sender, SizeChangedEventArgs e) { Resize(); }
        public  void Resize()
        {
            try
            {
                if (Editor != null && Button != null)
                {
                    Editor.Height = ActualHeight;
                    Editor.Width = (ActualWidth - ((mDropType.Equals(NxComboBase.DropTypes.None)) || IsReadOnly ? 0 : Button.Width));
                    //Button.SetVisibility( (mDropType.Equals(NxComboBase.DropTypes.None) || IsReadOnly) ? VisiStates.Hidden : VisiStates.Visible);
                }
            }
            catch (Exception x)
            { }
        }


        public void SetFocus()                           { if (Editor != null)        Editor.Focus(FocusState.Programmatic);               }
        public bool ProcessKey(string Key, bool SendKey) { if (Editor != null) return Editor.ProcessKey(Key, SendKey); else return false; }


        #endregion

        #region Private Methods

        private void   Combo_PreviewKeyDown  (object sender, KeyRoutedEventArgs e)
        {
            if (!IsReadOnly)
            {              
                try
                {
                    mIsTabEvent = false;
                    switch (e.Key)
                    {
                        case VirtualKey.Down:
                            ShowDrop(dialoghelpers.ScreenRect(this));
                            break;

                        case VirtualKey.Tab:
                            mIsTabEvent = true;
                            //if (!string.IsNullOrEmpty(List.Display))
                            //{
                            //    if (string.IsNullOrEmpty(Editor.Value)) Editor.Value = List.Display;
                            //}
                            HideDrop();
                            RaiseOnClicked(true);
                            break;

                        case VirtualKey.Enter:
                            HideDrop();
                            //if (cb.IsDropDownOpen)
                            //{
                            //    //List.Value = Editor.Value;                                
                            //    this.Editor.Focus();
                            //    Dispatcher.BeginInvoke(new Action(() => { cb.IsDropDownOpen = false; }));                              
                            //}
                            //else
                            //{
                            //   this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            //}
                            RaiseOnClicked(true);
                            break;       
              
                        case VirtualKey.Escape:
                            HideDrop();
                            break;                     
                    }
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }
            }
            else { e.Handled = true; }

        }

        private void Editor_OnChanged(object? sender, ChangedEventArgs e)
        {
            //Value = cb.GetDisplayValue(Value, false);
            //if (!string.IsNullOrEmpty(Value))
            //{
            //    if (OnChanged != null) OnChanged(Value, Tag);
            //}
            //else
            //{
            //    if (OnAddItem != null) OnAddItem(Value);
            //    else
            //        Editor.Value = "";

            //}
        }
        private void   Editor_PreviewKeyUp   (object sender, KeyRoutedEventArgs e)
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
                            exShowDrop();       
                            break;
                        case VirtualKey.Enter:
                            HideDrop();
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
        private void Button_OnClick (string Tag) { if (!IsReadOnly) exShowDrop();  }

        #endregion

        #region Progress Methods

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

        private void exShowDrop()
        {
            if (DropShown) HideDrop();

            if (LoadlistOnDemand)
            {
                OnLoadListRequest?.Invoke();
                LoadlistOnDemand = false;
            }

            if (DropType.Equals(NxComboBase.DropTypes.External)) { OnExternalDrop?.Invoke(string.IsNullOrEmpty(Tag.ToString()) ? this.Name : Tag.ToString(), ScreenRect(this)); }
            else
            {
                ShowDrop(dialoghelpers.ScreenRect(this));
            }

            if (Editor != null ) Editor.Focus(FocusState.Programmatic);

        }


        #region Override Methods


        protected override void OnLostFocus(RoutedEventArgs e)
        {
           //if (!IsReadonly)
           // {
           //    // if (List != null) { if (List.HasFocus) return; }
           //     try
           //     {
           //         this.IsKeyEvent     = false;
           //         this.IsDropDownOpen = false;

           //         this.searchTimer.Close();// release timer resource

           //         this.Editor.CaretIndex = 0;

           //     }
           //     catch (Exception x) { }
           // }
           // base.OnLostFocus(e);
        }

        //protected void OnSelectionChanged(SelectionChangedEventArgs e)
        //{
        //cb.SearchTimer.Stop();
        // base.OnSelectionChanged(e);
        //}
        protected override void _OnChanged(object? sender, ChangedEventArgs e)
        {
            Value = e.Value;
            Display = FindDisplayFromValue(Value);
            Editor.Value = Display;
            RaiseOnChanged(Value);
        }

        #endregion
    }


}