
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Composition;

using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.dal;
using nexus.shared.gaming;

namespace nexus.plugins.floor
{
    public partial class DashBoard : NxPanelBase
    {
        private string SelectedTab = "Paging";
        private DispatcherTimer _timer;

        public DashBoard()
        {
            InitializeComponent();

            ResultTabs.OnChanged += ResultTabs_OnChanged; 
        }

        public async Task Create()
        {
            await LoadAllData();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _timer.Tick += async (s, e) => await LoadAllData();
            _timer.Start();
        }

        private async Task LoadAllData()
        {
            var tasks = new List<Task>();

            tasks.Add(LoadChartData());
            tasks.Add(LoadPayouts());
            tasks.Add(LoadGrid(SelectedTab));

            await Task.WhenAll(tasks);
        }

        private async Task LoadChartData()
        {
            try
            {
                using var helper = new TodayHelper();
                var data = await helper.Process();

                if (data == null || data.Count == 0)
                {
                    BarChart.DrawChart(new DataTable(), "TurnOver", "CloseTime", 300, 800);
                    return;
                }

                var sortedData = data
                    .Where(x => !string.IsNullOrEmpty(x.CloseTime) && x.TurnOver > 0)
                    .OrderBy(x => x.CurrentDate)
                    .ToList();

                DataTable dt = new();
                dt.Columns.Add("CloseTime", typeof(string));
                dt.Columns.Add("TurnOver", typeof(decimal));

                foreach (var item in sortedData)
                    dt.Rows.Add(item.CloseTime, item.TurnOver);

                double chartHeight = BarChart.ActualHeight > 0 ? BarChart.ActualHeight : 300;
                double chartWidth = BarChart.ActualWidth > 0 ? BarChart.ActualWidth : 800;

                BarChart.DrawChart(dt, "TurnOver", "CloseTime", chartHeight, chartWidth);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadChartData error: {ex.Message}");
            }
        }

        private async void ResultTabs_OnChanged(object? sender, ChangedEventArgs e)
        {
            if (ResultTabs.SelectedItem is string selected) await LoadGrid(e.Value);
        }
        private async Task LoadGrid(string selectTab)
        {
            SelectedTab = selectTab;
            switch (selectTab)
            {
                case "Paging":  await LoadPaging();     break;
                case "Player":  await LoadPlayer();     break;
                case "Card":    await LoadCardGrid();   break;
                case "Audit":   await LoadAuditGrid();  break;
            }
        }

        private async Task LoadPayouts()
        {
            try
            {
                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "Type",   Header = "Type",   Width = "120", TextAlign = "Left",  Visible = true },
                    new FieldDefinition { Field = "Amount", Header = "Amount", Width = "*", TextAlign = "Right", Visible = true }
                };

                string OpenDate  = "";
                string CloseDate = "";

                using var liabilityHelper = new LiabilityHelper();
                var liability = await liabilityHelper.Process(OpenDate: OpenDate, CloseDate: CloseDate);

                using var payoutHelper = new PayoutHelper();
                var payout = await payoutHelper.Process(OpenDate: OpenDate, CloseDate: CloseDate);

                var dt = new DataTable();
                dt.Columns.Add("Type");
                dt.Columns.Add("Amount");

                if (liability != null)
                {
                    dt.Rows.Add("Opening", liability.Opening.ToString());
                    dt.Rows.Add("Closing", liability.Closing.ToString());
                    dt.Rows.Add("Movement", liability.Movement.ToString());
                }

                foreach (var p in payout)
                    dt.Rows.Add(p.Description.ToString(), p.Value.ToString());

                Payouts.SetItemsSource(dt, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadPayouts error: {ex.Message}");
            }
        }

        private async Task LoadPaging()
        {
            try
            {
                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "EventDate",   Header = "PageDate",    Width = "130", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Description", Header = "Description", Width = "480",  TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Property",    Header = "Property",    Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Entity",      Header = "Entity",      Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Human",       Header = "Human",       Width = "200", TextAlign = "Left", Visible = true },
                };

                using var helper = new PagingHelper();
                var data = await helper.Process();
                Results.SetItemsSource(data, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadPaging error: {ex.Message}");
            }
        }

        private async Task LoadPlayer()
        {
            try
            {
                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "Name",         Header = "Name",         Width = "220", TextAlign = "Left",   Visible = true },
                    new FieldDefinition { Field = "Tier",         Header = "Tier",         Width = "130", TextAlign = "Left",   Visible = true },
                    new FieldDefinition { Field = "Turnover",     Header = "Turnover",     Width = "80",  TextAlign = "Right",  Visible = true },
                    new FieldDefinition { Field = "AvgBet",       Header = "AvgBet",       Width = "80",  TextAlign = "Right",  Visible = true },
                    new FieldDefinition { Field = "Duration",     Header = "Duration",     Width = "80",  TextAlign = "Center", Visible = true },
                    new FieldDefinition { Field = "Spent",        Header = "Spent",        Width = "80",  TextAlign = "Right",  Visible = true },
                    new FieldDefinition { Field = "Earned",       Header = "Earned",       Width = "80",  TextAlign = "Right",  Visible = true },
                    new FieldDefinition { Field = "LastPlayedAt", Header = "LastPlayedAt", Width = "*",   TextAlign = "Left",   Visible = true },
                };

                using var helper = new PlayerHelper();
                var data = await helper.Process();
                Results.SetItemsSource(data, fields);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadPlayer error: {ex.Message}");
            }
        }

        private async Task LoadCardGrid()
        {
            try
            {
                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "Duration",     Header = "Duration",     Width = "80",  TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "OpenDate",     Header = "Start",        Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "FullName",     Header = "Player",       Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "GameName",     Header = "GameName",     Width = "150", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Turnover",     Header = "Turnover",     Width = "80",  TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "AvgBet",       Header = "AvgBet",       Width = "80",  TextAlign = "Left", Visible = true },
                };

                using var helper = new CardPlayHelper();
                var data = await helper.Process(ActionType: "3");
                Results.SetItemsSource(data, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadCardGrid error: {ex.Message}");
            }
        }

        private async Task LoadAuditGrid()
        {
            try
            {
                string OpenDate = "Apr 11, 2025 4:30am";
                string CloseDate = "Apr 12, 2025 4:30am";

                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "EventDate",   Header = "PageDate",    Width = "130", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Description", Header = "Description", Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Property",    Header = "Property",    Width = "350", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Entity",      Header = "Entity",      Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Human",       Header = "Human",       Width = "200", TextAlign = "Left", Visible = true },
                };

                //using var helper = new EventHelper();
                //var data = await helper.Process(OpenDate: OpenDate, CloseDate: CloseDate, MaxRows: "50");
                //Results.SetItemsSource(data, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadAuditGrid error: {ex.Message}");
            }
        }

    }
}
