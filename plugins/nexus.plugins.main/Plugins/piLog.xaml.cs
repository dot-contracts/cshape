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
using nexus.common.dto;
using nexus.common.control;

namespace nexus.plugins.main
{

    public partial class piLog : NxPluginBase
    {

        private DateTime mCreated;
        string GridType = "Events";

        public piLog()
        {
            InitializeComponent();

            this.GotFocus  += PiLog_GotFocus;
            this.LostFocus += PiLog_LostFocus;
            
            //btUpdate.OnClicked    += BtUpdate_OnClicked;
            Tabs.SelectionChanged += Tabs_SelectionChanged;
        }


        private void PiLog_LostFocus(object sender, RoutedEventArgs e)
        {
            LogGrid.ClearDataTable();
        }

        private void PiLog_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowData();
        }

        public bool Create()
        {
            mCreated = DateTime.Now;

            try
            {
                txVenue.Value    = shell.Instance.Venue + ", " + shell.Instance.VenueID;
                txLocation.Value = shell.Instance.ComputerName + ", " + shell.Instance.ComputerId;
            }
            catch (Exception ex) { string b = ex.Message; }

            System.Version Vers = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            txVersion.Value = Vers.Major.ToString() + "." + Vers.Minor.ToString() + "." + Vers.Build.ToString() + "." + Vers.Revision.ToString("0");


            //ShowData();
 
          //  Human.Instance.OnMessage     += Human_OnMessage;
            LoggerCache.Instance.NewData += Logger_NewData;
            EventCache.Instance.NewData  += Event_NewData;



            return true;
        }



        private void Human_OnMessage(enums.MessageTypes MessageType, string Message, bool Logit)
        {
            LoggerCache.Instance.InsertLogEntry(MessageType.ToString().Contains("Error"), "Human", "", "Message", Message);
        }


        private void Logger_NewData()
        {
            //ShowData();
            //LogGrid.SetDataTable(LoggerCache.Instance.LogData);
        }
        private void Event_NewData()
        {
            //ShowData();
            //LogGrid.SetDataTable(LoggerCache.Instance.LogData);
        }


        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TabViewItem ti = (TabViewItem)Tabs.SelectedItem;
                GridType = ti.Header.ToString();
                ShowData();
            }
            catch (System.Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void ShowData()
        {
            if (DateTime.Now.Subtract(mCreated).TotalSeconds >30)
            {
                switch (GridType)
                {
                    case "Events": LogGrid.SetDataTable(EventCache.Instance.LogData);  break;
                    case "Logs":   LogGrid.SetDataTable(LoggerCache.Instance.LogData); break;
                }
            }
        }

        private void BtUpdate_OnClicked(string Tag, string Prompt, System.Drawing.Rectangle ScreenRect) { Update(); }

        public void Update()
        {
            //mLoader.TextOutput("Startup", "Checking NextHub", false);
            Processes.ProcStates state = Processes.ProcStates.NotInstalled;

            LoggerCache.Instance.InsertLogEntry(enums.MessageTypes.NoPage.ToString().Contains("Error"), "piLog Update", "", "Message", "Initiating Updater");

            using (Updater up = new Updater())
            {
                if (up.Process("NextUpdate", true))
                {
                    LoggerCache.Instance.InsertLogEntry(enums.MessageTypes.NoPage.ToString().Contains("Error"), "piLog Update", "", "Message", "Updater Processed");

                    using (Processes proc = new Processes())
                    {
                        if (proc.AppState("NextUpdate").Equals(Processes.ProcStates.NotInstalled))
                        {
                            LoggerCache.Instance.InsertLogEntry(enums.MessageTypes.NoPage.ToString().Contains("Error"), "piLog Update", "", "Message", "Installing NextUpdate");
                            up.Process("NextUpdate", true);
                        }
                        proc.StartApp(Package: @"c:\nexus\programs\nextupdate\nextupdate.exe", Arguments: System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
                    }
                }
                else
                {
                    LoggerCache.Instance.InsertLogEntry(enums.MessageTypes.NoPage.ToString().Contains("Error"), "piLog Update", "", "Message", "Failed to Initiate Updater");
                }
            }


        }

        #region Override

        public new bool Load(int LoadState, string Property)
        {
            return false;
        }
        public new void Reset()
        {
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
