using System;
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

using nexus.shared.common;
using nexus.shared.promo;



namespace nexus.plugins.promo
{


    public partial class PromoEdit : NxGroupBase 
    {
        public DataTable? Actions  { get; set; }
        public DataTable? Triggers { get; set; }

        List<FieldDefinition> fields;
        PromotionRequest      request;
        Promotion             promotion;
        Schedule              schedule;

        public PromoEdit()
        {
            InitializeComponent();
        }

        public async void Create()
        {
            cbPromoType. SetDataTable(EnumCache.Instance.getEnumType  ("Promotion"),  "Description", "ValuePk" );
            cbPromoState.SetDataTable(EnumCache.Instance.getEnumState ("Promotion"),  "Description", "ValuePk" );
            cbTrigger.   SetDataTable(EnumCache.Instance.getEnumType  ("ProTrigger"), "Description", "ValuePk" );
            cbAction.    SetDataTable(EnumCache.Instance.getEnumType  ("ProAction"),  "Description", "ValuePk" );

            Array ctls = "txDescription;cbPromoType;cbPromoState;cbTrigger;cbAction;txGameTimeOut;txMaxDraws;ckAutoDraw;txStartPage;txDrawPage;txSponsorPage;txSchedule".Split(';');
            foreach (string name in ctls)
            {
                if (FindName(name) is NxTextEdit nx)
                {
                    nx.OnChangedControl += Nx_OnChangedControl;
                    nx.OnProcessKeyDown += Nx_OnProcessKeyDown;
                    AddControl(nx);
                }
            }

            btSchedule.OnClicked += BtSchedule_OnClicked;
        }

        private async void BtSchedule_OnClicked(object? sender, ClickedEventArgs e)
        {
            using (var dlg = new DlgSchedule(schedule))
            {
                var tcs = new TaskCompletionSource<bool>();

                dlg.OnScheduleFinished += (sched) =>
                {
                    schedule = sched;
                    tcs.TrySetResult(true);
                    dlg.Close(); // Closes popup properly
                };

                dlg.Show(); // Shows popup
                await tcs.Task;
            }
        }

        private void Nx_OnProcessKeyDown(Windows.System.VirtualKey Key, ref bool Handled)
        {

        }

        private void Nx_OnChangedControl(NxTextEdit Ctl, string Tag, string Display, string Value, ref bool Valid)
        {

        }

        public async Task ShowData(Promotion promo)
        {
            
            if (promo != null)
            {
                promotion = promo;
                try
                {
                    txDescription.Value  = promo.Description;
                    cbPromoType.Value    = promo.PromotionTypeId.ToString();
                    cbPromoState.Value   = promo.PromotionStateId.ToString();
                    txGameTimeout.Value  = promo.GameTimeout?.ToString();
                    txMaxDraws.Value     = promo.MaxDraws?.ToString();
                    ckAutoDraw.Value     = (promo.AutoDraw ?? false).ToString();
                    txStartPage.Value    = promo.StartPage;
                    txSchedule.Value     = promo.Schedule.ScheduleDesc;
                }
                catch (Exception ex)
                {
                    string xx = ex.Message;
                }

                schedule = promo.Schedule;

            }
        }

        private async void LoadData()
        {
            try
            {
                ActionRequest request = new ActionRequest(0, setting.Vendor, setting.Location);

                using var helper = new ActionHelper();
                var data = await helper.Process(request);

                DataTable actionTable = new DataTable();
                actionTable.Columns.Add("ActionType",   typeof(string));
                actionTable.Columns.Add("ActionDefn",   typeof(string));
                actionTable.Columns.Add("ActionTypeId", typeof(int));

                var filtered = data.Actions .Where   (a => a.ActionState == "Active State")
                                            .GroupBy (a => a.ActionType);

                foreach (var g in filtered)
                {
                    var item = g.FirstOrDefault();
                    if (item != null)
                    {
                        actionTable.Rows.Add(g.Key, item.ActionDefn ?? "", item.ActionTypeId ?? -1);
                    }
                }

                Actions = actionTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }

            try
            {
                TriggerRequest trigger = new TriggerRequest(0, setting.Vendor, setting.Location);

                using var helper = new TriggerHelper();
                var data = await helper.Process(trigger);

                DataTable triggerTable = new DataTable();
                triggerTable.Columns.Add("PromotionType", typeof(string));
                triggerTable.Columns.Add("TriggerType",   typeof(string));
                triggerTable.Columns.Add("TriggerTypeId", typeof(int));

                var filtered = data.Triggers .Where   (t => t.TriggerState == "Active State")
                                             .GroupBy (t => new { t.PromotionType, t.TriggerType });

                foreach (var g in filtered)
                {
                    var item = g.FirstOrDefault();
                    if (item != null)
                    {
                        triggerTable.Rows.Add(g.Key.PromotionType ?? "", g.Key.TriggerType ?? "", item.TriggerTypeId ?? -1);
                    }
                }

                Triggers = triggerTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }

        }

        public DataTable GetTriggerSubset(string promotionType)
        {
            if (Triggers == null || string.IsNullOrEmpty(promotionType))
                return new DataTable();

            var view = Triggers.DefaultView;
            view.RowFilter = $"PromotionType = '{promotionType.Replace("'", "''")}'";

            return view.ToTable();
        }


        public async Task<bool> SavePromo()
        {
            promotion.Description    = txDescription.Value;
            promotion.PromotionType  = cbPromoType.Value;
            promotion.PromotionState = cbPromoState.Value;
            promotion.GameTimeout    = int.TryParse(txGameTimeout.Value, out var gto)      ? gto : null;
            promotion.MaxDraws       = int.TryParse(txMaxDraws.Value,    out var max)      ? max : null;
            promotion.DrawInterval   = int.TryParse(txInterval.Value,    out var interval) ? interval : null;

            var request = new PromotionRequest { ActionType = 3, Promotion = promotion };

            using var helper = new PromotionHelper();
            var data = await helper.Process(request);

            return true;
        }

        public async Task<bool> DeletePromo()
        {

            int deleted = 0;

            promotion.PromotionStateId = deleted;

            var request = new PromotionRequest { ActionType = 3, Promotion = promotion };

            using var helper = new PromotionHelper();
            var data = await helper.Process(request);

            return true;
        }


        #region Override

        public new bool Load(int LoadState, string Property)
        {
            return false;
        }
        public new void Reset()
        {
        }
        public new bool Save() { return true; }

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


    public class ActionTypeSummary
    {
        public string? ActionType   { get; set; }
        public string? ActionDefn   { get; set; }
        public int?    ActionTypeId { get; set; }
    }

    public class TriggerTypeSummary
    {
        public string? PromotionType { get; set; }
        public string? TriggerType   { get; set; }
        public int?    TriggerTypeId { get; set; }
    }
}
