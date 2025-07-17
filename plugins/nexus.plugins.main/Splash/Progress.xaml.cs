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
    public partial class Progress : NxGroupBase
    {
        public Progress()
        {
            InitializeComponent();
        }


        public void Create()
        {

        }

        public async void AddMessage(string message)
        {
            LBox.Items.Insert(0, message);

            // Yield to the UI thread to allow rendering
            await Task.Yield();
        }

        public async void DoProgress(int value, string message)
        {
            try
            {
                ProgBar.Value = value;
            }
            catch (Exception)
            {
                ProgBar.Maximum = value + 10;
                ProgBar.Value = value;
            }

            LBox.Items.Insert(0, message);

            // Let UI render this change before continuing
            await Task.Yield();
        }

    }
}
