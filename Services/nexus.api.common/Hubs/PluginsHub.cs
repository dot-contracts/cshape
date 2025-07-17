using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;


namespace nexus.api.common.Hubs;

public class PluginsHub : Hub
{
    private static readonly Dictionary<string, PluginInfo> PluginConnections = new Dictionary<string, PluginInfo>();

    public void RegisterPlugin(string pluginName, PluginInfo plugInfo)
    {
        PluginConnections[pluginName] = plugInfo;
    }

    public async Task OnAction(string pluginName, string key)
    {
        if (PluginConnections.ContainsKey(pluginName))
        {
            await Clients.Client(PluginConnections[pluginName].ConnectionID).SendAsync("ActionPerform", key);
        }
    }

    public async Task OnStatus(string pluginName, string status)
    {
        await Clients.All.SendAsync("SetStatus", pluginName, status);
    }

    public async Task GetPluginInfo(string pluginName)
    {
        if (PluginConnections.ContainsKey(pluginName))
        {
            await Clients.Caller.SendAsync("SetPlugin", PluginConnections[pluginName].Actions);
        }
        else
        {
            await Clients.Caller.SendAsync("SetPlugin", Array.Empty<PluginAction>());
        }
    }

}
