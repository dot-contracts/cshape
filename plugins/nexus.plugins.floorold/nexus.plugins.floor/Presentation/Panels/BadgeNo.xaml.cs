using System;
using System.IO;
using System.Data;
using System.Windows;
using System.IO.Packaging;

using Microsoft.UI.Xaml.Controls;

using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.control.uno;
using nexus.common.dal;

namespace nexus.plugins.floor
{

    public partial class BadgeNo : NxPanelBase 
    {

        private string currentSearch = "";

        public BadgeNo()
        {
            InitializeComponent();

            // Hook up event handlers
            //txBadgeNo.OnChanged += TxBadgeNo_OnChanged;
            //NumPad.OnNumberChanged += NumPad_OnNumberChanged; // assuming you raise this event from NxNumPad
            //Results.SelectionChanged += Results_SelectionChanged;

            ShowSearch();
        }

        public void Create(string MemberId)
        {

        }

        private async void TxBadgeNo_OnChanged(object? sender, ChangedEventArgs e)
        {
           // currentSearch = txBadgeNo.Value;
            await RunSearch(currentSearch);
        }

        private async void NumPad_OnNumberChanged(object sender, string newNumber)
        {
           // txBadgeNo.Value = newNumber; // mirror into the textbox
            currentSearch   = newNumber;
            await RunSearch(currentSearch);
        }

        private async Task RunSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
                return;

            var results = await LookUpAsync(input);

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

            //txBadgeNo.Value  = result.BadgeNo;
            txPlayer.Value   = result.Player;
            txGameName.Value = result.GameName;
            txTier.Value     = result.Tier;
            txPlayTime.Value = result.PlayTime;
            txAvgBet.Value   = result.AverageBet;

            // Load history if needed
            //History.ItemsSource = LoadHistory(result.BadgeNo); // optional
        }

        private async Task<List<LookupResult>> LookUpAsync(string badgeNo)
        {
            // TODO: Replace with real API call
            await Task.Delay(100); // Simulate network latency

            return new List<LookupResult>
            {
                new LookupResult
                {
                    BadgeNo    = badgeNo,
                    Player     = "John Smith",
                    GameName   = "Poker",
                    Tier       = "Gold",
                    PlayTime   = "3:20",
                    AverageBet = "$12.30"
                }
            };
        }

        private List<string> LoadHistory(string badgeNo)
        {
            return new List<string>
            {
                "03/04 - Poker - $10",
                "02/04 - Slots - $5"
            };
        }

        public bool Create()
        {
            return true;
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

        #endregion

    }
}
