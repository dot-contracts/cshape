using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

using Windows.System;
using Windows.Foundation;
using Windows.UI.Core;

using Microsoft.UI;
//using Microsoft.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;

using nexus.common;
using nexus.common.cache;
using nexus.common.core;
using nexus.common.dal;
using nexus.common.control;

namespace nexus.plugins.main
{
    /// <summary>
    /// Interaction logic for NoConnect.xaml
    /// </summary>
    public partial class NoConnect : NxGroupBase
    {

        private Boolean mConnecting = false;
        private DateTime mLastConnect = DateTime.Now.AddDays(-1);

        private delegate void CreateCB();
        private delegate void CloseMeCB();
        private Boolean mChanged = false;

        public event OnCloseEventHandler OnClose;
        public delegate void OnCloseEventHandler();

        public event OnQuitEventHandler OnQuit;
        public delegate void OnQuitEventHandler();


        public NoConnect()
        {
            InitializeComponent();

            btQuit.Click += new RoutedEventHandler(btQuit_Click);
            btTest.Click += new RoutedEventHandler(btTest_Click);
            btUpdate.Click += new RoutedEventHandler(btUpdate_Click);

            //txtServer.PreviewKeyUp  += new KeyEventHandler(KeyDown);
            //txtPort.PreviewKeyUp    += new KeyEventHandler(KeyDown);
            //txtConnect.PreviewKeyUp += new KeyEventHandler(KeyDown); 

        }

        public void Create()
        {

            //txtServer.Text  = shell.Instance.LAN.Server;
            txtPort.Text    = shell.Instance.LAN.ServerPort;
            txtConnect.Text = shell.Instance.LAN.Connect;
        }

        public bool Process()
        {

            bool Connected = false;

            if (!mConnecting)
            {
                Single NextAtt = 60 - (Single)DateTime.Now.Subtract(mLastConnect).TotalSeconds;
                if (NextAtt > 0)
                {
                    lbTimer.Text = "Please wait (" + NextAtt.ToString("0") + ")";
                }
                else
                {
                    mConnecting = true;
                    mLastConnect = DateTime.Now;
                    Settings.Opacity = 0.35;

                    try
                    {
                        AddMessage("trying to connect " + shell.Instance.LAN.Server + "," + shell.Instance.LAN.ServerPort);
                        if (shell.Instance.LAN.CanConnect()) { }
                            //this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new CloseMeCB(CloseMe));
                        else
                            AddMessage(shell.Instance.LAN.ConnectError); 
                    }
                    catch (Exception ex) {AddMessage("connect error " + ex.Message);}

                    mConnecting = false;
                }
            }

            return Connected;

        }


        void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (btUpdate.Content.Equals("Edit Endpoint"))
            {
                btUpdate.Content = "Update endpoint";
                Settings.Opacity = 1;
            }
            else
            {
                btUpdate.Content = "Edit endpoint";
                Settings.Opacity = 0.35;

                AddMessage("Endpoint settings saved");
                mLastConnect.AddDays(-1);
            }
        }



        void btTest_Click(object sender, RoutedEventArgs e)
        {
            shell.Instance.LAN.Server = txtServer.Text;
            shell.Instance.LAN.ServerPort = txtPort.Text;
            shell.Instance.LAN.Connect = txtConnect.Text;
            mLastConnect = mLastConnect.AddDays(-1);
        }
        void KeyDown(object sender, KeyEventArgs e)
        {
            Settings.Opacity = 1;
            mChanged = true;
        }
        void btQuit_Click(object sender, RoutedEventArgs e) 
        {
            CloseMe();
            if (OnQuit != null) OnQuit();
        }

        private void CloseMe()
        {
            if (mChanged) shell.Instance.Save();
        }

        private void AddMessage(string message)
        {
            if (LBox.Items.Count > 255)
            {
                do
                { if (LBox.Items.Count > 0) LBox.Items.RemoveAt(LBox.Items.Count - 1); }
                while (LBox.Items.Count > 255);
            }
            LBox.Items.Insert(0, message);
        }

    }
}
