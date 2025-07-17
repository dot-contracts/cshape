using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

//using Nexus.Services;
//using nexus.shared.common;


namespace nexus.common.control.blazor
{
    public class nxShellBase : ComponentBase
    {
        public HubConnection? hubConnection;
        public Menu[]         _menus        { get; set; } = [];
        public PluginAction[] _actions      { get; set; } = [];
        public string         strPluginSrc  { get; set; } = "";
        public string         strPluginName { get; set; } = "";
        public string         strStatusBar  { get; set; } = "";

        [Inject]
        protected AppStore store { get; set; } = default!;
        [Inject]
        protected IConfiguration config { get; set; } = default!;
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;


        public event OnChangeEventHandler  OnChange;   public delegate void OnChangeEventHandler  (bool IsChanged);
        public event OnMessageEventHandler OnMessage;  public delegate void OnMessageEventHandler (string type, string content);
        public event OnCommandEventHandler OnCommand;  public delegate void OnCommandEventHandler (ref bool Canceled, string type, string content);

        public async Task Create(string ShellName, string ConnectionId = "", string hubURL = "http://localhost:9010/pluginhub")
        {
            _actions = await store.GetHorizontalActions();
            _menus = await store.GetVerticalMenus();

            AuthenticationState state = await AuthStateProvider.GetAuthenticationStateAsync();

            Menu profile = new Menu();
            profile.MenuDescription = state.User.FindFirst(ClaimTypes.Name)?.Value;
            profile.EndPoint = "/auth/profile";
            _menus = _menus.Concat([profile]).ToArray<Menu>();

            strPluginSrc = await store.GetPluginKey();
            strPluginName = _menus.First(m => (m.EndPoint == strPluginSrc)).Property;
            
            hubConnection = new HubConnectionBuilder().WithUrl(hubURL).Build();  // Create a new HubConnection

            hubConnection.On<string, string>("SetStatus", (pluginName, status) =>
            {
                if (pluginName == strPluginName)
                {
                    strStatusBar = status;
                    InvokeAsync(StateHasChanged);
                }
            });

            hubConnection.On<PluginAction[]>("SetPlugin", (actions) =>
            {
                store.SetHorizontalActions(actions);
                _actions = actions;
                InvokeAsync(StateHasChanged);
            });

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

            await hubConnection.StartAsync();                                    // Start the connection

            handleMenuClicked([strPluginSrc, strPluginName]);
        }

        public async void handleMenuClicked(string[] args)
        {
            if (args[0] != "")
            {
                strPluginSrc = args[0];
                store.ClearHorizontalActions();
                store.SetPluginKey(args[0]);
                strStatusBar = "";
                _actions = [];
                if (args[1] != "" && hubConnection is not null)
                {
                    strPluginName = args[1];
                    await hubConnection.SendAsync("GetPluginInfo", strPluginName);
                }
            }
            StateHasChanged();
        }

        public async void handleActionClicked(string ActionKey)
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("OnAction", strPluginName, ActionKey);
            }
            StateHasChanged();
        }

        public void handleSettingClicked()
        {
            handleMenuClicked(["/auth/profile", ""]);
        }

        public async void onQuit()
        {
            store.ClearUserCredential();
            strStatusBar = "";
            await AuthStateProvider.GetAuthenticationStateAsync();
        }


        public void SendMessage(string type, string content)
        {
            hubConnection?.InvokeAsync("OnMessage", type, content);
        }

        public void SendCommand(string type, string content)
        {
            hubConnection?.InvokeAsync("OnCommand", type, content);
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
        ~nxShellBase()
        {
            if (hubConnection is not null)
                hubConnection.DisposeAsync();

            Dispose(false);      // Dispose of unmanaged resources
        }
        #endregion


    }
}
