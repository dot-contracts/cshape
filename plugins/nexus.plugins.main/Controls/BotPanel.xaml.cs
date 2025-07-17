using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;

using Windows.System;


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
using nexus.common.control;
using Windows.UI.Core;

namespace nexus.plugins.main
{
    /// <summary>
    /// Interaction logic for TopPanel.xaml
    /// </summary>
    public partial class BotPanel : NxControlBase 
    {

        public event OnInfoEventHandler OnInfo; public delegate void OnInfoEventHandler();

        public BotPanel()
        {
            InitializeComponent();

            btInfo.OnClicked += BtInfo_OnClicked;   
        }

        private void BtInfo_OnClicked(object? sender, ClickedEventArgs e)
        {
            OnInfo?.Invoke();
        }

        public void UpdateInfo(string info) { lbInfo.Prompt = info; }
    }
}
