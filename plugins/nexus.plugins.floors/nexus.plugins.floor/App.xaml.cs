using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace nexus.plugins.floor
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new Window();
            window.Content = new MainUI();
            window.Activate();
        }
    }
}
