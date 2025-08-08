using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

using nexus.common;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxDialogBase: Control, IDisposable
    {

        //public string Tag { get; set; } = string.Empty; 

        private readonly Popup _popup;
        private readonly Border _container;
        private readonly ContentPresenter _contentHost;
        private readonly ScrollViewer _scrollViewer;

        public event EventHandler<ChangeEventArgs>  OnChange;
        public event EventHandler<ChangedEventArgs> OnChanged;

        public NxDialogBase()
        {
            _contentHost = new ContentPresenter { Padding = new Thickness(0) };

            _scrollViewer = new ScrollViewer
            {
                Content = _contentHost,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            _container = new Border
            {
                Background = NxThemeManager.Current.PanelBack.Brush,
                BorderBrush = NxThemeManager.Current.Border.Brush,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(8),
                Child = _scrollViewer
            };

            _popup = new Popup
            {
                Child = _container,
                IsLightDismissEnabled = true
            };

            _scrollViewer.ViewChanged += OnScrollChanged;
            _scrollViewer.Loaded += OnScrollChanged;
        }

        public UIElement Content
        {
            get => (UIElement)_contentHost.Content;
            set => _contentHost.Content = value;
        }

        public DataTemplate ContentTemplate
        {
            get => _contentTemplate;
            set
            {
                _contentTemplate = value;
                if (_contentTemplate != null)
                {
                    Content = (UIElement)_contentTemplate.LoadContent();
                }
            }
        }
        private DataTemplate _contentTemplate;

        public static Rect GetShowRect(UIElement anchorElement)
        {
            var root = (FrameworkElement)Microsoft.UI.Xaml.Window.Current.Content;
            return anchorElement.TransformToVisual(root).TransformBounds(new Rect(0, 0, ((FrameworkElement)anchorElement).ActualWidth, ((FrameworkElement)anchorElement).ActualHeight));
        }

        public void ShowUnder(FrameworkElement anchorElement)
        {
            if (anchorElement == null)
            {
                Show();
                return;
            }

            var rect = GetShowRect(anchorElement);
            ShowBelow(rect);
        }

        public void ShowBelow(Rect anchor)
        {
            var root = (FrameworkElement)Microsoft.UI.Xaml.Window.Current.Content;

            if (_popup.XamlRoot == null)
                _popup.XamlRoot = root.XamlRoot;

            // Measure the container to get proper size
            _container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            
            double windowHeight = root.ActualHeight;
            double windowWidth = root.ActualWidth;

            double containerHeight = _container.DesiredSize.Height;
            double containerWidth = _container.DesiredSize.Width;

            double spaceBelow = windowHeight - (anchor.Y + anchor.Height) - 32;
            double spaceAbove = anchor.Y - 32;
            double maxAvailableHeight = Math.Max(spaceBelow, spaceAbove);

            _scrollViewer.MaxHeight = maxAvailableHeight;

            double targetX = anchor.X;
            double targetY;
            
            // Position below if there's enough space, otherwise above
            if (spaceBelow >= containerHeight || spaceBelow >= spaceAbove)
            {
                targetY = anchor.Y + anchor.Height + 4;
            }
            else
            {
                targetY = anchor.Y - containerHeight - 4;
            }

            // Ensure the dialog stays within screen bounds
            if (targetX + containerWidth > windowWidth)
                targetX = windowWidth - containerWidth - 10;
            if (targetX < 0)
                targetX = 10;

            System.Diagnostics.Debug.WriteLine($"[DEBUG] ShowBelow - Anchor: ({anchor.X}, {anchor.Y}, {anchor.Width}, {anchor.Height})");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ShowBelow - Target: ({targetX}, {targetY})");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ShowBelow - Container Size: ({containerWidth}, {containerHeight})");

            _popup.HorizontalOffset = targetX;
            _popup.VerticalOffset = targetY;
            _popup.IsOpen = true;
        }

        public void Show()
        {
            if (_popup.XamlRoot == null)
            {
                var root = (FrameworkElement)Window.Current.Content;
                _popup.XamlRoot = root.XamlRoot;
            }

            _popup.HorizontalOffset = 100;
            _popup.VerticalOffset = 100;
            _popup.IsOpen = true;
        }

        public void Close()
        {
            _popup.IsOpen        = false;
            _contentHost.Content = null;
            _popup.Child         = null;
        }


        public bool RaiseOnChange(string Value)
        {
            var args = new ChangeEventArgs("", Value, Value);
            OnChange?.Invoke(this, args);
            return args.Valid;
        }

        public void RaiseOnChanged(string Value)
        {
            var rect = new Windows.Foundation.Rect(0, 0, 0, 0); // Default empty rect for dialog
            OnChanged?.Invoke(this, new ChangedEventArgs(Tag?.ToString() ?? "", Value, Value, rect));
        }

        private void UpdateContentPadding()
        {
            bool verticalScrollVisible = _scrollViewer.ScrollableHeight > 0;
            _contentHost.Padding = verticalScrollVisible ? new Thickness(0, 0, 14, 0) : new Thickness(0);
        }

        private void OnScrollChanged(object sender, RoutedEventArgs e)
        {
            UpdateContentPadding();
        }

        private void OnScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateContentPadding();
        }
    }

}
