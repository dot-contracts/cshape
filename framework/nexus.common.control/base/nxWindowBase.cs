
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;



//#if WINDOWS
//using nexus.common.control.Interface;
//#if WINDOWS
//using Microsoft.UI;
//using Microsoft.UI.Windowing;
//using WinRT.Interop;
//#endif

////#if !WINDOWS && !__ANDROID__ && !__IOS__ && !__MACCATALYST__ && !__WASM__
////using Uno.WinUI.Runtime.Skia;
////#endif

namespace nexus.common.control
{
    public partial class NxWindowBase : Window, IWindow
    {

        public delegate void OnDialogEventHandler(); public new event OnDialogEventHandler OnChange;

        public bool IsLoaded       { get; }
        public bool IsChanged      { get; }
        public bool IsValid        { get; }
        public bool IsInitialized  { get; }
        public bool IsActive       { get; }

        public PropertyBag<NameValue> PropBag;

        public bool Load()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public static void SetWindowSize(int Width, int Height)
        {
            SetWindowSize(new System.Drawing.Size(Width, Height));
        }

        public static void SetWindowSize(System.Drawing.Size winSize)
        {
#if WINDOWS
            var hwnd = WindowNative.GetWindowHandle(Window.Current);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = winSize.Width, Height = winSize.Height });

#elif __SKIA__ && !__ANDROID__ && !__IOS__ && !__MACCATALYST__
            var skiaWindow = (Window.Current as ISkiaHost)?.Window;
            if (skiaWindow != null)
            {
                skiaWindow.Size = new SizeInt32(winSize.Width, winSize.Height);
            }
#endif
        }

        public static void SetWindowPosition(System.Drawing.Rectangle Position)
        {
#if WINDOWS
            var hwnd = WindowNative.GetWindowHandle(Window.Current);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Move(new Windows.Graphics.PointInt32 { X = Position.Left, Y = Position.Top });

#elif __SKIA__ && !__ANDROID__ && !__IOS__ && !__MACCATALYST__
            var skiaWindow = (Window.Current as ISkiaHost)?.Window;
            if (skiaWindow != null)
            {
                skiaWindow.Position = new PointInt32(Position.Left, Position.Top);
            }
#endif
        }

        public static void CenterWindow(System.Drawing.Size winSize)
        {
#if WINDOWS
            var hwnd = WindowNative.GetWindowHandle(Window.Current);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

            var centerX = displayArea.WorkArea.Width / 2  - winSize.Width / 2;
            var centerY = displayArea.WorkArea.Height / 2 - winSize.Height / 2;

            appWindow.Move(new Windows.Graphics.PointInt32 { X = centerX, Y = centerY });

#elif __SKIA__ && !__ANDROID__ && !__IOS__ && !__MACCATALYST__
            var skiaWindow = (Window.Current as ISkiaHost)?.Window;
            if (skiaWindow != null)
            {
                // Replace with actual screen size if available
                var screenWidth  = 1920;
                var screenHeight = 1080;

                var centerX = screenWidth / 2  - winSize.Width   / 2;
                var centerY = screenHeight / 2 - winSize..Height / 2;

                skiaWindow.Position = new PointInt32(centerX, centerY);
            }
#endif
        }
    }
}
