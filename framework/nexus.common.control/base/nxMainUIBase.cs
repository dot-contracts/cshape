using Microsoft.AspNetCore.SignalR.Client;
using nexus.common;
using nexus.common.control.Themes;
using nexus.common.control;
using nexus.common.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Windows.Foundation;

namespace nexus.common.control
{

    public class MainInfo
    {
        public string Name            { get; set; } = string.Empty;
        public string ConnectionID    { get; set; } = string.Empty;
        public PluginAction[] Actions { get; set; } = Array.Empty<PluginAction>();

        public MainInfo(string name, string connectionID)
        {
            Name         = name;
            ConnectionID = connectionID;
        }
    }

    public partial class NxMainUIBase : Page, IMainUI
    {
        #region Public Properties

        private HubConnection? hubConnection;
        public  MainInfo       MainInfo;

        public bool   IsBusy          { get; set; }
        public bool   IsActive        { get; set; }
        public bool   IsLoadComplete  { get; set; }

        public int    LoadState       { get; set; }
        public string LastPanel       { get; set; }
        public int    MaxLoadLoops    { get; set; }
        public bool   PauseLoad       { get; set; }

        public WinSize   MinSize         { get; set; }
        public WinSize   MaxSize         { get; set; }

        public string LoadError       { get; set; }
        public string ActionMenu      { get; set; }
        public string ModuleId        { get; set; }

        public PropertyBag<NameValue> PropBag;

        #endregion

        #region Events

        public class OnMessageArgs
        {
            public string Tag     { get; set; }
            public string Display { get; set; }
            public string Value   { get; set; }
            public bool   Valid   { get; set; } = true;

            public OnMessageArgs(string Tag, string Display, string Value)
            {
                this.Tag = Tag;
                this.Display = Display;
                this.Value = Value;
            }
        }

        public delegate void  OnQuitEventHandler           ();                                           public event OnQuitEventHandler           OnQuit;
        public delegate void  OnStartLoadEventHandler      (int MaxLoops);                               public event OnStartLoadEventHandler      OnStartLoad;
        public delegate void  OnLoadLoopEventHandler       ();                                           public event OnLoadLoopEventHandler       LoadLoop;
        public delegate void  OnLoopFinishedEventHandler   ();                                           public event OnLoopFinishedEventHandler   LoopFinished;
        public delegate void  OnLoadingEventHandler        (int Progress, string Message);               public event OnLoadingEventHandler        OnProgress;
        public delegate void  OnLoadedEventHandler         ();                                           public event OnLoadedEventHandler         OnLoaded;
        public delegate void  OnResizeEventHandler         (double Width, double Height);                public event OnResizeEventHandler         OnResize;
        
        public event OnChangeEventHandler         OnChange;            public delegate void OnChangeEventHandler    (bool IsChanged);
        public event OnActionEventHandler         OnActionPerformed;   public delegate void OnActionEventHandler    (string key, ref string returnMessage);
        public event OnSetStatusEventHandler      OnSetStatus;         public delegate void OnSetStatusEventHandler (string status);

        public event OnHubCommandEventHandler     OnHubCommand;        public delegate void OnHubCommandEventHandler(ref bool Canceled, string user, string message);
        public event OnHubMessageEventHandler     OnHubMessage;        public delegate void OnHubMessageEventHandler(string user, string message);
        public event EventHandler<OnMessageArgs>? OnMessage;

        #endregion

        #region Constructor

        public NxMainUIBase()
        {
            this.MainInfo = MainInfo;

            PropBag = new PropertyBag<NameValue>();
            PropBag.Add(new NameValue("PlugIn",   ""));
            PropBag.Add(new NameValue("ModuleId", ""));

            MinSize = new WinSize(800, 600);
            MaxSize = new WinSize(1366, 768);

            ApplyThemeDefaults();                  // 🚀 Immediately apply panel background
            this.Loaded += NxMainUIBase_Loaded;     // 🚀 Hook Loaded
        }

        private void NxMainUIBase_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyThemeDefaults();                  // 🚀 Apply again if Theme changed
            NxThemeManager.ThemeChanged += (s, args) => ApplyThemeDefaults(); // 🚀 Update when Theme changes

            OnLoaded?.Invoke();
        }

        #endregion

        #region Theme Support

        private void ApplyThemeDefaults()
        {
            if (this.ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
            {
                this.Background = NxThemeManager.Current.PanelBack.Brush;
            }
        }

        #endregion

        #region Timer and Loading

        private System.Timers.Timer mTimer;
        private delegate void TimerCB();

        public async Task Create(string PluginName, string Property = "", string hubURL = "http://localhost:9010/pluginhub" )
        {
            Array Arr = (Property + ";;;").Split(';');
            PropBag["PlugIn"].Value   = Arr.GetValue(0).ToString();
            PropBag["ModuleId"].Value = Arr.GetValue(1).ToString();

            mTimer = new System.Timers.Timer();
            //TimerStart(MaxLoadLoops);

            hubConnection = new HubConnectionBuilder().WithUrl(hubURL).Build();  // Create a new HubConnection

            hubConnection.On<string, string>("OnMessage", (user, message) =>
            {
                // Invoke the event
                OnHubMessage?.Invoke(user, message);
            });

            hubConnection.On<string, string>("OnCommand", (user, message) =>
            {
                // Invoke the event
                bool Canceled = false;
                OnHubCommand?.Invoke(ref Canceled, user, message);
            });

            hubConnection.On<string>("ActionPerform", async (key) =>
            {
                string returnMessage = "";
                OnActionPerformed?.Invoke(key, ref returnMessage);
                await hubConnection.SendAsync("OnStatus", PluginName, returnMessage);
            });

            hubConnection.On<string, string>("SetStatus", (pluginName, status) =>
            {
                if (pluginName == PluginName)
                {
                    OnSetStatus?.Invoke(status);
                }
            });

            await hubConnection.StartAsync();                                          // Start the connection
            //ConnectionId = hubConnection.ConnectionId;
            //mainInfo = new MainInfo(PluginName, ConnectionId);
            await hubConnection.SendAsync("RegisterPlugin", PluginName, MainInfo);

        }

        public virtual void Reset() { }
        public virtual bool Save() { return false; }
        public virtual bool Execute(WinRect ScreenRect, enums.EventTypes Reason, string SourcePlugin, string Command, string Parameters = "", string DestinationPlugin = "") { return true; }
        public virtual bool Process() { return false; }

        public virtual void Resize(Windows.Foundation.Size PanelSize)
        {
            this.Width  = PanelSize.Width;
            this.Height = PanelSize.Height;
        }

        public void TimerStart(int maxLoadLoops)
        {
            MaxLoadLoops = maxLoadLoops;
            OnStartLoad?.Invoke(MaxLoadLoops);

            if (mTimer == null)
                mTimer = new System.Timers.Timer();

            mTimer.AutoReset = false;
            mTimer.Interval = 100;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTimer_Elapsed);
            mTimer.Start();
        }

        private async void mTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(ProcessTimer);
        }

        private async void ProcessTimer()
        {
            mTimer.Stop();

            if (!IsBusy)
            {
                IsBusy = true;

                if (!PauseLoad)
                {
                    try { LoadLoop?.Invoke(); }
                    catch (Exception ex)
                    {
                        LoadError = ex.Message;
                        LoadState = MaxLoadLoops;
                        await dialoghelpers.ShowMessageAsync(this.XamlRoot, LoadError, "Error");
                    }
                }

                IsBusy = false;
            }

            await Task.Yield();

            if (LoadState < MaxLoadLoops)
                mTimer.Start();
            else
                LoopFinished?.Invoke();
        }

        public void ProcessLoop(int loadState, string Message)
        {
            LoadState = loadState;
            OnProgress?.Invoke(LoadState, Message);
        }

        public void ProgressStart(int maxLoadLoops)
        {
            OnStartLoad?.Invoke(maxLoadLoops);
        }

        public void Progress(int loadState, string Message)
        {
            OnProgress?.Invoke(loadState, Message);
        }

        public void ProgressFinish()
        {
            OnLoaded?.Invoke();
        }

        #endregion
    }
}