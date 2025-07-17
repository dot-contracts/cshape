
using Microsoft.UI;
//using Microsoft.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.core;
using nexus.common.dal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;

namespace nexus.plugins.main
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Login : NxGroupBase 
    {

        private System.Timers.Timer mTimer;
        private delegate void TimerCB();

        public event OnQuitEventHandler  OnQuit;   public delegate void OnQuitEventHandler(bool Cancel);

        public Login()
        {
            InitializeComponent();

            lbBad.Visibility = Visibility.Collapsed;

            base.OnProcessKey += ProcessKey;

            btLogin.OnClicked += BtLogin_OnClicked;

            mTimer = new System.Timers.Timer();
            mTimer.AutoReset = false;
            mTimer.Interval  = 1000;
            mTimer.Elapsed  += new System.Timers.ElapsedEventHandler(mTimer_Elapsed);
            mTimer.Start();

            Reset();

            AddControl((NxTextEdit)this.FindName((string)"txUsername"));
            AddControl((NxTextEdit)this.FindName((string)"txPassword"));

            var logoUri = new Uri("ms-appx:///Images/graphic.png");
            imgIG.Source = new BitmapImage(logoUri);

            var nextUri = new Uri("ms-appx:///Images/NextNet.png");
            imgNextNet.Source = new BitmapImage(nextUri);

        }

        public void Reset()
        {
            txPassword.Value = "";

        }
        public void Start()
        {
            mTimer.AutoReset = false;
            mTimer.Start();
        }

        private void _PreviewKeyUp(object sender, KeyEventArgs e)
        {
            lbBad.Visibility = Visibility.Collapsed;
        }

        public async void ProcessKey(string Key)
        {

            if (CurrentCtl == null) SetFocus("txUsername");

            switch (Key)
            {
                case "Enter":
                case "Tab":
                    CurrentCtl.Validate();

                    switch (KeyFocus)
                    {
                        case "txUsername":
                            MoveNext();
                            break;

                        case "txPassword":
                            await TryLogin();
                            break;
                    }
                    break;

                case "Back":
                    MovePrevious();
                    break;

                default:
                    CurrentCtl.ProcessKey(Key, true);
                    return;
            }

        }

        private async void BtLogin_OnClicked(object? sender, ClickedEventArgs e)
        {
            await TryLogin();
        }

        public async Task TryLogin()
        {
            var loginRequest = new LoginRequest
            {
                Username = txUsername.Value,
                Password = txPassword.Value
            };

            var loginService = new LoginService(); // Replace with the class that contains LoginAsync
            LoginResponse response = await loginService.LoginAsync(loginRequest);

            if (response != null)
            {
                if (ckRemember.IsSelected)
                {
                }
                Session.CurrentUser = response;

                OnQuit?.Invoke(false);

            }
            else ShowBad();
        }
        private void btCancel_OnClicked(string Tag, string Prompt, System.Drawing.Rectangle ScreenRect)  { OnQuit?.Invoke(true); }

        private void ShowBad()
        {
            lbBad.Visibility = Visibility.Visible;
            mTimer.Interval = 1000;
            mTimer.Start();
        }

        private void mTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(ProcessTimer);
        }
        private void ProcessTimer()
        {
            mTimer.Stop();

            if (mTimer.Interval.Equals(1000))
            {
                SetFocus("txUsername");
                mTimer.Interval = 3000;
                mTimer.Start();
            }
            else
            {
                mTimer.Stop();
                lbBad.Visibility = Visibility.Collapsed;
            }

        }

    }
}
