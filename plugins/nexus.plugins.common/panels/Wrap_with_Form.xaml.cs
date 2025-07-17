using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using nexus.common.control;

namespace nexus.plugins.common
{
    public partial class WrapWithForm : ContentControl 
    {
        bool PanelVisible = false;

        public event EventHandler<ClickedEventArgs>?      OnMenuClicked;

        public WrapWithForm()
        {
            this.InitializeComponent();

            this.SizeChanged += GridWithForm_SizeChanged;
        }

        private void GridWithForm_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            RootPanel.Width  = this.ActualWidth;
            RootPanel.Height = this.ActualHeight;
        }

        public void AddButtons(string Buttons)
        {
            if (ButtonToolbar == null)
                return;

            ButtonToolbar.Children.Clear();

            var buttonLabels = Buttons.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!buttonLabels.Any())
            {
                ButtonToolbar.Visibility = Visibility.Collapsed;
                return;
            }

            // Estimate the width of the longest label using a rough character width multiplier
            int maxLength = buttonLabels.Max(s => s.Length);
            double charWidth = 8.5; // Tune this if your font is wider/narrower
            double buttonWidth = maxLength * charWidth + 24; // padding allowance

            foreach (var label in buttonLabels)
            {
                var button = new NxButton
                {
                    Prompt = label,
                    Width  = buttonWidth,
                    Margin = new Thickness(4, 0, 4, 0)
                };

                button.OnClicked += (s, e) =>
                {
                    bool valid = true;
                    OnMenuClicked?.Invoke(this, new ClickedEventArgs(label, valid,  new Windows.Foundation.Rect()));   
                };

                ButtonToolbar.Children.Add(button);
            }

            ButtonToolbar.Visibility = Visibility.Visible;
        }

        public void AddPanel(ContentControl panel)
        {
            if (Panel != null)
            {
                Panel.Children.Clear(); // optional: replace whatever was there
                Panel.Children.Add(panel);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ FormPanel is not defined.");
            }
        }   

        public void ClearWrapper()
        {
            Wrapper.Children.Clear(); // Clear all items in the wrapper `
        }

        public bool WrapItemExists(Func<ContentControl, bool> match)
        {
            if (Wrapper != null)
            {
                return Wrapper.Children
                    .OfType<ContentControl>()
                    .Any(match);
            }
            return false;
        }

        public void AddWrapItem(ContentControl item)
        {
            if (Wrapper != null)   Wrapper.Children.Add(item);
            else                   System.Diagnostics.Debug.WriteLine("❌ WrapPanel is not defined.");
        }

        public async Task AnimateForm(bool show, int ShowState=0) // showstate=0 for edit, 1 for create
        {
            if (FormColumn == null || FormPanel == null || Wrapper == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ One or more named elements are missing.");
                return;
            }

            const int durationMs = 300;
            const int steps = 10;
            var delay = durationMs / steps;
            var start = FormColumn.Width.Value;
            var end = show ? 425 : 0;
            var delta = (end - start) / steps;

            for (int i = 0; i <= steps; i++)
            {
                double newWidth = start + (delta * i);
                FormColumn.Width = new GridLength(newWidth, GridUnitType.Pixel);
                await Task.Delay(delay);
            }

            PanelVisible = show;
            if (PanelVisible)
            {
                FormPanel.Visibility = Visibility.Visible;
                Wrapper.Opacity = 0.4;
            }
            else
            {
                FormPanel.Visibility = Visibility.Collapsed;
                Wrapper.Opacity = 1.0;
            }
        }

        public void SetBackGrounds(ImageBrush brush)
        {
            foreach (var item in Wrapper)
            {
                NxPanelBase panelBase = item as NxPanelBase;
                panelBase.Background = brush; // Update existing background
            }

        }
    }
}