using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace nexus.common.control
{
    public class NxPopup
    {

        public UIElement Content
        {
            get => (UIElement)((ContentPresenter)((Border)_popup.Child).Child).Content;
            set => ((ContentPresenter)((Border)_popup.Child).Child).Content = value;
        }

        private readonly Popup _popup;

        public NxPopup()
        {
            _popup = new Popup
            {
                IsLightDismissEnabled = true,
                Child = new Border
                {
                    Background      = new SolidColorBrush(Colors.White),
                    Padding         = new Thickness(10),
                    BorderBrush     = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(1),
                    Child = new ContentPresenter
                    {
                        Name = "PopupContentHost"
                    }
                }
            };
        }

        public void ShowBelow(WinRect anchorRect)
        {
            _popup.HorizontalOffset = anchorRect.X;
            _popup.VerticalOffset   = anchorRect.Y + anchorRect.Height;
            _popup.IsOpen = true;
        }

        public void Close() => _popup.IsOpen = false;


        public void SetContent(UIElement content)
        {
            if (_popup.Child is Border border)
            {
                border.Child = content;
            }
        }
    }

}
