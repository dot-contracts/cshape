using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace nexus.common

{
    static public partial class dialoghelpers
    {
        public static async Task ShowMessageAsync(XamlRoot xamlRoot,string message, string title = "Message")
        {
            var dialog = new ContentDialog
            {
                Title   = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = xamlRoot
            };

            await dialog.ShowAsync();
        }


        public static WinRect ScreenRect(FrameworkElement element)
        {
            var transform = element.TransformToVisual(null);
            WinPoint topLeft = transform.TransformPoint(new WinPoint(0, 0));

            return new WinRect((double)topLeft.X, (double)topLeft.Y, (double)element.ActualWidth, (double)element.ActualHeight );
        }

        public static WinPoint GetDialogLocation(FrameworkElement hostDialog, FrameworkElement anchorElement)
        {
            WinRect controlRect = ScreenRect(anchorElement);
            WinRect windowBounds = Window.Current.Bounds;

            double dialogWidth = hostDialog.ActualWidth;
            double dialogHeight = hostDialog.ActualHeight;

            double x = controlRect.X;
            double y = controlRect.Y + controlRect.Height;

            // Clamp horizontally
            if (x + dialogWidth > windowBounds.Right)
                x = windowBounds.Right - dialogWidth;
            if (x < windowBounds.Left)
                x = windowBounds.Left;

            // Clamp vertically
            if (y + dialogHeight > windowBounds.Bottom)
                y = controlRect.Y - dialogHeight;
            if (y < windowBounds.Top)
                y = windowBounds.Top;

            return new WinPoint(x, y);
        }

    }

}
