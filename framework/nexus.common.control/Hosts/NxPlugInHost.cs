using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using nexus.common.control;
using nexus.common.control.Themes;

namespace nexus.common.control
{

    public class PluginMessage
    {
        public string       Type     { get; set; } = "event";
        public string       Id       { get; set; } = Guid.NewGuid().ToString();
        public string       Command  { get; set; } = string.Empty;
        public JsonElement  Payload  { get; set; }
    }

    public class PluginMessageResponse
    {
        public string       Id       { get; set; } = string.Empty;
        public string       Status   { get; set; } = "ok";
        public JsonElement  Data     { get; set; }
    }

    public partial class NxPluginHost : NxPluginBase, IDisposable
    {
        public  string                                              ErrorMessage     = string.Empty;

        public  string                                              PluginFile       => mPluginFile;
        public  string                                              PluginName       => mPluginName;
        public  string                                              EntryPoint       => mEntryPoint;
        public  int                                                 MenuId           => mMenuId;
        public  string                                              Version          => mVersion;
        public  bool                                                DLLLoaded        => Loaded;
        public  string                                              PlugInName       => mPluginFile + "." + mEntryPoint;
        public  WinSize                                                MinSize          => mMinSize;
        public  bool                                                IsWebPlugin      => mIsWebPlugin;

        private string                                              mPluginFile      = string.Empty;
        private string                                              mPluginName      = string.Empty;
        private string                                              mPluginType      = string.Empty;
        private string                                              mEntryPoint      = string.Empty;

        private int                                                 mWidth;
        private int                                                 mHeight;
        private bool                                                mHasBaseSize;
        private WinSize                                                mMinSize;
        private WinSize                                                mMaxSize;
        private bool                                                mScaleToFit;
        private bool                                                mEmbeded;
        private bool                                                Loaded;
        private bool                                                mIsWebPlugin;

        private int                                                 mMenuId;
        private string                                              mVersion;

        private IMainUI?                                            mInstance;
        private ContentControl                                      mControl;
        private WebView2?                                           _webView;
        private Canvas                                              Host;

        private MethodInfo?                                         mProcess;
        private MethodInfo?                                         mExecute;
        private MethodInfo?                                         mShow;
        private MethodInfo?                                         mSave;

        private readonly Dictionary<string, TaskCompletionSource<PluginMessageResponse>> _pendingResponses = new();
        private readonly TimeSpan                                   _responseTimeout = TimeSpan.FromSeconds(10);

        public event EventHandler<Uri>? OnWebNavigationStarting;    private void RaiseWebNavigationStarting(Uri uri) { OnWebNavigationStarting?.Invoke(this, uri); }

        public  event EventHandler<Uri>?                            OnWebNavigationCompleted;
        public  event EventHandler<string>?                         OnWebNavigationFailed;

        public  delegate void                                       OnStartLoadEventHandler(int MaxLoops);
        public  delegate void                                       OnLoadingEventHandler(int Progress, string Message);
        public  delegate void                                       OnLoadedEventHandler();
        public  delegate void                                       OnPluginEventHandler(enums.EventTypes reason, string sender, string command, string property);
        public  delegate void                                       OnPluginResizeEventHandler(double Width, double Height);

        public  event OnStartLoadEventHandler                       OnStartLoad;
        public  event OnLoadingEventHandler                         OnProgress;
        public  event OnLoadedEventHandler                          OnLoaded;
        public  event OnPluginEventHandler                          OnPluginEvent;
        public  event OnPluginResizeEventHandler                    OnPluginResize;

        public NxPluginHost()
        {
            DefaultStyleKey       = typeof(NxPluginHost);
            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("Host") is Canvas host)
                Host = host;
        }

        private void ApplyThemeDefaults()
        {
            Background       = NxThemeManager.Current.PanelBack;
        }

        public bool LoadPlugin(string PluginFile, string EntryPoint = "MainUI")
        {
            object[] Pars = { "" };
            return LoadPlugin(0, PluginFile, EntryPoint, true, true, Pars);
        }
        public bool LoadPlugin(int menuId, string pluginKey, string entryPoint, bool scaleToFit, bool embedded, object[] parameters)
        {
            Reset();

            mMenuId              = menuId;
            mEmbeded             = embedded;
            mScaleToFit          = scaleToFit;
            mPluginFile          = pluginKey;
            mEntryPoint          = entryPoint;

            try
            {
                if (pluginKey.StartsWith("hybrid|"))
                {
                    var parts = pluginKey.Split('|');
                    if (parts.Length == 3 && IsDomainAllowed(parts[2]))
                        return LoadHybridPlugin(parts[1], parts[2], parameters);
                }
                else if (Uri.IsWellFormedUriString(pluginKey, UriKind.Absolute) && IsDomainAllowed(pluginKey))
                {
                    return LoadWebPlugin(pluginKey);
                }
                else
                {
                    return LoadNativePlugin(pluginKey, parameters);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage       = $"Error loading plugin: {ex.Message}";
            }

            return false;
        }

        private bool LoadWebPlugin(string url)
        {
            mPluginName = url;

            mIsWebPlugin         = true;
            _webView             = CreateConfiguredWebView(new Uri(url));
            mControl             = new ContentControl { Content = _webView };

            Host?.Children.Clear();
            Host?.Children.Add(mControl);

            Loaded               = true;
            OnLoaded?.Invoke();
            return true;
        }

        private bool LoadHybridPlugin(string xamlPluginKey, string url, object[] parameters)
        {
            var plugin           = PluginFactory.Create(xamlPluginKey);
            if (plugin is not FrameworkElement uiElement)
            {
                ErrorMessage     = $"Hybrid XAML plugin '{xamlPluginKey}' failed.";
                return false;
            }

            _webView             = CreateConfiguredWebView(new Uri(url));
            var hybridPanel      = new StackPanel();

            hybridPanel.Children.Add(uiElement);
            hybridPanel.Children.Add(_webView);

            mInstance            = plugin;
            mControl             = new ContentControl { Content = hybridPanel };
            mVersion             = plugin.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";

            Host?.Children.Clear();
            Host?.Children.Add(mControl);

            mInstance.Create(parameters.FirstOrDefault()?.ToString() ?? "");
            mIsWebPlugin         = true;
            OnLoaded?.Invoke();

            return true;
        }

        private bool LoadNativePlugin(string pluginKey, object[] parameters)
        {
            var plugin = PluginFactory.Create(pluginKey);
            
            if (plugin is not FrameworkElement uiElement)
            {
                ErrorMessage     = $"Plugin '{pluginKey}' does not provide a valid UI.";
                return false;
            }

            mInstance            = plugin;
            mControl             = new ContentControl { Content = uiElement };
            mVersion             = plugin.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";

            Host?.Children.Clear();
            Host?.Children.Add(mControl);

            var type = mInstance.GetType();
            mProcess = type.GetMethod("Process");
            mExecute = type.GetMethod("Execute", new[] { typeof(WinRect), typeof(string), typeof(string), typeof(string) });
            mShow    = type.GetMethod("Show");
            mSave    = type.GetMethod("Save");

            mInstance.Create(parameters.FirstOrDefault()?.ToString() ?? "");

            if (Loaded)
            {
                if (mScaleToFit) ScalePlugIn();
                else             Resize(new WinSize(mWidth, mHeight));
            }

            OnLoaded?.Invoke();
            return Loaded;
        }

        private WebView2 CreateConfiguredWebView(Uri source)
        {
            var webView          = new WebView2 { Source = source };

            webView.NavigationStarting += (s, e)  => RaiseWebNavigationStarting(source);
            webView.NavigationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                    OnWebNavigationCompleted?.Invoke(this, webView.Source);
                else
                    OnWebNavigationFailed?.Invoke(this, $"Navigation failed: {e.WebErrorStatus}");
            };

            webView.WebMessageReceived += async (s, e) =>
            {
                string json = e.TryGetWebMessageAsString();
                try
                {
                    var msg = JsonSerializer.Deserialize<PluginMessage>(json);
                    if (msg?.Type == "response" && _pendingResponses.TryGetValue(msg.Id, out var tcs))
                    {
                        _pendingResponses.Remove(msg.Id);
                        var response = JsonSerializer.Deserialize<PluginMessageResponse>(json);
                        tcs.TrySetResult(response);
                    }
                    else if (msg != null)
                    {
                        OnPluginEvent?.Invoke(enums.EventTypes.command, "webview", msg.Command, msg.Payload.ToString());
                    }
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine($"Failed to parse plugin message: {ex.Message}");
                }
            };

            return webView;
        }

        private bool IsDomainAllowed(string url)
        {
            //string[] allowedDomains = new[] { "http://localhost:5000", "trustedsite.org" };
            //return Uri.TryCreate(url, UriKind.Absolute, out var uri) && allowedDomains.Any(d => uri.Host.EndsWith(d, StringComparison.OrdinalIgnoreCase));
            return true; // For now, allow all domains. Implement domain checks as needed.
        }

        public async Task<PluginMessageResponse?> SendCommandToWebAsync(string command, object payload)
        {
            if (_webView == null || !mIsWebPlugin)
                return null;

            var msg               = new PluginMessage
            {
                Type             = "command",
                Id               = Guid.NewGuid().ToString(),
                Command          = command,
                Payload          = JsonSerializer.SerializeToElement(payload)
            };

            var json              = JsonSerializer.Serialize(msg);
            var tcs               = new TaskCompletionSource<PluginMessageResponse>();
            _pendingResponses[msg.Id] = tcs;

            await _webView.ExecuteScriptAsync($"window.dispatchEvent(new CustomEvent('hostMessage', {{ detail: {json} }}));");

            using var timeout    = new CancellationTokenSource(_responseTimeout);
            using (timeout.Token.Register(() => tcs.TrySetCanceled()))
            {
                try   { return await tcs.Task; }
                catch { return null; }
            }
        }

        public void Reset()
        {
            mInstance            = null;
            mControl             = null;
            _webView             = null;
            ErrorMessage         = string.Empty;
            Loaded               = false;
            mIsWebPlugin         = false;
            Host?.Children.Clear();
        }

        public void Resize(WinSize panelSize)
        {
            mWidth               = (int)panelSize.Width;
            mHeight              = (int)panelSize.Height;
            Width                = panelSize.Width;
            Height               = panelSize.Height;

            if (!mIsWebPlugin)
            {
                try { mInstance?.Resize(panelSize); } catch { }
            }
        }

        public void ScalePlugIn()
        {
            if (mControl == null || mIsWebPlugin)
                return;

            double scaleX = 1.0, scaleY = 1.0;

            if (mScaleToFit && mHasBaseSize)
            {
                if (mMinSize.Width > 0)  scaleX = mWidth  < mMinSize.Width  ? mWidth  / mMinSize.Width  : 1.0;
                if (mMinSize.Height > 0) scaleY = mHeight < mMinSize.Height ? mHeight / mMinSize.Height : 1.0;
            }

            if (scaleX != 1.0 || scaleY != 1.0)
            {
                mControl.RenderTransform = new ScaleTransform { ScaleX = scaleX, ScaleY = scaleY };
            }
            else
            {
                mControl.Width    = mWidth;
                mControl.Height   = mHeight;
            }
        }

        public bool Process()
        {
            try { return (bool)(mProcess?.Invoke(mControl, null) ?? false); }
            catch { return false; }
        }

        public bool Show()
        {
            try { return (bool)(mShow?.Invoke(mControl, null) ?? false); }
            catch { return false; }
        }

        public bool Save()
        {
            try
            {
                return (bool)(mSave?.Invoke(mControl, null) ?? false);
            }
            catch { return false; }
        }

        public bool Execute(string Command, string Parameters, string FunctionId)
        {
            return Execute(new WinRect(), Command, Parameters, FunctionId);
        }

        public bool Execute(WinRect ScreenRect, string Command, string Parameters, string FunctionId)
        {
            if (mIsWebPlugin && _webView != null)
            {
                _ = SendCommandToWebAsync(Command, new { parameters = Parameters, functionId = FunctionId });
                return true;
            }

            try
            {
                if (mExecute != null)
                {
                    object[] Pars = { ScreenRect, Command, Parameters, FunctionId };
                    return (bool)mExecute.Invoke(mInstance, Pars);
                }
            }
            catch { }

            return false;
        }

        #region IDisposable

        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        #endregion
    }
}
