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
    public partial class PanelButton : NxControlBase 
    {

        public string      Plugin     { get; set; }
        public string      Prompt     { get { return Button.Prompt; }     set { Button.Prompt     = value; } }
        public BorderTypes BorderType { get { return Button.BorderType; } set { Button.BorderType = value; } }


        public event OnClickedEventHandler   OnClicked;  public delegate void OnClickedEventHandler  (string Tag, string Prompt, Windows.Foundation.Rect ScreenRect);
        public event OnPicClickEventHandler  OnPicClick; public delegate void OnPicClickEventHandler (string Tag, Windows.Foundation.Rect ScreenRect);

        public PanelButton()
        {
            InitializeComponent();

            Button.OnClicked += Button_OnClicked;
            //Image.OnClicked  += Image_OnClicked;

        }

        //{
        //    OnPicClick?.Invoke(Plugin, ScreenRect(this));
        //}

        private void Button_OnClicked(object? sender, ClickedEventArgs e)
        {
            OnClicked?.Invoke(Plugin, Prompt, e.ScreenRect);
        }
    }

}
