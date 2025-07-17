using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace Nexus
{
    public static class WindowHelper
    {
        public static Size GetWindowSize(Page page)
        {
            System.Diagnostics.Debug.WriteLine($"Uno Platform: {Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily}");

            #if __WINDOWS__
                var hwnd = WindowNative.GetWindowHandle(page);
                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);
                var sz = appWindow?.Size ?? new Windows.Graphics.SizeInt32 { Width = 800, Height = 600 };
                return new Size(sz.Width, sz.Height);
            #elif __WASM__ || __SKIA__ || __ANDROID__ || __IOS__
                        var bounds = Window.Current.Bounds;
                        return new Size(bounds.Width, bounds.Height);
            #else
                return new Size(800, 600); // Default fallback
            #endif
        }
    }
}
