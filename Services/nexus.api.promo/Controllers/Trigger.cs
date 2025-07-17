
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
    public class apiTrigger : ControllerBase
    {
        [HttpPost("Trigger")]
        [Authorize]
        public IActionResult Post([FromBody] TriggerRequest request)
        {

            TriggerResponse response = new TriggerResponse()
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
                               Params += ";I~S~Vendor~"    + request.Vendor;
                               Params += ";I~S~Location~"  + request.Location;
                               Params += ";I~S~QuietMode~0;I~S~ActionTypes~0";

                        DataTable DT = db.GetDataTable("xta.ValidateUse", Params);
                        if (DT.Rows.Count > 0)
                        {
                            if (helpers.ToInt(DT.Rows[0]["ReturnCode"].ToString()).Equals(0))
                            {
                                Params = "";
                                if (request.Trigger.TriggerId>0)                         Params += ";I~S~TriggerId~"    + request.Trigger.TriggerId;
                                if (!string.IsNullOrEmpty(request.Trigger.TriggerType))  Params += ";I~S~TriggerType~"  + request.Trigger.TriggerTypeId;
                                if (!string.IsNullOrEmpty(request.Trigger.TriggerState)) Params += ";I~S~TriggerState~" + request.Trigger.TriggerStateId;

                                if (request.ActionType.Equals(0))    DT = db.GetDataTable("pro.lst_Trigger", Params);
                                else
                                {
                                    if (request.Trigger != null && request.Trigger.HasData())
                                    {
                                        if (request.Trigger.TriggerId > 0)                            Params += ";I~S~TriggerId~"         + request.Trigger.TriggerId;
                                        if (!string.IsNullOrEmpty(request.Trigger.Promotion))         Params += ";I~S~Promotion~"         + request.Trigger.Promotion;
                                        if (!string.IsNullOrEmpty(request.Trigger.PromotionType))     Params += ";I~S~PromotionType~"     + request.Trigger.PromotionType;
                                        if (!string.IsNullOrEmpty(request.Trigger.PromotionState))    Params += ";I~S~PromotionState~"    + request.Trigger.PromotionState;
                                        if (!string.IsNullOrEmpty(request.Trigger.TriggerType))       Params += ";I~S~TriggerType~"       + request.Trigger.TriggerType;
                                        if (!string.IsNullOrEmpty(request.Trigger.TriggerState))      Params += ";I~S~TriggerState~"      + request.Trigger.TriggerState;
                                        if (!string.IsNullOrEmpty(request.Trigger.TriggerDefn))       Params += ";I~S~TriggerDefn~"       + request.Trigger.TriggerDefn;
                                        if (!string.IsNullOrEmpty(request.Trigger.Description))       Params += ";I~S~Description~"       + request.Trigger.Description;
                                        if (request.Trigger.MinValue > 0)                             Params += ";I~S~MinValue~"          + request.Trigger.MinValue;
                                        if (request.Trigger.MaxValue > 0)                             Params += ";I~S~MaxValue~"          + request.Trigger.MaxValue;
                                        if (request.Trigger.TriggerTypeId > 0)                        Params += ";I~S~TriggerTypeId~"     + request.Trigger.TriggerTypeId;
                                        if (request.Trigger.TriggerStateId > 0)                       Params += ";I~S~TriggerStateId~"    + request.Trigger.TriggerStateId;
                                        if (request.Trigger.PromotionId > 0)                          Params += ";I~S~PromotionId~"       + request.Trigger.PromotionId;
                                        if (request.Trigger.Sequence > 0)                             Params += ";I~S~Sequence~"          + request.Trigger.Sequence;
                                        if (request.Trigger.Sharing > 0)                              Params += ";I~S~Sharing~"           + request.Trigger.Sharing;
                                        if (request.Trigger.ShareState > 0)                           Params += ";I~S~ShareState~"        + request.Trigger.ShareState;
                                        if (request.Trigger.Operability > 0)                          Params += ";I~S~Operability~"       + request.Trigger.Operability;
                                        if (request.Trigger.Logging > 0)                              Params += ";I~S~Logging~"           + request.Trigger.Logging;
                                        if (request.Trigger.ScheduleId > 0)                           Params += ";I~S~ScheduleId~"        + request.Trigger.ScheduleId;
                                        if (request.Trigger.SystemId > 0)                             Params += ";I~S~SystemId~"          + request.Trigger.SystemId;
                                        if (request.Trigger.VenueId > 0)                              Params += ";I~S~VenueId~"           + request.Trigger.VenueId;
                                        if (request.Trigger.ParentId > 0)                             Params += ";I~S~ParentId~"          + request.Trigger.ParentId;
                                        if (request.Trigger.MediaId > 0)                              Params += ";I~S~MediaId~"           + request.Trigger.MediaId;
                                        if (request.Trigger.OwnerId > 0)                              Params += ";I~S~OwnerId~"           + request.Trigger.OwnerId;
                                        if (request.Trigger.LogId > 0)                                Params += ";I~S~LogId~"             + request.Trigger.LogId;
                                    }
                                    string TriggerId = db.ExecLookup("pro.mod_Trigger", Params);

                                    if (helpers.ToInt(TriggerId) >0)
                                        DT = db.GetDataTable("pro.lst_Trigger", "I~S~TriggerId~" + TriggerId);
                                    else response.Response = db.DataError;
                                }

                                if (DT.Rows.Count > 0)
                                {
                                    if      (DT.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError)) response.Response = "No Records Found";
                                    else if (DT.Rows.Count>0)
                                    {
                                        response.Status = "Valid";
                                        response.Response = "[" + DT.Rows.Count.ToString() + "] Records Found";

                                        nexus.shared.promo.Trigger[] Triggers = new nexus.shared.promo.Trigger[DT.Rows.Count];

                                        for (int i = 0; i < DT.Rows.Count; i++)
                                        {
                                            nexus.shared.promo.Trigger Trigger = new nexus.shared.promo.Trigger
                                            {
                                                TriggerId         = helpers.ToInt(     DT.Rows[i]["TriggerPk"].ToString()),
                                                Promotion         =                    DT.Rows[i]["Promotion"].ToString(),
                                                PromotionType     =                    DT.Rows[i]["PromotionType"].ToString(),
                                                PromotionState    =                    DT.Rows[i]["PromotionState"].ToString(),
                                                TriggerType       =                    DT.Rows[i]["TriggerType"].ToString(),
                                                TriggerState      =                    DT.Rows[i]["TriggerState"].ToString(),
                                                TriggerDefn       =                    DT.Rows[i]["TriggerDefn"].ToString(),
                                                Description       =                    DT.Rows[i]["Trigger"].ToString(),
                                                MinValue          = helpers.ToDecimal( DT.Rows[i]["MinValue"].ToString()),
                                                MaxValue          = helpers.ToDecimal( DT.Rows[i]["MaxValue"].ToString()),
                                                TriggerTypeId     = helpers.ToInt(     DT.Rows[i]["TriggerTypeId"].ToString()),
                                                TriggerStateId    = helpers.ToInt(     DT.Rows[i]["TriggerStateId"].ToString()),
                                                PromotionId       = helpers.ToInt(     DT.Rows[i]["PromotionId"].ToString()),
                                                Sequence          = helpers.ToInt(     DT.Rows[i]["Sequence"].ToString()),
                                                Sharing           = helpers.ToInt(     DT.Rows[i]["Sharing"].ToString()),
                                                ShareState        = helpers.ToInt(     DT.Rows[i]["ShareState"].ToString()),
                                                Operability       = helpers.ToInt(     DT.Rows[i]["Operability"].ToString()),
                                                Logging           = helpers.ToInt(     DT.Rows[i]["Logging"].ToString()),
                                                ScheduleId        = helpers.ToInt(     DT.Rows[i]["ScheduleId"].ToString()),
                                                SystemId          = helpers.ToInt(     DT.Rows[i]["SystemId"].ToString()),
                                                VenueId           = helpers.ToInt(     DT.Rows[i]["VenueId"].ToString()),
                                                ParentId          = helpers.ToInt(     DT.Rows[i]["ParentId"].ToString()),
                                                MediaId           = helpers.ToInt(     DT.Rows[i]["MediaId"].ToString()),
                                                OwnerId           = helpers.ToInt(     DT.Rows[i]["OwnerId"].ToString()),
                                                LogId             = helpers.ToInt(     DT.Rows[i]["LogId"].ToString())
                                            };
                                            Triggers[i] = Trigger;
                                        }
                                        response.Triggers = Triggers;

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
                    helpers.WriteToLog("Trigger Err:" + ex.Message);
                    response.Response = ex.Message;
                }
            }

            return BadRequest(response);
        }
    }
}
