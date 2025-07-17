
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

using nexus.common;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.promo;

namespace nexus.api.promo
{
    [Route("pro")]
    public class apiTransaction : ControllerBase
    {
        [HttpPost("Transaction")]
        [Authorize]
        public IActionResult Post([FromBody] TransactionRequest request)
        {

            TransactionResponse response = new TransactionResponse()
            {
                Status   = "Error",
                Response = "Request Poorly formatted"
            };

            if (request!=null)
            {
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {
                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;
                               Params += ";I~S~QuietMode~0;I~S~ActionTypes~0";

                        DataTable DT = db.GetDataTable("xta.ValidateUse", Params);
                        if (DT.Rows.Count > 0)
                        {
                            if (helpers.ToInt(DT.Rows[0]["ReturnCode"].ToString()).Equals(0))
                            {
                                Params = "";
                                if (request.Transaction.TransactionId>0)                         Params += ";I~S~TransactionId~"    + request.Transaction.TransactionId;
                                if (!string.IsNullOrEmpty(request.Transaction.TransactionState)) Params += ";I~S~TransactionState~" + request.Transaction.TransactionStateId;

                                if (request.ActionType.Equals(0))    DT = db.GetDataTable("pro.lst_Transaction", Params);
                                else
                                {
                                    if (request.Transaction != null && request.Transaction.HasData())
                                    {
                                        var tx = request.Transaction;

                                        if (tx.TransactionId         > 0)                    Params += ";I~S~TransactionId~"         + tx.TransactionId;
                                        if (!string.IsNullOrEmpty(tx.Promotion))             Params += ";I~S~Promotion~"             + tx.Promotion;
                                        if (!string.IsNullOrEmpty(tx.PromotionType))         Params += ";I~S~PromotionType~"         + tx.PromotionType;
                                        if (!string.IsNullOrEmpty(tx.TransactionState))      Params += ";I~S~TransactionState~"      + tx.TransactionState;
                                        if (tx.MinValue              > 0)                    Params += ";I~S~MinValue~"              + tx.MinValue;
                                        if (tx.MaxValue              > 0)                    Params += ";I~S~MaxValue~"              + tx.MaxValue;
                                        if (tx.TransactionStateId    > 0)                    Params += ";I~S~TransactionStateId~"    + tx.TransactionStateId;
                                        if (tx.PromotionId           > 0)                    Params += ";I~S~PromotionId~"           + tx.PromotionId;
                                        if (tx.ParentId              > 0)                    Params += ";I~S~ParentId~"              + tx.ParentId;

                                        if (!string.IsNullOrEmpty(tx.Trigger))               Params += ";I~S~Trigger~"               + tx.Trigger;
                                        if (!string.IsNullOrEmpty(tx.Action))                Params += ";I~S~Action~"                + tx.Action;
                                        if (!string.IsNullOrEmpty(tx.Device))                Params += ";I~S~Device~"                + tx.Device;
                                        if (!string.IsNullOrEmpty(tx.Worker))                Params += ";I~S~Worker~"                + tx.Worker;
                                        if (!string.IsNullOrEmpty(tx.Member))                Params += ";I~S~Member~"                + tx.Member;
                                        if (tx.TriggerValue          > 0)                    Params += ";I~S~TriggerValue~"          + tx.TriggerValue;
                                        if (tx.TriggerCount          > 0)                    Params += ";I~S~TriggerCount~"          + tx.TriggerCount;
                                        if (!string.IsNullOrEmpty(tx.TriggerDefn))           Params += ";I~S~TriggerDefn~"           + tx.TriggerDefn;
                                        if (!string.IsNullOrEmpty(tx.AdjustmentType))        Params += ";I~S~AdjustmentType~"        + tx.AdjustmentType;
                                        if (tx.Adjustment            > 0)                    Params += ";I~S~Adjustment~"            + tx.Adjustment;
                                        if (!string.IsNullOrEmpty(tx.Expires))               Params += ";I~S~Expires~"               + tx.Expires;
                                        if (!string.IsNullOrEmpty(tx.Taken))                 Params += ";I~S~Taken~"                 + tx.Taken;
                                        if (!string.IsNullOrEmpty(tx.Starts))                Params += ";I~S~Starts~"                + tx.Starts;
                                        if (!string.IsNullOrEmpty(tx.Finishes))              Params += ";I~S~Finishes~"              + tx.Finishes;
                                        if (!string.IsNullOrEmpty(tx.ActionDefn))            Params += ";I~S~ActionDefn~"            + tx.ActionDefn;
                                        if (tx.Prize                 > 0)                    Params += ";I~S~Prize~"                 + tx.Prize;
                                        if (!string.IsNullOrEmpty(tx.OutputTo))              Params += ";I~S~OutputTo~"              + tx.OutputTo;
                                        if (!string.IsNullOrEmpty(tx.Account))               Params += ";I~S~Account~"               + tx.Account;
                                        if (tx.Amount                > 0)                    Params += ";I~S~Amount~"                + tx.Amount;
                                        if (tx.PointsRate            > 0)                    Params += ";I~S~PointsRate~"            + tx.PointsRate;
                                        if (!string.IsNullOrEmpty(tx.ExpiryType))            Params += ";I~S~ExpiryType~"            + tx.ExpiryType;
                                        if (tx.Expiry                > 0)                    Params += ";I~S~Expiry~"                + tx.Expiry;
                                        if (!string.IsNullOrEmpty(tx.StoryProperty))         Params += ";I~S~StoryProperty~"         + tx.StoryProperty;
                                        if (tx.StoryId               > 0)                    Params += ";I~S~StoryId~"               + tx.StoryId;
                                        if (tx.ActionTypeId          > 0)                    Params += ";I~S~ActionTypeId~"          + tx.ActionTypeId;
                                        if (tx.ActionStateId         > 0)                    Params += ";I~S~ActionStateId~"         + tx.ActionStateId;
                                        if (tx.ExpiryTypeId          > 0)                    Params += ";I~S~ExpiryTypeId~"          + tx.ExpiryTypeId;
                                        if (tx.AdjustmentTypeId      > 0)                    Params += ";I~S~AdjustmentTypeId~"      + tx.AdjustmentTypeId;
                                        if (tx.TriggerId             > 0)                    Params += ";I~S~TriggerId~"             + tx.TriggerId;
                                        if (tx.ActionId              > 0)                    Params += ";I~S~ActionId~"              + tx.ActionId;
                                        if (tx.DeviceId              > 0)                    Params += ";I~S~DeviceId~"              + tx.DeviceId;
                                        if (tx.WorkerId              > 0)                    Params += ";I~S~WorkerId~"              + tx.WorkerId;
                                        if (tx.MemberId              > 0)                    Params += ";I~S~MemberId~"              + tx.MemberId;
                                        if (!string.IsNullOrEmpty(tx.Inserted))              Params += ";I~S~Inserted~"              + tx.Inserted;
                                        if (!string.IsNullOrEmpty(tx.Modified))              Params += ";I~S~Modified~"              + tx.Modified;
                                        if (tx.MinuteId              > 0)                    Params += ";I~S~MinuteId~"              + tx.MinuteId;
                                        if (tx.DayId                 > 0)                    Params += ";I~S~DayId~"                 + tx.DayId;
                                        if (tx.WeekId                > 0)                    Params += ";I~S~WeekId~"                + tx.WeekId;
                                        if (tx.MonthId               > 0)                    Params += ";I~S~MonthId~"               + tx.MonthId;
                                        if (tx.QuarterId             > 0)                    Params += ";I~S~QuarterId~"             + tx.QuarterId;
                                        if (tx.YearId                > 0)                    Params += ";I~S~YearId~"                + tx.YearId;
                                        if (tx.ShiftId               > 0)                    Params += ";I~S~ShiftId~"               + tx.ShiftId;
                                    }
                                    string TransactionId = db.ExecLookup("pro.mod_Transaction", Params);

                                    if (helpers.ToInt(TransactionId) >0)
                                        DT = db.GetDataTable("pro.lst_Transaction", "I~S~TransactionId~" + TransactionId);
                                    else response.Response = db.DataError;
                                }

                                if (DT.Rows.Count > 0)
                                {
                                    if      (DT.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError)) response.Response = "No Records Found";
                                    else if (DT.Rows.Count>0)
                                    {
                                        response.Status = "Valid";
                                        response.Response = "[" + DT.Rows.Count.ToString() + "] Records Found";

                                        nexus.shared.promo.Transaction[] Transactions = new nexus.shared.promo.Transaction[DT.Rows.Count];

                                        for (int i = 0; i < DT.Rows.Count; i++)
                                        {
                                            nexus.shared.promo.Transaction Transaction = new nexus.shared.promo.Transaction
                                            {
                                                TransactionId         = helpers.ToInt(     DT.Rows[i]["TransactionPk"]      .ToString()),
                                                Promotion             =                    DT.Rows[i]["Promotion"]          .ToString(),
                                                PromotionType         =                    DT.Rows[i]["PromotionType"]      .ToString(),
                                                TransactionState      =                    DT.Rows[i]["TransactionState"]   .ToString(),
                                                MinValue              = helpers.ToDecimal( DT.Rows[i]["MinValue"]           .ToString()),
                                                MaxValue              = helpers.ToDecimal( DT.Rows[i]["MaxValue"]           .ToString()),
                                                TransactionStateId    = helpers.ToInt(     DT.Rows[i]["TransactionStateId"] .ToString()),
                                                PromotionId           = helpers.ToInt(     DT.Rows[i]["PromotionId"]        .ToString()),
                                                ParentId              = helpers.ToInt(     DT.Rows[i]["ParentId"]           .ToString()),

                                                Trigger               =                    DT.Rows[i]["Trigger"]            .ToString(),
                                                Action                =                    DT.Rows[i]["Action"]             .ToString(),
                                                Device                =                    DT.Rows[i]["Device"]             .ToString(),
                                                Worker                =                    DT.Rows[i]["Worker"]             .ToString(),
                                                Member                =                    DT.Rows[i]["Member"]             .ToString(),
                                                TriggerValue          = helpers.ToDecimal( DT.Rows[i]["TriggerValue"]       .ToString()),
                                                TriggerCount          = helpers.ToInt(     DT.Rows[i]["TriggerCount"]       .ToString()),
                                                TriggerDefn           =                    DT.Rows[i]["TriggerDefn"]        .ToString(),
                                                AdjustmentType        =                    DT.Rows[i]["AdjustmentType"]     .ToString(),
                                                Adjustment            = helpers.ToDecimal( DT.Rows[i]["Adjustment"]         .ToString()),
                                                Expires               =                    DT.Rows[i]["Expires"]            .ToString(),
                                                Taken                 =                    DT.Rows[i]["Taken"]              .ToString(),
                                                Starts                =                    DT.Rows[i]["Starts"]             .ToString(),
                                                Finishes              =                    DT.Rows[i]["Finishes"]           .ToString(),
                                                ActionDefn            =                    DT.Rows[i]["ActionDefn"]         .ToString(),
                                                Prize                 = helpers.ToInt(     DT.Rows[i]["Prize"]              .ToString()),
                                                OutputTo              =                    DT.Rows[i]["OutputTo"]           .ToString(),
                                                Account               =                    DT.Rows[i]["Account"]            .ToString(),
                                                Amount                = helpers.ToDecimal( DT.Rows[i]["Amount"]             .ToString()),
                                                PointsRate            = helpers.ToDecimal( DT.Rows[i]["PointsRate"]         .ToString()),
                                                ExpiryType            =                    DT.Rows[i]["ExpiryType"]         .ToString(),
                                                Expiry                = helpers.ToInt(     DT.Rows[i]["Expiry"]             .ToString()),
                                                StoryProperty         =                    DT.Rows[i]["StoryProperty"]      .ToString(),
                                                StoryId               = helpers.ToInt(     DT.Rows[i]["StoryId"]            .ToString()),
                                                ActionTypeId          = helpers.ToInt(     DT.Rows[i]["ActionTypeId"]       .ToString()),
                                                ActionStateId         = helpers.ToInt(     DT.Rows[i]["ActionStateId"]      .ToString()),
                                                ExpiryTypeId          = helpers.ToInt(     DT.Rows[i]["ExpiryTypeId"]       .ToString()),
                                                AdjustmentTypeId      = helpers.ToInt(     DT.Rows[i]["AdjustmentTypeId"]   .ToString()),
                                                TriggerId             = helpers.ToInt(     DT.Rows[i]["TriggerId"]          .ToString()),
                                                ActionId              = helpers.ToInt(     DT.Rows[i]["ActionId"]           .ToString()),
                                                DeviceId              = helpers.ToInt(     DT.Rows[i]["DeviceId"]           .ToString()),
                                                WorkerId              = helpers.ToInt(     DT.Rows[i]["WorkerId"]           .ToString()),
                                                MemberId              = helpers.ToInt(     DT.Rows[i]["MemberId"]           .ToString()),
                                                Inserted              =                    DT.Rows[i]["Inserted"]           .ToString(),
                                                Modified              =                    DT.Rows[i]["Modified"]           .ToString(),
                                                MinuteId              = helpers.ToInt(     DT.Rows[i]["MinuteId"]           .ToString()),
                                                DayId                 = helpers.ToInt(     DT.Rows[i]["DayId"]              .ToString()),
                                                WeekId                = helpers.ToInt(     DT.Rows[i]["WeekId"]             .ToString()),
                                                MonthId               = helpers.ToInt(     DT.Rows[i]["MonthId"]            .ToString()),
                                                QuarterId             = helpers.ToInt(     DT.Rows[i]["QuarterId"]          .ToString()),
                                                YearId                = helpers.ToInt(     DT.Rows[i]["YearId"]             .ToString()),
                                                ShiftId               = helpers.ToInt(     DT.Rows[i]["ShiftId"]            .ToString())
                                            };

                                            Transactions[i] = Transaction;
                                        }
                                        response.Transactions = Transactions;

                                        if (response.Status.Equals("Valid")) return Ok(response);

                                    }
                                }
                                else response.Response = db.DataError;
                            }
                            else response.Response = DT.Rows[0]["ReturnDesc"].ToString();
                        }
                        else response.Response = db.DataError;
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Transaction Err:" + ex.Message);
                    response.Response = ex.Message;
                }
            }

            return BadRequest(response);
        }
    }
}
