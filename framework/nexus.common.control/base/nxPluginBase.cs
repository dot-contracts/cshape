using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;

using Microsoft.AspNetCore.SignalR.Client;

using nexus.common;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public class PluginAction
    {
        public string Name { get; set; } = string.Empty;
        public string Key  { get; set; } = string.Empty;

        public PluginAction() {}

        public PluginAction(string name, string key)
        {
            Name = name;
            Key = key;
        }
    }

    public class PluginInfo
    {
        public string Name            { get; set; } = string.Empty;
        public string ConnectionID    { get; set; } = string.Empty;
        public PluginAction[] Actions { get; set; } = Array.Empty<PluginAction>();

        public PluginInfo(string name, string connectionID)
        {
            Name = name;
            ConnectionID = connectionID;
        }
    }

    public partial class NxPluginBase : UserControl
    {
        private PluginInfo     pluginInfo;

        private HubConnection? hubConnection;

        private bool   mAccessSet = false;
        private string mActionCode = "";    public string ActionCode { get { return mActionCode; }  set {  if (!String.IsNullOrEmpty(value)) {  mAccessSet = true; mActionCode = value;  } } }
        private bool   mIsChanged  = false; public bool   IsChanged  { get { return mIsChanged;  }  set { mIsChanged = value;   if (OnChange != null)   OnChange(mIsChanged); } }

        public string  PluginId    { get; set; } = string.Empty;
        public string  Tag         { get; set; } = string.Empty;
        public bool    Enabled     { get; set; } = true;
        public bool    Readonly    { get; set; } = false;

        public event OnChangeEventHandler       OnChange;            public delegate void OnChangeEventHandler   (bool IsChanged);
        public event OnMessageEventHandler      OnMessage;           public delegate void OnMessageEventHandler  (string type, string content);
        public event OnCommandEventHandler      OnCommand;           public delegate void OnCommandEventHandler  (ref bool Canceled, string type, string content);
        public event OnActionEventHandler       OnActionPerformed;   public delegate void OnActionEventHandler   (string key, ref string returnMessage);
        public event OnSetStatusEventHandler    OnSetStatus;         public delegate void OnSetStatusEventHandler(string status);

        public async Task Create(string PluginName, string ConnectionId = "", string hubURL = "http://localhost:9010/pluginhub")
        {
            try
            {
                hubConnection = new HubConnectionBuilder().WithUrl(hubURL).Build();  // Create a new HubConnection

                hubConnection.On<string, string>("OnMessage", (user, message) =>
                {
                    // Invoke the event
                    OnMessage?.Invoke(user, message);
                });

                hubConnection.On<string, string>("OnCommand", (user, message) =>
                {
                    // Invoke the event
                    bool Canceled = false;
                    OnCommand?.Invoke(ref Canceled, user, message);
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
                ConnectionId = hubConnection.ConnectionId;
                pluginInfo = new PluginInfo(PluginName, ConnectionId);
                await hubConnection.SendAsync("RegisterPlugin", PluginName, pluginInfo);

            }
            catch (Exception ex)
            {
                helpers.WriteToLog("hub Connect:" + ex.Message);
            }
        }

        public void SetActions(PluginAction[] Actions)
        {
            if (pluginInfo is not null)
            {
                pluginInfo.Actions = Actions;
                hubConnection?.SendAsync("RegisterPlugin", pluginInfo.Name, pluginInfo);
            }
        }

        public void SendMessage(string type, string content)
        {
            hubConnection?.InvokeAsync("OnMessage", type, content);
        }

        public void SendCommand(string type, string content)
        {
            hubConnection?.InvokeAsync("OnCommand", type, content);
        }

        public bool IsAccessible ()
        {
            bool IsAcc = true;// (mAccessSet ? nexusUserIdentity.Instance.isActivityAccessible(this.nexusActionCode) : true);
            return IsAcc;
        }


        public bool IsConnected =>
            hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
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
        ~NxPluginBase()
        {
            Dispose(false);      // Dispose of unmanaged resources
        }
        #endregion


    }
}
