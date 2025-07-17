
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;

using Microsoft.UI;
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

using nexus.shared.common;
using nexus.plugins.common;

namespace nexus.plugins.main
{

    public sealed partial class MainUI : NxMainUIBase, IMainUI
    {

        private BotPanel    m_BotPanel;
        private TopPanel    m_TopPanel;
        private piLogin     m_Login;
        private piLog       m_Log;
        private NxMenuItem  m_MenuItem;

        private const string txt_hide = "Hide Menu";
        private const string txt_show = "Show Menu";

        private string m_DefaultPrompt = "";
        private string m_DefaultPlugin = "";
        private string m_DefaultEntry  = "MainUI";
        private string m_DefaultSnapin = "StartUp";
        private int    m_ModuleId = 0;
        private int    m_FunctionId = 0;

        private delegate bool ProcessCommandCB(string Data);
        private delegate void CreateCB();
        private delegate void ShowPanelCB(int MemberID);

        public MainUI()
        {
            this.InitializeComponent();

            MaxLoadLoops = 20;

            //shell.Instance.PropBin.Add(new NameValue("MemberId", ""));

            base.OnLoaded += MainUI_OnLoaded;

            this.Loaded       += MainUI_Loaded;
            base.LoadLoop     += MainUI_LoadLoop;
            base.LoopFinished += MainUI_LoopFinished;
        }

        private void MainUI_OnLoaded()
        {

        }

        private void MainUI_Loaded(object sender, RoutedEventArgs e) {  TimerStart(20); }

        private async void MainUI_LoadLoop()
        {

            switch (LoadState)
            {
                case 0:
                    Task.Run(async () => await InitStartupAsync());

                    PluginHost.OnLoadButton  += PluginHost_OnLoadButton;
                    PluginHost.OnStartLoad   += PluginHost_OnStartLoad;
                    PluginHost.OnProgress    += PluginHost_OnProgress;
                    PluginHost.OnLoaded      += PluginHost_OnLoaded;

                    msg.OnPluginEvent        += PluginHost_OnPluginEvent;

                    m_TopPanel = new TopPanel();
                    TopBar.Children.Add(m_TopPanel);
                    m_TopPanel.Margin = new Thickness(142, 0, 0, 0);

                    m_BotPanel = new BotPanel();
                    BotBar.Children.Add(m_BotPanel);
                    m_BotPanel.Margin = new Thickness(142, 0, 0, 0);
                    m_BotPanel.OnInfo += OnInfo;

                    m_Log = new piLog();
                    Panel.Children.Add(m_Log);
                    m_Log.Visibility = Visibility.Collapsed;
                    m_Log.Create();


                    // need a common service which allows for users to log on through face or rfid

                    //Human.Instance.Create(shell.Instance.ComputerID);
                    //Human.Instance.OnValidated  += OnValidated;

                    LoadState++;
                    break;

                case 1: // connect to SignalR instead
                    //tcpClient.Create("Main", "man","man123");
                    LoadState++;
                    break;

                case 2:
                    btnMenu.OnClicked           += MenuClicked;
                    btnQuit.OnClicked           += QuitClicked;
                    SideMenu.OnMenuItemClicked  += OnMenuItemClicked;
                    //m_TopPanel.OnActionSelected += TopPanel_OnSelected;
                    m_TopPanel.OnTabSelected    += TopPanel_OnTabSelected;
                    LoadState++;
                    break;

                case 3: // load any default module based on look up
                    LoadState++;
                    break;

                case 4:

                    m_DefaultPlugin = "http://localhost:3342";
                    m_DefaultEntry  = "";

                    if (!String.IsNullOrEmpty(m_DefaultPlugin))
                        MenuItemClicked(m_DefaultPrompt, m_DefaultPlugin, m_DefaultEntry, m_DefaultSnapin, m_ModuleId, m_FunctionId);

                    LoadState++;
                    break;

                default:
                    LoadState = MaxLoadLoops;
                    break;
            }
        }
        private void MainUI_LoopFinished()
        {
            if (Session.CurrentUser == null) ShowLogin();
        }

        public async Task InitStartupAsync()
        {
            using (OptionHelper opt = new OptionHelper())
            {
                await opt.GetOptions(OptionPath: "Startup.State");

                string Params = opt.GetValue("State", "");
                if (!string.IsNullOrEmpty(Params))
                    msg.RaisePluginEvent(enums.EventTypes.command, "", "SetState", Params);
            }
        }

        private void Msg_OnPluginEvent(enums.EventTypes reason, string sender, string command, string Property)
        {
            throw new NotImplementedException();
        }

        private void OnInfo()
        {
            if (PluginHost.Visibility == Visibility.Visible)
                ShowPanel("Log");
            else
                HidePanel();

        }

        private void ShowPanel(string PanelName)
        {
            m_TopPanel.LoadActions("");
            for (int i = 0; i <= Panel.Children.Count - 1; i++)
                Panel.Children[i].Visibility = Visibility.Collapsed;

            PluginHost.Visibility = Visibility.Collapsed;

            switch (PanelName)
            {
                case "Log":   m_Log.Visibility   = Visibility.Visible; break;
            }
        }
        private void HidePanel()
        {
            for (int i = 0; i <= Panel.Children.Count - 1; i++)
                Panel.Children[i].Visibility = Visibility.Collapsed;

            PluginHost.Visibility = Visibility.Visible;
        }


        private async void ShowLogin()
        {

            //Schedule sche = new Schedule() { StartsOn = "2023-10-01", StartsAt = "08:00", FinishesOn = "2023-10-10", FinishesAt = "17:00", Hours = "", Days = "" };

            //var dlg = new DlgSchedule(sche);
            //dlg.XamlRoot = this.XamlRoot;
            //ContentDialogResult result = await dlg.ShowAsync();

            //if (result == ContentDialogResult.Primary)
            //{
            //    //Schedule updated = dlg.Result;
            //    // Do something with updated
            //}



            if (m_Login==null)
            {
                m_Login = new piLogin();
                m_Login.OnQuit  += OnQuit;
                Panel.Children.Add(m_Login);
            }
            m_Login.Reset();
            m_Login.Visibility = Visibility.Visible;

        }

        private void Login_OnChangedLogin()
        {
            btnMenu.IsReadOnly = false;
            SetMenuVisbility(true);
        }

        private void Login_OnChangedLogout()
        {
            btnMenu.IsReadOnly = true;
            SetMenuVisbility(false);
            ShowLogin(); 
        }

        private new void OnQuit(bool Cancel)
        {
            if (Cancel)
            {
                msg.RaisePluginEvent(enums.EventTypes.command, "", "Exit", "");
                Application.Current.Exit();
            }
            else
            {
                base.PauseLoad = false;
                m_Login.Visibility = Visibility.Collapsed;

                MenuItemClicked("", "localhost:5000", "", "", 0, 0);
            }
        }


        public new void Resize(Size PanelSize)
        {
            this.Width        = PanelSize.Width;
            this.Height       = PanelSize.Height;
            if (PanelSize.Width - ((SideMenu.Visibility.Equals(Visibility.Visible)) ? SideMenu.Width : 0)>0)
               PluginHost.Width  = PanelSize.Width - ((SideMenu.Visibility.Equals(Visibility.Visible)) ? SideMenu.Width : 0);
            if (PanelSize.Height - 40>0)
               PluginHost.Height = PanelSize.Height-40;
            PluginHost.Resize(new Size(PluginHost.Width, PluginHost.Height));
        }


        private void OnMenuItemClicked(NxMenuItem MenuItem, string Prompt, string Assembly, string Entry, string SnapIn, int ModuleId, int FunctionID)
        {
            m_MenuItem = MenuItem;
            MenuItemClicked(Prompt, Assembly, Entry, SnapIn, ModuleId, FunctionID);
        }

        private void MenuItemClicked(string Prompt, string Assembly, string Entry, string SnapIn, int ModuleId, int FunctionID)
        {
            if (chkDefault.IsSelected)
            {
                shell.Instance.Computer.DefaultUser = WorkerCache.Instance.WorkerId();
                shell.Instance.Computer.DefaultModule = ModuleId.ToString();
                shell.Instance.Computer.Update();
                chkDefault.SetChecked(false);
            }
            chkDefault.Visibility = Visibility.Collapsed;
            Thickness marg = SideMenu.Margin; marg.Bottom = 0; SideMenu.Margin = marg;

            m_TopPanel.Reset();
            object[] Pars = { SnapIn +";" + ModuleId.ToString() };

            //if (!PluginHost.Save()) return;

            if (PluginHost.LoadPlugin(Prompt, ModuleId, Assembly, Entry, false, true, Pars))
            {
                //m_TopPanel.Create(ModuleId, Assembly+ Entry, Prompt);
                PluginHost.Background = null;
                if (Assembly.Contains(".dll"))
                   PluginHost.Show(System.IO.Path.GetFileNameWithoutExtension(Assembly) + Entry);
                else
                   PluginHost.Show(Assembly + Entry);
            }
        }

        private void PluginHost_OnPluginEvent(enums.EventTypes reason, string sender, string command, string Property)
        {
            switch (reason)
            {
                case enums.EventTypes.status: m_BotPanel.UpdateInfo("sender " + sender + ", cmd " + command + ", prop " + Property); break;
                case enums.EventTypes.topmenu: if (command.ToUpper().Equals("ACTIONS")) m_TopPanel.LoadActions(sender, Property); break;

                case enums.EventTypes.snapin:   
                case enums.EventTypes.progress: 
                case enums.EventTypes.context:  
                case enums.EventTypes.content:  
                case enums.EventTypes.filter:   
                case enums.EventTypes.messageq: break;
                case enums.EventTypes.command:
                    switch (command)
                    {
                        case "KioskMode":
                            TopBar.Visibility = Visibility.Collapsed;
                            BotBar.Visibility = Visibility.Collapsed;


                            break;
                        default:
                            break;
                    }


                    break;
                default: break;
            }
        }

        private void PluginHost_OnStartLoad(int MaxLoops)
        {
            if (m_MenuItem != null) m_MenuItem.OnStartLoad(MaxLoops); 
        }
        private void PluginHost_OnProgress(int Progress, string Message)
        {
            if (m_MenuItem != null) m_MenuItem.OnProgress(Progress);
        }
        private void PluginHost_OnLoaded()
        {
            if (m_MenuItem != null) m_MenuItem.OnLoaded();
            SetMenuVisbility(false);
        }

        private void PluginHost_OnLoadButton(string ModuleId, string PluginTag, string Prompt)
        {
            if (m_TopPanel!=null) m_TopPanel.Create(ModuleId, PluginTag, Prompt);
        }


        private void MenuClicked(object? sender, ClickedEventArgs e)
        {
            SetMenuVisbility(btnMenu.Prompt.ToString() == txt_show);
        }
        private void SetMenuVisbility(bool SetVisible)
        {
            if (SetVisible)
            {
                SideMenu.Visibility = Visibility.Visible;
                PluginHost.Width  = this.Width - 140;
                PluginHost.Margin = new Thickness(140,0,0,0);
                btnMenu.Prompt    = txt_hide;
            }
            else
            {
                SideMenu.Visibility = Visibility.Collapsed;
                PluginHost.Width    = this.ActualWidth;
                PluginHost.Margin   = new Thickness(0);
                btnMenu.Prompt      = txt_show;
            }
            PluginHost.Resize(new Size(PluginHost.Width, PluginHost.Height));
        }

        private void TopPanel_OnSelected(string Command, string FunctionId, Windows.Foundation.Rect ScreenRect)
        {
            if (Command.Equals("<<EXIT>>"))
            {
                //if (PluginHost.UnLoadPlugin(FunctionId, ScreenRect)) m_TopPanel.RemoveMenu(FunctionId);
            }
            else
               PluginHost.Execute(ScreenRect, Command, "", FunctionId);
        }
        private void TopPanel_OnTabSelected(string PluginTag, string Prompt)
        {
            PluginHost.Show(PluginTag);
        }

        private void QuitClicked(object? sender, ClickedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Quit NextNet Application?", "Confirm",MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //    msg.RaisePluginEvent(enums.EventTypes.command, "", "Exit", "");
            //}
        }
        public new bool Execute(Windows.Foundation.Rect ScreenRect, string Command, string Parameters = null, string FunctionId = null)
        {
            if (Command.Equals("SaveState"))
            {
                //using (Option opt = new Option("Startup.State"))
                //{
                //    opt.SetValue("State", Parameters);
                //    opt.Update();
                //}
                return true;
            }
            else return PluginHost.Execute(ScreenRect, Command, Parameters, FunctionId);
        }

    }
}
