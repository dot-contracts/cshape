
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
        }

        private void MainUI_Loaded(object sender, RoutedEventArgs e) { TimerStart(20); }

        private async void MainUI_LoadLoop()
        {
            switch (LoadState)
            {
                case 0:
                    ApplyThemeDefaults();
                    NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

                    LoadState++;
                    break;

                case 1:


                    break;

                default:
                    LoadState = MaxLoadLoops;
                    break;
            }
        }

        private void ApplyThemeDefaults()
        {
        }



    }
}
