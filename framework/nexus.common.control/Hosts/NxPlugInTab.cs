using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.Foundation;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common.control.Themes;
using nexus.common.control;

namespace nexus.common.control
{
    public partial class NxPluginTab : NxTabBase, IDisposable
    {
        public int CurrentPlugin = -1;

        public event OnStartLoadEventHandler OnStartLoad;
        public event OnLoadingEventHandler OnProgress;
        public event OnLoadedEventHandler OnLoaded;
        public event OnLoadButtonEventHandler OnLoadButton;
        public event OnPluginResizeEventHandler OnPluginResize;

        public delegate void OnStartLoadEventHandler(int MaxLoops);
        public delegate void OnLoadingEventHandler(int Progress, string Message);
        public delegate void OnLoadedEventHandler();
        public delegate void OnLoadButtonEventHandler(string ModuleId, string PluginTag, string Prompt);
        public delegate void OnPluginResizeEventHandler(double Width, double Height);


        public NxPluginTab()
        {
            DefaultStyleKey = typeof(NxPluginTab);

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Host = GetTemplateChild("Host") as Canvas;
        }
        private void ApplyThemeDefaults()
        {
            Background = NxThemeManager.Current.PanelBack;
        }

        private Canvas Host;

        public bool LoadPlugin(string prompt, int menuId, string pluginFile, string entryPoint, bool scaleToFit, bool embedded, object[] parameters = null)
        {
            parameters ??= new object[] { string.Empty };
            string tag = Path.GetFileNameWithoutExtension(pluginFile) + entryPoint;

            CurrentPlugin = -1;

            for (int i = 0; i < Host.Children.Count; i++)
            {
                if (Host.Children[i] is NxPluginHost host)
                {
                    host.Visibility = Visibility.Collapsed;
                    if (host.PluginName + host.EntryPoint == tag)
                        CurrentPlugin = i;
                }
            }

            if (CurrentPlugin < 0)
            {
                NxPluginHost host = new();

                host.Background = new SolidColorBrush(Colors.BlueViolet);

                Host.Children.Add(host);
                host.OnStartLoad += (max)    => OnStartLoad?.Invoke(max);
                host.OnProgress  += (p, msg) => OnProgress?.Invoke(p, msg);
                host.OnLoaded    += ()       => OnLoaded?.Invoke();

                if (host.LoadPlugin(menuId, pluginFile, entryPoint, scaleToFit, embedded, parameters))
                {
                    CurrentPlugin = Host.Children.Count - 1;
                    host.Resize(new WinSize(Host.ActualWidth, Host.ActualHeight));

                    host.Width  = Host.ActualWidth;
                    host.Height = Host.ActualHeight;
                    host.Visibility = Visibility.Visible;   


                    OnLoadButton?.Invoke(menuId.ToString(), tag, prompt);
                }
                else
                {
                    Host.Children.Remove(host);
                }
            }

            if (CurrentPlugin >= 0 && Host.Children[CurrentPlugin] is NxPluginHost h)
                h.Visibility = Visibility.Visible;

            return CurrentPlugin >= 0;
        }

        public bool UnloadPlugin(string tag, WinRect screenRect)
        {
            for (int i = 0; i < Host.Children.Count; i++)
            {
                if (Host.Children[i] is NxPluginHost host && host.PluginName + host.EntryPoint == tag)
                {
                    CurrentPlugin = i;
                    return host.Execute(screenRect, "<<EXIT>>", string.Empty, string.Empty);
                }
            }
            return false;
        }

        //public bool OnValidated(object[] parameters)
        //{
        //    foreach (var child in Host.Children.OfType<NxPluginHost>())
        //        if (!child.OnValidated(parameters)) return false;
        //    return true;
        //}

        //public bool Load(int state, string property)
        //{
        //    foreach (var child in Host.Children.OfType<NxPluginHost>())
        //        if (!child.Load(state, property)) return false;
        //    return true;
        //}

        public bool Process()
        {
            foreach (var child in Host.Children.OfType<NxPluginHost>())
                if (!child.Process()) return false;
            return true;
        }

        public bool Execute(WinRect screenRect, string command, string parameters, string functionId)
        {
            if (Host == null) return false;
            return CurrentPlugin >= 0 && Host.Children[CurrentPlugin] is NxPluginHost host
                ? host.Execute(screenRect, command, parameters, functionId)
                : false;
        }

        public void Resize(WinSize WinSize)
        {
            if (Host == null) return;
            foreach (var child in Host.Children.OfType<NxPluginHost>())
                child.Resize(WinSize);
        }

        public bool Save()
        {
            if (Host == null) return false;
            return CurrentPlugin >= 0 && Host.Children[CurrentPlugin] is NxPluginHost host
                ? host.Save()
                : true;
        }

        public bool Show(string tag)
        {
            bool shown = false;
            if (Host == null) return shown;
            for (int i = 0; i < Host.Children.Count; i++)
            {
                if (Host.Children[i] is NxPluginHost host)
                {
                    bool match = host.PluginName + host.EntryPoint == tag;
                    host.Visibility = match ? Visibility.Visible : Visibility.Collapsed;
                    if (match)
                    {
                        CurrentPlugin = i;
                        shown = host.Show();
                    }
                }
            }
            return shown;
        }

        public void ScalePlugIn(double width, double height)
        {
            foreach (var host in Host.Children.OfType<NxPluginHost>())
                host.ScalePlugIn();
        }

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
    }
}