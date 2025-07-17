using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using nexus.common.control;
using System;
using System.Threading.Tasks;

namespace nexus.common.control
{
    public sealed partial class NxMessageBox : NxDialogBase, IAsyncDisposable
    {
        public enum NxMessageBoxResult { None, Yes, No, Cancel, OK }

        private TaskCompletionSource<NxMessageBoxResult>? _tcs;
        private NxButton btOK;
        private NxButton btClose;

        public NxMessageBox(string title, string message)
        {
            this.InitializeComponent();

            // Build layout
            var layout = new StackPanel
            {
                Spacing = 16,
                Padding = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var titleText = new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var messageText = new TextBlock
            {
                Text = message,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            btOK = new NxButton
            {
                Prompt = "OK",
                Width = 80
            };
            btClose = new NxButton
            {
                Prompt = "Cancel",
                Width = 80
            };

            btOK.OnClicked += OnOKClicked;
            btClose.OnClicked += OnCloseClicked;

            var buttonRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8
            };
            buttonRow.Children.Add(btOK);
            buttonRow.Children.Add(btClose);

            layout.Children.Add(titleText);
            layout.Children.Add(messageText);
            layout.Children.Add(buttonRow);

            this.Content = layout;
        }

        private void OnOKClicked(object sender, ClickedEventArgs e)
        {
            this.Close(); // Hides the popup
            _tcs?.TrySetResult(NxMessageBoxResult.Yes);
        }

        private void OnCloseClicked(object sender, ClickedEventArgs e)
        {
            this.Close(); // Hides the popup
            _tcs?.TrySetResult(NxMessageBoxResult.Cancel);
        }

        public async Task<NxMessageBoxResult> ShowAsync(FrameworkElement? anchor = null)
        {
            _tcs = new TaskCompletionSource<NxMessageBoxResult>();

            if (anchor != null)
                this.ShowBelow(GetShowRect(anchor));
            else
                this.Show();

            var result = await _tcs.Task;
            return result;
        }

        public ValueTask DisposeAsync()
        {
            btOK.OnClicked -= OnOKClicked;
            btClose.OnClicked -= OnCloseClicked;
            return ValueTask.CompletedTask;
        }
    }
}
