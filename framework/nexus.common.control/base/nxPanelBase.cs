using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxPanelBase : ContentControl, IDisposable
    {
        #region Public Properties

        public PropertyBag<NameValue> PropBag = new();
        public string AllowedActions;
        public string LastActions;
        public string ModuleId;

        private string mPrompt;        public string Prompt        { get { return mPrompt; }       set { mPrompt = value; } }
        private string mName;          public string Name          { get { return mName; }         set { mName = value; } }
        private string mActionCode;    public string ActionCode    { get { return mActionCode; }   set { mActionCode = value; } }
        private string mActionMenu;    public string ActionMenu    { get { return mActionMenu; }   set { mActionMenu = value; } }

        private bool   mAllowFinish;   public bool   AllowFinish   { get { return mAllowFinish; }  set { mAllowFinish = value; if (OnAllowFinish != null) OnAllowFinish(mAllowFinish); } }
        private bool   mAllowChange;   public bool   AllowChange   { get { return mAllowChange; }  set { mAllowChange = value; } }

        private bool   mPinIsActive;   public bool   PinIsActive   { get { return mPinIsActive; }  set { mPinIsActive = value; if (OnActive != null) OnActive(mPinIsActive); } }
        private bool   mIsChanged;     public bool   IsChanged     { get { return mIsChanged; }    set { mIsChanged = value;   if (OnChange != null) OnChange(mIsChanged); } }
        private bool   mIsValid;       public bool   IsValid       { get { return mIsValid; }      set { mIsValid = value;     if (OnValid != null)  OnValid(mIsValid); } }
        private bool   mIsRequired;    public bool   IsRequired    { get { return mIsRequired; }   set { mIsRequired = value;  if (OnRequired != null) OnRequired(mIsRequired); } }
        private bool   mIsHidden;      public bool   IsHidden      { get { return mIsHidden; }     set { mIsHidden = value; } }
        private bool   mIsLoaded;      public bool   IsLoaded      { get { return mIsLoaded; }     set { mIsLoaded = value; } }
        private bool   mIsShown;       public bool   IsShown       { get { return mIsShown; }      set { mIsShown = value; } }
        private bool   mIsReadonly;    public bool   IsReadonly    { get { return mIsReadonly; }   set { mIsReadonly = value;  if (OnReadonlyChanged != null) OnReadonlyChanged(mIsReadonly); } }
        private bool   mIsCollapsed;   public bool   IsCollapsed   { get { return mIsCollapsed; }  set { mIsCollapsed = value; if (OnCollapseChanged != null) OnCollapseChanged(mIsCollapsed); } }
        
        private string mDataError;     public string DataError     { get { return mDataError; }    set { mDataError = value; } }
        private string mPrimaryK;      public string PrimaryK      { get { return mPrimaryK; }     set { mPrimaryK = value; } }
        private string mListProc;      public string ListProc      { get { return mListProc; }     set { mListProc = value; } }
        private string mDisplayPath;   public string DisplayPath   { get { return mDisplayPath; }  set { mDisplayPath = value; } }
        private bool   mHasParent;     public bool   HasParent     { get { return mHasParent; }    set { mHasParent = value; } }

        #endregion

        #region Public Events

        public event EventHandler<CloseEventArgs>?  OnClose;

        public event OnPanelEventHandler            OnEvent;           public delegate void OnPanelEventHandler(enums.EventTypes reason, string sender = "", string command = "", string Property = "");

        public event OnActiveEventHandler           OnActive;          public delegate void OnActiveEventHandler          (bool IsActive);
        public event OnChangeEventHandler           OnChange;          public delegate void OnChangeEventHandler          (bool IsChanged);
        public event OnValidEventHandler            OnValid;           public delegate void OnValidEventHandler           (bool IsValid);
        public event OnRequiredEventHandler         OnRequired;        public delegate void OnRequiredEventHandler        (bool IsRequired);
        public event OnAllowFinishEventHandler      OnAllowFinish;     public delegate void OnAllowFinishEventHandler     (bool AllowFinish);
        public event OnReadonlyChangedEventHandler  OnReadonlyChanged; public delegate void OnReadonlyChangedEventHandler (bool IsReadonly);
        public event OnCollapseChangedEventHandler  OnCollapseChanged; public delegate void OnCollapseChangedEventHandler (bool IsCollapsed);

        #endregion

        #region Constructors

        public NxPanelBase()
        {
            ApplyThemeDefaults();                        // 🚀 Immediately apply theme defaults

            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment   = VerticalAlignment.Stretch;

            this.Loaded += NxPanelBase_Loaded;            // 🚀 Hook Loaded
        }

        private void NxPanelBase_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyThemeDefaults();                        // 🚀 Apply again in case theme changed
            NxThemeManager.ThemeChanged += (s, args) => ApplyThemeDefaults();  // 🚀 Auto update on ThemeChanged
        }

        #endregion

        #region Theming Support

        private void ApplyThemeDefaults()
        {
            if (this.ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
            {
                this.Background = NxThemeManager.Current.PanelBack.Brush;
            }
        }

        #endregion

        #region ISnapin Implementation (Optional override methods)

        public virtual void Create(PropertyBag<NameValue> Context) { }
        public virtual void Reset () { }
        public virtual bool Load  () { return true; }
        public virtual bool Show  () { return true; }
        public virtual bool Save  () { return true; }
        public virtual bool Edit  () { return true; }
        public virtual bool Cancel() { return true; }
        public virtual bool Delete() { return true; }

        public virtual string Confirm() { return string.Empty; }
        public virtual bool Execute(System.Drawing.Rectangle ScreenRect, string Command, string Parameters, string FunctionId) { return true; }
        public virtual bool Update() { return true; }
        public virtual void SetActivity(string ActionCodes) { }

        #endregion

        public bool RaiseClose(string Value)
        {
            var args = new CloseEventArgs(Value);
            OnClose?.Invoke(this, args);
            return args.Valid;
        }

        public void RaiseEvent(enums.EventTypes reason, string sender = "", string command = "", string Property = "")
        {
            OnEvent?.Invoke(reason, sender, command, Property); 
        }


        #region Resizing Helpers

        public void ReSize(WinSize PanelSize)
        {
            this.Width  = PanelSize.Width;
            this.Height = PanelSize.Height;
        }

        public WinSize WinSize()
        {
            return new WinSize(this.ActualWidth, this.ActualHeight);
        }

        public System.Drawing.Rectangle ScreenRect()
        {
            return default(System.Drawing.Rectangle);
        }

        #endregion

        #region IDisposable Implementation

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                try
                {
                    if (disposing)
                    {
                        // Dispose managed resources here
                    }
                    // Dispose unmanaged resources here
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        ~NxPanelBase()
        {
            Dispose(false);
        }

        #endregion
    }
}
