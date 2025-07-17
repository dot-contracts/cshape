
using System.Data;
using System.Linq;
using Microsoft.UI;

using nexus.shared.gaming;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.control.Themes;
using nexus.common.dal;

namespace nexus.plugins.promo
{
    public sealed partial class MainUI : NxMainUIBase, IMainUI
    {

        private Promos    Promos;
        //private DashBoard DashBoard;
        //private HouseNo   HouseNo;

        private delegate bool ProcessCommandCB(string Data);
        private delegate void CreateCB();
        private delegate void ShowPanelCB(int MemberID);

        public MainUI()
        {
            this.InitializeComponent();

            MainInfo = new MainInfo("Promos", "957A050D-8418-4341-B6D1-D647937F3C99");

            MaxLoadLoops = 20;

            this.Loaded       += MainUI_Loaded;
            base.LoadLoop     += MainUI_LoadLoop;
            base.LoopFinished += MainUI_LoopFinished;
        }

        private void MainUI_Loaded(object sender, RoutedEventArgs e) { TimerStart(20); }

        private async void MainUI_LoadLoop()
        {

            switch (LoadState)
            {
                case 0:
                    await Task.Run(async () => await InitStartupAsync());
                    msg.OnPluginEvent += Msg_OnPluginEvent;

                    LoadState++;
                    break;

                case 1: // connect to SignalR instead
                    //tcpClient.Create("Main", "man","man123");
                    LoadState++;
                    break;

                case 2:
                    Promos    = new Promos();    Host.Children.Add(Promos);    Promos.Visibility    = Visibility.Collapsed;
                    //DashBoard = new DashBoard(); Host.Children.Add(DashBoard); DashBoard.Visibility = Visibility.Visible;
                    //HouseNo   = new HouseNo();   Host.Children.Add(HouseNo);   HouseNo.Visibility   = Visibility.Collapsed;

                    LoadState++;
                    break;

                case 3: // load any default module based on look up
                    LoadState++;
                    break;

                case 4:
                    ApplyThemeDefaults();
                    NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

                    LoadState++;
                    break;

                case 5:

                    LoadState++;

                    Promos.Create();

                    break;

                default:
                    LoadState = MaxLoadLoops;
                    break;
            }
        }
        private void ApplyThemeDefaults()
        {
        }

        private void MainUI_LoopFinished()
        {
        }
        public async Task InitStartupAsync()
        {
            //using (OptionHelper opt = new OptionHelper())
            //{
            //    await opt.GetOptions(OptionPath: "Startup.State");

            //    string Params = opt.GetValue("State", "");
            //    if (!string.IsNullOrEmpty(Params))
            //        msg.RaisePluginEvent(enums.EventTypes.command, "", "SetState", Params);
            //}
        }

        private void Msg_OnPluginEvent(enums.EventTypes reason, string sender, string command, string Property)
        {
        }

    }
}
