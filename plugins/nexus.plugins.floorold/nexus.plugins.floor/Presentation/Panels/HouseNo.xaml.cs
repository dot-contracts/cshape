using System;
using System.IO;
using System.Data;
using System.Windows;
using System.IO.Packaging;

using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.control.uno;
using nexus.common.dal;
using Microsoft.UI.Xaml.Controls;

namespace nexus.plugins.floor
{

    public partial class HouseNo : NxPanelBase
    {

        private string currentSearch = "";

        public HouseNo()
        {
            InitializeComponent();

            // Hook up events
            txHouse.OnChanged += _OnChanged;
            //NumPad.OnNumberChanged += NumPad_OnNumberChanged; // Custom event from NxNumPad
            //Results.SelectionChanged += Results_SelectionChanged;

            ShowSearch();
        }

        public void Create(string EgmId)
        {

        }

        private void ShowData()
        {
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

        private async void _OnChanged(object? sender, ChangedEventArgs e)
        {
            currentSearch = txHouse.Value;
            await RunSearch(currentSearch);
        }

        private async void NumPad_OnNumberChanged(object sender, string newNumber)
        {
            txHouse.Value = newNumber;
            currentSearch = newNumber;
            await RunSearch(currentSearch);
        }

        private async Task RunSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
                return;

            var results = await LookUpHouseAsync(input);

            //Results.ItemsSource = results;

            if (results.Count == 1)
            {
                ShowResult(results[0]);
            }
        }

        private void Results_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Results.SelectedItem is LookupResult selected)
            //{
            //    ShowResult(selected);
            //}
        }

        private void ShowSearch()
        {
            //Search.Visibility = Visibility.Visible;
            //Result.Visibility = Visibility.Collapsed;
        }

        private void ShowResult(LookupResult result)
        {
            //Search.Visibility = Visibility.Collapsed;
            //Result.Visibility = Visibility.Visible;

            txHouse.Value    = result.BadgeNo;  // Assuming BadgeNo = HouseNo
            txPlayer.Value   = result.Player;
            txGameName.Value = result.GameName;
            txTier.Value     = result.Tier;
            txPlayTime.Value = result.PlayTime;
            txAvgBet.Value   = result.AverageBet;

            //History.ItemsSource = LoadHistory(result.BadgeNo); // Optional mock
        }

        private async Task<List<LookupResult>> LookUpHouseAsync(string houseNo)
        {
            // Replace this with a real API call
            await Task.Delay(100); // simulate latency

            return new List<LookupResult>
            {
                new LookupResult
                {
                    BadgeNo    = houseNo,
                    Player     = "Jane Doe",
                    GameName   = "Baccarat",
                    Tier       = "Platinum",
                    PlayTime   = "2:45",
                    AverageBet = "$50.00"
                }
            };
        }

        private List<string> LoadHistory(string houseNo)
        {
            return new List<string>
            {
                "04/04 - Baccarat - $100",
                "03/04 - Roulette - $25"
            };
        }
        #endregion

    }
}
