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

    public partial class piLogin : NxPluginBase
    {

        private Login mLogin;
        public event OnQuitEventHandler  OnQuit;   public delegate void OnQuitEventHandler(bool Cancel);

        public piLogin()
        {
            InitializeComponent();

            mLogin = new Login();
            mLogin.OnQuit  += doQuit;
            Panel.Children.Add(mLogin);
           
        }

        private void doQuit(bool Cancel)
        {
            OnQuit?.Invoke(Cancel); 
        }

        public bool Create()
        {
            return true;
        }

        #region Override

        public new bool Load(int LoadState, string Property)
        {
            return false;
        }
        public new void Reset()
        {
            mLogin.Reset();
        }
        public new bool Save()
        {
            return false;
        }
        public new bool Process()
        {
            return false;
        }
        public new bool Execute(string Command, string Parameters)
        {
            return false;
        }

        #endregion



    }
}
