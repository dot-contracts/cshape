using System;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using Microsoft.UI.Xaml.Controls;
using Uno.UI.RemoteControl.Messaging.IdeChannel;

using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.dal;
using nexus.shared.promo;
{
    
}

namespace nexus.plugins.promo
{

    public partial class Promos : NxPanelBase 
    {
        public Promos()
        {
            InitializeComponent();
        }

        public void Create()
        {

        }

        private async Task ShowData()
        {
            try
            {
                List<FieldDefinition> fields = new()
                {
                    new FieldDefinition { Field = "Type",        Header = "Header",      Width = "100",  TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Description", Header = "Description", Width = "200",  TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Starts",      Header = "Starts",      Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Finishes",    Header = "Finishes",    Width = "200", TextAlign = "Left", Visible = true },
                    new FieldDefinition { Field = "Human",       Header = "Human",       Width = "200", TextAlign = "Left", Visible = true },
                };

                PromotionRequest request = new PromotionRequest()
                {
                    ActionType = 0, // 0 = Get All Promotions
                    Vendor     = setting.Vendor,
                    Location   = setting.Location
                };

                using var helper = new PromotionHelper();
                var data = await helper.Process(request);
                var list = data.Promotions.ToList();
                //Results.SetItemsSource(list, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }

        }


        //private async void LoadPromotions()
        //{
        //    var request = new PromotionRequest { ActionType = 1 };
        //    var result = await _helper.Process(request);
        //    _promoTable = await PromotionHelper.GetDataFromResponse(result, await PromotionHelper.CreateDataTableFromClass<Promotion>());
        //    Results.SetItemsSource(_promoTable, _promoTable.Columns.Cast<DataColumn>().Select(c => new FieldDefinition { Field = c.ColumnName, Header = c.ColumnName }).ToList());
        //}

        //private async void Results_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
        //    if (Results.SelectedRow is DataRow row && int.TryParse(row["PromotionId"]?.ToString(), out var id))
        //    {
        //        var request = new PromotionRequest { ActionType = 2, Promotion = new Promotion { PromotionId = id } };
        //        var result = await _helper.Process(request);
        //        var promo = result.Promotions.FirstOrDefault();
        //        if (promo != null)
        //        {
        //            editPromotionType.Value = promo.PromotionType;
        //            editPromotionState.Value = promo.PromotionState;
        //            editDescription.Value = promo.Description;
        //            editGameTimeout.Value = promo.GameTimeout?.ToString();
        //            editMaxDraws.Value = promo.MaxDraws?.ToString();
        //            editDrawInterval.Value = promo.DrawsInterval?.ToString();
        //            ScheduleEditor.LoadSchedule(promo.Schedule);
        //            await AnimateForm(true);
        //        }
        //    }
        //}


        //private async void BtUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    //var updatedPromo = new Promotion
        //    //{
        //    //    PromotionType = editPromotionType.Value,
        //    //    PromotionState = editPromotionState.Value,
        //    //    Description = editDescription.Value,
        //    //    GameTimeout = int.TryParse(editGameTimeout.Value, out var gto) ? gto : null,
        //    //    MaxDraws = int.TryParse(editMaxDraws.Value, out var max) ? max : null,
        //    //    DrawsInterval = int.TryParse(editDrawInterval.Value, out var interval) ? interval : null
        //    //};

        //    //ScheduleEditor.SaveSchedule();
        //    //updatedPromo.Schedule = ScheduleEditor.Schedule;

        //    //var request = new PromotionRequest { ActionType = 3, Promotion = updatedPromo };
        //    //await _helper.Process(request);
        //    //await AnimateForm(false);
        //    //LoadPromotions();
        //}



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
