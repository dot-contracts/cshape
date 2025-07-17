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

using Windows.UI.Core;

using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.common.control;

namespace nexus.plugins.main
{
    /// <summary>
    /// Interaction logic for TopPanel.xaml
    /// </summary>
    public partial class TopPanel : NxControlBase 
    {

        private string mModuleId = "";

        public event OnTabSelectedEventHandler    OnTabSelected;    public delegate void OnTabSelectedEventHandler    (string PluginTag, string Prompt);
        public event OnActionSelectedEventHandler OnActionSelected; public delegate void OnActionSelectedEventHandler (string Command, string FunctionPk, System.Drawing.Rectangle ScreenRect);

        public TopPanel()
        {
            InitializeComponent();
        }

        public void Create(string ModuleId, string PluginTag, string Prompt)
        {
            // Load menu

            Reset();

            LoadActions(ModuleId);

            bool found = false;
            if (Tabs.Children.Count > 0)
            {
                for (int i = 0; i <= Tabs.Children.Count - 1; i++)
                {
                    PanelButton button = (PanelButton)Tabs.Children[i];
                    if ((button.Plugin).Equals(PluginTag)) found = true;
                }
            }

            if (!found)
            {
                PanelButton button   = new PanelButton();
                button.Prompt     = Prompt;
                button.Plugin     = PluginTag;
                button.Height     = 30;
                button.Width      = 100;

                //button.OnClicked  += Button_OnClicked;
                //button.OnPicClick += Button_OnPicClick;

                Tabs.Children.Add(button);
            }

        }

        public void LoadActions(string ModuleId)
        {
            Reset();

            mModuleId = ModuleId;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.ValidateUserAction", "I~I~ModuleId~" + mModuleId + ";I~I~WorkerId~" + WorkerCache.Instance.WorkerId());
                for (int i = 0; i <= DT.Rows.Count - 1; i++)
                {
                    if (DT.Rows[i]["FunctionType"].ToString().Equals("Taskbar Action"))
                    {
                        NxButton bt = new NxButton();
                        bt.Prompt     = DT.Rows[i]["Description"].ToString();
                        bt.Tag        = DT.Rows[i]["FunctionId"].ToString();
                        bt.Width      = 100;
                        bt.Height     = Actions.Height;
                        bt.Visibility = Visibility.Collapsed;
                        //bt.OnClicked += onSelected;

                        Actions.Children.Add(bt);
                    }
                }
            }

        }


        public void LoadActions(string ModuleId, string ActionList)
        {
            // Load menu

            mModuleId = String.IsNullOrEmpty(ModuleId) ? "" : ModuleId;

            if (!mModuleId.Equals(ModuleId) && !string.IsNullOrEmpty(mModuleId)) LoadActions(mModuleId); 
            else if (!String.IsNullOrEmpty(ActionList)) LoadActionList(ActionList); 
            else if ((Actions.Children.Count==0) & (!String.IsNullOrEmpty(ActionList))) LoadActionList(ActionList);

            foreach (NxButton bt in Actions.Children) bt.Visibility = Visibility.Collapsed;

            if (!String.IsNullOrEmpty(ActionList))
            {
                Array arr = ActionList.Split(';');

                foreach (NxButton bt in Actions.Children)
                {
                    for (int i = 0; i <= arr.GetLength(0) - 1; i++)
                    {
                        if (bt.Prompt.Equals(arr.GetValue(i).ToString()))
                        {
                            bt.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        public void LoadActionList(string ActionList)
        {

            Reset();

            Array arr = ActionList.Split(';');

            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                NxButton bt = new NxButton();
                bt.Prompt     = arr.GetValue(i).ToString();
                bt.Tag        = arr.GetValue(i).ToString();
                bt.Width      = 100;
                bt.Height     = Actions.Height;
                bt.Visibility = Visibility.Collapsed;
                //bt.OnClicked += onSelected;

                Actions.Children.Add(bt);
            }
        }

        private void Button_OnClicked(string Plugin, string Prompt, System.Drawing.Rectangle ScreenRect)
        {
            for (int i = 0; i <= Tabs.Children.Count - 1; i++)
            {
                PanelButton button = (PanelButton)Tabs.Children[i];
                button.BorderType = (button.Plugin.Equals(Plugin)) ? BorderTypes.OK : BorderTypes.Cancel;
                if (button.Plugin.Equals(Plugin))
                    OnTabSelected?.Invoke(Plugin, Prompt);
            }
        }

        private void Button_OnPicClick(string Plugin, System.Drawing.Rectangle ScreenRect)
        {
            for (int i = 0; i <= Tabs.Children.Count - 1; i++)
            {
                PanelButton button = (PanelButton)Tabs.Children[i];
                button.BorderType = (button.Plugin.Equals(Plugin)) ? BorderTypes.OK : BorderTypes.Cancel;
                if (button.Plugin.Equals(Plugin))
                    OnActionSelected?.Invoke("<<EXIT>>", Plugin, ScreenRect);
            }

        }

        private void onSelected(string Tag, string Prompt, System.Drawing.Rectangle ScreenRect)
        {
            OnActionSelected?.Invoke(Prompt, Tag, ScreenRect);
        }

        public void Reset()
        {

            if (Actions.Children.Count>0)
            {
                foreach (NxButton bt in Actions.Children)
                {
                    //bt.OnClicked -= onSelected;
                }
                    
                Actions.Children.Clear();
            }
        }

        public void RemoveMenu(string Plugin)
        {
            for (int i = 0; i <= Tabs.Children.Count - 1; i++)
            {
                PanelButton button = (PanelButton)Tabs.Children[i];
                if (button.Plugin.Equals(Plugin))
                {
                    //button.OnClicked  -= Button_OnClicked;
                    //button.OnPicClick -= Button_OnPicClick;
                    Tabs.Children.Remove(button);
                    return;
                }
            }

        }

    }
}
