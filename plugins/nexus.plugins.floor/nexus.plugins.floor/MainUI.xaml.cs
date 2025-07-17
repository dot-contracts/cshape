
using System.Data;
using System.Linq;
using Microsoft.UI;

using nexus.shared.gaming;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.control.Themes;
using nexus.common.dal;

namespace nexus.plugins.floor
{
    public sealed partial class MainUI : NxMainUIBase, IMainUI
    {

        private string SearchType = "HouseNo";
        List<LookUp> searchResults = new List<LookUp>();

        private DashBoard DashBoard;
        private BadgeNo   BadgeNo;
        private HouseNo   HouseNo;

        private delegate bool ProcessCommandCB(string Data);
        private delegate void CreateCB();
        private delegate void ShowPanelCB(int MemberID);

        public MainUI()
        {
            this.InitializeComponent();

            MainInfo = new MainInfo("Floor", "46591D41-A0C5-4C09-BF63-0DB3BD831BFF");

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
                    DashBoard = new DashBoard(); Host.Children.Add(DashBoard); DashBoard.Visibility = Visibility.Visible;
                    BadgeNo   = new BadgeNo();   Host.Children.Add(BadgeNo);   BadgeNo.Visibility   = Visibility.Collapsed;
                    HouseNo   = new HouseNo();   Host.Children.Add(HouseNo);   HouseNo.Visibility   = Visibility.Collapsed;

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
                    ckHouse.IsSelected = true;
                    ckBadge.IsSelected = false;

                    ckHouse.OnChanged        += HandleCheck; 
                    ckBadge.OnChanged        += HandleCheck;
                    ResList.OnDataRowChanged += HandleDataRowChanged;
                    NumPad.OnChanged         += HandleNumPad;

                    LoadState++;

                    await DashBoard.Create();

                    break;

                default:
                    LoadState = MaxLoadLoops;
                    break;
            }
        }
        private void ApplyThemeDefaults()
        {
            Search.Background = NxThemeManager.Current.GetThemeBrush("NxMenuBack", Colors.Gray);
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

        private void HandleCheck(object? sender, ChangedEventArgs e)
        {
            SearchType = e.Tag;

            ckHouse.IsSelected = false;
            ckBadge.IsSelected = false;

            HouseNo.Visibility = Visibility.Collapsed;
            BadgeNo.Visibility = Visibility.Collapsed;

            if      (SearchType.Equals("HouseNo")) { ckHouse.IsSelected = true; HouseNo.Visibility = Visibility.Visible; }
            else if (SearchType.Equals("BadgeNo")) { ckBadge.IsSelected = true; BadgeNo.Visibility = Visibility.Visible; }

            NumPad.Value = "";

            DoSearch();
        }

        private void HandleNumPad(object? sender, ChangedEventArgs e)
        {
            DoSearch();
        }

        private void HandleDataRowChanged(int dataRowIndex, string Display, string Value)
        {
            if      (SearchType.Equals("HouseNo")) { HouseNo.Create(Value); }
            else if (SearchType.Equals("BadgeNo")) { ckBadge.IsSelected = true; BadgeNo.Visibility = Visibility.Visible; }
        }

        private async void DoSearch()
        {
            searchResults = new List<LookUp>();

            List<FieldDefinition> fields = new()
            {
                new FieldDefinition { Field = "Description", Header = "Description", Width = "300", Format = "", TextAlign = "Left", Visible = true }
            };

            if (!string.IsNullOrEmpty(NumPad.Value))
            {
                using (var helper = new LookUpHelper())
                    searchResults = await helper.Process(Search: SearchType, Value: NumPad.Value);
            }

            if (searchResults != null && searchResults.Any())
            {
                ResList.SetItemsSource(searchResults, fields);
            }
            else
            {
                ResList.SetItemsSource(new List<LookUp>(), fields);
            }
        }
    }
}
