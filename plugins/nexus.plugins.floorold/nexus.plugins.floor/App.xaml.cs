using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Uno.Resizetizer;

namespace nexus.plugins.floor
{
    public partial class App : Application
    {
        public static AppConfig Config { get; private set; } = new AppConfig();
        public IHost? Host { get; private set; }
        protected Window? MainWindow { get; private set; }

        public App()
        {
            this.InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Load appsettings.json and extract AppConfig section only
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///appsettings.json"));
            var buffer = await FileIO.ReadBufferAsync(file);
            using var stream = buffer.AsStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var doc = JsonDocument.Parse(json);
            var configText = doc.RootElement.GetProperty("AppConfig").GetRawText();
            Config = JsonSerializer.Deserialize<AppConfig>(configText) ?? new AppConfig();


            var builder = this.CreateBuilder(args)
                .Configure(host => host
#if DEBUG
                    .UseEnvironment(Environments.Development)
#endif
                    .UseLogging((context, logBuilder) =>
                    {
                        logBuilder.SetMinimumLevel(LogLevel.Information).CoreLogLevel(LogLevel.Warning);
                    }, enableUnoLogging: true)
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton(Config);
                    })
                );

            MainWindow = builder.Window;
#if DEBUG
            MainWindow.UseStudio();
#endif
            MainWindow.SetWindowIcon();

            var mainUI = new MainUI();
            MainWindow.Content = mainUI;
            MainWindow.Activate();
        }
    }
}