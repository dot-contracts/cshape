using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using Microsoft.UI.Xaml.Controls;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.dal;
using nexus.plugins.common;
using nexus.shared.promo;
using Uno.UI.RemoteControl.Messaging.IdeChannel;
using static nexus.common.control.NxMessageBox;

namespace nexus.plugins.promo
{

    public partial class Promos : NxPanelBase 
    {
        List<FieldDefinition> fields;
        PromotionRequest      request;
        PromoEdit             editor;

        private List<Promotion> promotions = new();

        int  dataRowIndex;
        bool showDeleted   = false;
        bool editorVisible = false;

        private DispatcherTimer counterTimer;

        public Promos()
        {
            InitializeComponent();
        }

        public async void Create()
        {
            fields = new()
            {
                new FieldDefinition { Field = "Counter",         Header = "Counter",     Width = "80" ,  TextAlign = "Center", Visible = true },
                new FieldDefinition { Field = "PromotionState",  Header = "Status",      Width = "110" , TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "PromotionType",   Header = "Type",        Width = "80" ,  TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Description",     Header = "Description", Width = "150",  TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Trigger",         Header = "Trigger",     Width = "110",  TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Action",          Header = "Action",      Width = "110",  TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Schedule.LastRun",      Header = "LastRun",     Width = "80",   TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Schedule.NextRun",      Header = "NextRun",     Width = "80",   TextAlign = "Left", Visible = true },
                new FieldDefinition { Field = "Schedule.ScheduleDesc", Header = "Schedule",    Width = "*",    TextAlign = "Left", Visible = true },
            };

            request = new PromotionRequest()
            {
                ActionType = 0, // 0 = Get All Promotions
                Vendor     = setting.Vendor,
                Location   = setting.Location
            };

            editor = new PromoEdit();
            editor.Create();
            editor.Visibility = Visibility.Collapsed;
            GridWithForm.AddPanel(editor);

            GridWithForm.AddButtons("Show Deleted;Add;Show Draw");

            GridWithForm.OnShowPanel      += GridWithForm_OnShowPanel;
            GridWithForm.OnMenuClicked    += GridWithForm_OnMenuClicked;
            GridWithForm.OnDataRowChanged += GridWithForm_OnDataRowChanged;

            counterTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };

            counterTimer.Tick += async (s, e) =>
            {
                if (!editorVisible)
                {
                    if (GridWithForm.SourceData is List<object> list)
                    {
                        foreach (var item in list)
                        {
                            if (item is Promotion promo)
                            {
                                // This forces re-evaluation of binding
                                promo.GetType().GetProperty("Counter")?.GetValue(promo);
                            }
                        }

                        await ShowData(showDeleted);
                    }
                }
            };
            counterTimer.Start();

            await ShowData();

        }

        private void GridWithForm_OnDataRowChanged(int dataRowIndex, string Display, string Value)
        {
            this.dataRowIndex = dataRowIndex;
            if (showDeleted)  GridWithForm.AddButtons("Hide Deleted;Archive;Revive");
            else              GridWithForm.AddButtons("Show Deleted;Add;Edit;Delete");
        }

        private async void GridWithForm_OnShowPanel(object? sender, GridWithForm.ShowPanelEventArgs e)
        {
            editorVisible     = true;
            editor.Visibility = Visibility.Visible;
            await GridWithForm.AnimateForm(true);
            GridWithForm.AddButtons("Update;Cancel");
        }

        private void GridWithForm_OnMenuClicked(object? sender, ClickedEventArgs e)
        {
            HandleButtonClicked (e.Tag);
        }

        private async void HandleButtonClicked(string buttonName = "")
        {
            var item = GridWithForm.SourceData[dataRowIndex] as Promotion;

            switch (buttonName)
            {
                case "Show Draw":
                    if (item == null) return;
                    RaiseEvent(enums.EventTypes.command, "Promos", "ShowGame", item.PoolId.ToString());
                    break;

                case "Cancel":
                    editorVisible = false;
                    await GridWithForm.AnimateForm(false, 1);
                    GridWithForm.AddButtons(showDeleted ? "Hide Deleted" : "Show Deleted" + ";Add");
                    break;

                case "Show Deleted":
                case "Hide Deleted":
                    await ShowData(buttonName.Equals("Show Deleted"));
                    GridWithForm.AddButtons(showDeleted ? "Hide Deleted":"Show Deleted" + ";Add");
                    break;

                case "Add":
                    editor.Reset();
                    editor.Visibility = Visibility.Visible;
                    await GridWithForm.AnimateForm(true, 1);
                    GridWithForm.AddButtons("Update;Cancel");
                    break;

                case "Edit":
                    if (item == null) return;

                    // Load it into the editor
                    await editor.ShowData(item); // You can rename ShowData() to Load() if needed
                    editor.Visibility = Visibility.Visible;
                    await GridWithForm.AnimateForm(true, 1);
                    GridWithForm.AddButtons("Update;Cancel");
                    break;

                case "Delete":
                    {
                        await using (var msg = new NxMessageBox("Delete Promotion", "Are you sure you want to delete this promotion?"))
                        {
                            if (await msg.ShowAsync() == NxMessageBoxResult.Yes)
                            {

                            }
                        }
                        
                        await ShowData();
                        GridWithForm.AddButtons("Show Deleted;Add");
                    }
                    break;

                case "Archive":
                    {
                        await using (var msg = new NxMessageBox("Archive Promotion", "Are you sure you want to archive this promotion?"))
                        {
                            if (await msg.ShowAsync() == NxMessageBoxResult.Yes)
                            {

                            }
                        }

                        await ShowData();
                        GridWithForm.AddButtons("Show Deleted;Add");
                    }
                    break;

                case "Update":
                    {
                        if (editor.Save())
                        {
                            editorVisible = false;
                            await GridWithForm.AnimateForm(false, 1);
                            GridWithForm.AddButtons("Show Deleted;Add");
                            await ShowData();
                        }
                        else
                        {
                            await using var msg = new NxMessageBox("Error", "Failed to save promotion. Please check the details and try again.");
                            await msg.ShowAsync();
                        }
                    }
                    break;
            }
        }

        private async Task ShowData(bool ShowDeleted = false)
        {
            try
            {
                showDeleted  = ShowDeleted;
                dataRowIndex = 0;

                using var helper = new PromotionHelper();
                var data = await helper.Process(request);
                var list = data.Promotions.ToList();

                if (ShowDeleted) promotions = list.Where(p =>  string.Equals(p.PromotionState, "Deleted State", StringComparison.OrdinalIgnoreCase)).ToList();
                else             promotions = list.Where(p => !string.Equals(p.PromotionState, "Deleted State", StringComparison.OrdinalIgnoreCase)).ToList();

                GridWithForm.SetItemsSource(promotions, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
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
