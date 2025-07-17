using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Windows.Foundation;

namespace Nexus
{
    public sealed partial class MainPage : Page
    {
        private int mLoadState = 0;
        private bool mIsBusy = false;
        private int frameRate = 5;

        private DispatcherTimer mTimer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Unloaded += StartUp_Unloaded;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            //if (!setting.GetSettings(await GetIDAsync()))   need GetIDAsync to work
            //if (!setting.GetSettings())
            //{
            //    // TODO: Show diagnostics page
            //}
            //else
            {
                mTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(50)
                };
                mTimer.Tick += (s, e) => ProcessTimer();
                mTimer.Start();
            }
        }

        public async Task<string> GetIDAsync()
        {
            return "";//    await App.DeviceIdProvider.GetDeviceIdAsync();
        }

        private void ProcessTimer()
        {
            mTimer.Stop();

            if (!mIsBusy)
            {
                mIsBusy = true;
                try
                {
                    switch (mLoadState)
                    {
                        case 0:
                            mLoadState++;
                            break;

                        case 1:

                            //msg.OnPluginEvent += OnPluginEvent;

                            MainUI.Width = this.ActualWidth;
                            MainUI.Height = this.ActualHeight;

                            //Host.Resize(new Size(Host.ActualWidth, Host.ActualHeight));//     WindowHelper.GetWindowSize(this));
                            //Host.LoadPlugin(DefaultModule, DefaultEntry);
                            //else
                            //    LoggerCache.Instance.InsertLogEntry(true, "StartUp", "LoadPlugin", "Exception", $"Error Loading {DefaultModule}: {Host.ErrorMessage}");

                            mLoadState++;
                            break;

                        case 2:
                            //shell.Instance.OnWorkerChange += OnWorkerChange;
                            mLoadState = 20;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    //LoggerCache.Instance.InsertLogEntry(true, "StartUp", "ProcTimer", "Exception", $"LoadState {mLoadState} {ex.Message}");
                    mLoadState++;
                }

                mIsBusy = false;
            }

            if (mLoadState < 3 || (mLoadState > 3 && mLoadState < 20))
                mTimer.Start();
        }

        private void StartUp_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveState();
        }

        private void OnWorkerChange(int WorkerRoleID)
        {
            // handle role change
        }

        //    private void OnPluginEvent(enums.EventTypes reason, string sender, string command, string property)
        //    {
        //        if (command.Equals("Exit"))
        //        {
        //#if WINDOWS
        //            Windows.ApplicationModel.Core.CoreApplication.Exit();
        //#else
        //            Environment.Exit(0);
        //#endif
        //            SaveState();
        //        }
        //        else if (command.Equals("SetState") && !string.IsNullOrEmpty(property))
        //        {
        //            var arr = (property + ";;;;;").Split(';');
        //            // Optionally resize/move via platform APIs
        //        }
        //        else if (command.Equals("KioskMode"))
        //        {
        //            // Kiosk mode logic (e.g. hide header/footer controls)
        //        }
        //        else if (command.Equals("Logout"))
        //        {
        //            // Handle logout
        //        }
        //    }

        public void SaveState()
        {
            _ = DispatcherQueue.TryEnqueue(() => _SaveState());
        }

        private void _SaveState()
        {
            var size = WindowHelper.GetWindowSize(this);
            string parameters = $"Normal;0;0;{size.Width};{size.Height}";
            //Host.Execute(new System.Drawing.Rectangle(), "SaveState", parameters, "");
        }
    }
}

