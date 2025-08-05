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
    public partial class GridWithForm : ContentControl 
    {
        bool PanelVisible = false;

        public IReadOnlyList<object> SourceData => Results.SourceData;

        public class ShowingPanelEventArgs
        {
            public int    DataRowIndex  { get; set; }
            public string Display       { get; set; }
            public string Value         { get; set; }
            public bool   Valid         { get; set; } = true;

            public ShowingPanelEventArgs(int DataRowIndex, string Display, string Value)
            {
                this.DataRowIndex = DataRowIndex;
                this.Display      = Display;
                this.Value        = Value;
            }
        }
        public class ShowPanelEventArgs
        {
            public int    DataRowIndex  { get; set; }
            public string Display       { get; set; }
            public string Value         { get; set; }

            public ShowPanelEventArgs(int DataRowIndex, string Display, string Value)
            {
                this.DataRowIndex = DataRowIndex;
                this.Display      = Display;
                this.Value        = Value;
            }
        }

        public event EventHandler<ClickedEventArgs>?      OnMenuClicked;
        public event EventHandler<ShowingPanelEventArgs>? OnShowingPanel;
        public event EventHandler<ShowPanelEventArgs>?    OnShowPanel;
        public event ChangedEventHandler?                 OnDataRowChanged; public delegate void ChangedEventHandler(int dataRowIndex, string Display, string Value);



        public GridWithForm()
        {
            this.InitializeComponent();

            this.SizeChanged += GridWithForm_SizeChanged;

            Results.OnDataRowChanged   += Results_OnDataRowChanged;
            Results.OnRowDoubleClicked += Results_OnRowDoubleClicked;
        }

        private void GridWithForm_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            RootPanel.Width  = this.ActualWidth;
            RootPanel.Height = this.ActualHeight;
        }


        private void Results_OnDataRowChanged(int dataRowIndex, string Display, string Value)
        {
            OnDataRowChanged?.Invoke(dataRowIndex, Display, Value);
        }

        private async void Results_OnRowDoubleClicked(int dataRowIndex, string Display, string Value)
        {
            var args = new ShowingPanelEventArgs(dataRowIndex, Display, Value);
            OnShowingPanel?.Invoke(this, args);

            if (args.Valid)
                OnShowPanel?.Invoke(this, new ShowPanelEventArgs(dataRowIndex, Display, Value));

        }

        public void SetItemsSource<T>(List<T> source, List<FieldDefinition> fieldDefinitions)
        {
            Results.SetItemsSource(source, fieldDefinitions);
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

        public async Task AnimateForm(bool show, int ShowState = 0) // showstate=0 for edit, 1 for create
        {
            if (FormColumn == null || FormPanel == null || Results == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ One or more named elements are missing.");
                return;
            }

            const int durationMs = 300;
            const int steps = 10;
            var delay = durationMs / steps;
            var start = FormColumn.Width.Value;
            //var end = show ? ActualWidth * 0.5 : 0;
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
                Results.IsEnabled = false;
                Results.Opacity = 0.4;
            }
            else
            {
                FormPanel.Visibility = Visibility.Collapsed;
                Results.IsEnabled = true;
                Results.Opacity = 1.0;
            }
        }

        //private async void BtUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    // Implement your update logic here
        //    // await AnimateForm(false);
        //    // LoadPromotions();
        //}

        private async void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            await AnimateForm(false);
            var args = new CloseEventArgs("");
           //OnClose?.Invoke(this, args);
        }

        //private void Results_OnChanged(object sender, EventArgs e)
        //{
        //    // Optional selection handling
        //}

        //// Optional methods that were mentioned in the errors — dummy stubs to avoid errors
        //public void Initialize() { }
        //public void Update() { }
        //public void UpdateResources() { }
        //public void StopTracking() { }
        //public void NotifyXLoad(string s) { }
    }
}