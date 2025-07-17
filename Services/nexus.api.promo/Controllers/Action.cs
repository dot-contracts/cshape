
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
    public class apiAction : ControllerBase
    {
        [HttpPost("Action")]
        [Authorize]
        public IActionResult Post([FromBody] ActionRequest request)
        {

            ActionResponse response = new ActionResponse()
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
                                if (request.Action.ActionId>0)                         Params += ";I~S~ActionId~"    + request.Action.ActionId;
                                if (!string.IsNullOrEmpty(request.Action.ActionType))  Params += ";I~S~ActionType~"  + request.Action.ActionTypeId;
                                if (!string.IsNullOrEmpty(request.Action.ActionState)) Params += ";I~S~ActionState~" + request.Action.ActionStateId;

                                if (request.ActionType.Equals(0))    DT = db.GetDataTable("pro.lst_Action", Params);
                                else
                                {
                                    if (request.Action != null && request.Action.HasData())
                                    {
                                        if (!string.IsNullOrEmpty(request.Action.Description))      Params += ";I~S~ActionDescription~"   + request.Action.Description;
                                        if (!string.IsNullOrEmpty(request.Action.ActionType))       Params += ";I~S~ActionType~"          + request.Action.ActionType;
                                        if (!string.IsNullOrEmpty(request.Action.ActionState))      Params += ";I~S~ActionState~"         + request.Action.ActionState;
                                        if (!string.IsNullOrEmpty(request.Action.ActionDefn))       Params += ";I~S~ActionDefn~"          + request.Action.ActionDefn;
                                        if (request.Action.Prize > 0)                               Params += ";I~S~Prize~"               + request.Action.Prize;
                                        if (!string.IsNullOrEmpty(request.Action.Account))          Params += ";I~S~Account~"             + request.Action.Account;
                                        if (request.Action.Amount > 0)                              Params += ";I~S~Amount~"              + request.Action.Amount;
                                        if (request.Action.PointsRate > 0)                          Params += ";I~S~PointsRate~"          + request.Action.PointsRate;
                                        if (!string.IsNullOrEmpty(request.Action.DelayType))        Params += ";I~S~DelayType~"           + request.Action.DelayType;
                                        if (request.Action.Delay > 0)                               Params += ";I~S~Delay~"               + request.Action.Delay;
                                        if (!string.IsNullOrEmpty(request.Action.ExpireType))       Params += ";I~S~ExpireType~"          + request.Action.ExpireType;
                                        if (request.Action.Expiry > 0)                              Params += ";I~S~Expiry~"              + request.Action.Expiry;
                                        if (request.Action.StoryId > 0)                             Params += ";I~S~StoryId~"             + request.Action.StoryId;
                                        if (!string.IsNullOrEmpty(request.Action.StoryProperty))    Params += ";I~S~StoryProperty~"       + request.Action.StoryProperty;
                                        if (!string.IsNullOrEmpty(request.Action.OutputType))       Params += ";I~S~OutputType~"          + request.Action.OutputType;
                                        if (!string.IsNullOrEmpty(request.Action.OutputTo))         Params += ";I~S~OutputTo~"            + request.Action.OutputTo;
                                        if (!string.IsNullOrEmpty(request.Action.Definition))       Params += ";I~S~Definition~"          + request.Action.Definition;
                                        if (request.Action.ActionId > 0)                            Params += ";I~S~ActionId~"            + request.Action.ActionId;
                                        if (request.Action.ActionTypeId > 0)                        Params += ";I~S~ActionTypeId~"        + request.Action.ActionTypeId;
                                        if (request.Action.ActionStateId > 0)                       Params += ";I~S~ActionStateId~"       + request.Action.ActionStateId;
                                        if (request.Action.DelayTypeId > 0)                         Params += ";I~S~DelayTypeId~"         + request.Action.DelayTypeId;
                                        if (request.Action.ExpiryTypeId > 0)                        Params += ";I~S~ExpiryTypeId~"        + request.Action.ExpiryTypeId;
                                        if (request.Action.PromotionId > 0)                         Params += ";I~S~PromotionId~"         + request.Action.PromotionId;
                                        if (request.Action.Sequence > 0)                            Params += ";I~S~Sequence~"            + request.Action.Sequence;
                                        if (request.Action.Sharing > 0)                             Params += ";I~S~Sharing~"             + request.Action.Sharing;
                                        if (request.Action.ShareState > 0)                          Params += ";I~S~ShareState~"          + request.Action.ShareState;
                                        if (request.Action.Operability > 0)                         Params += ";I~S~Operability~"         + request.Action.Operability;
                                        if (request.Action.Logging > 0)                             Params += ";I~S~Logging~"             + request.Action.Logging;
                                        if (request.Action.ScheduleId > 0)                          Params += ";I~S~ScheduleId~"          + request.Action.ScheduleId;
                                        if (request.Action.SystemId > 0)                            Params += ";I~S~SystemId~"            + request.Action.SystemId;
                                        if (request.Action.VenueId > 0)                             Params += ";I~S~VenueId~"             + request.Action.VenueId;
                                        if (request.Action.ParentId > 0)                            Params += ";I~S~ParentId~"            + request.Action.ParentId;
                                        if (request.Action.MediaId > 0)                             Params += ";I~S~MediaId~"             + request.Action.MediaId;
                                        if (request.Action.OwnerId > 0)                             Params += ";I~S~OwnerId~"             + request.Action.OwnerId;
                                        if (request.Action.LogId > 0)                               Params += ";I~S~LogId~"               + request.Action.LogId;
                                    }
                                    string ActionId = db.ExecLookup("pro.mod_Action", Params);

                                    if (helpers.ToInt(ActionId) >0)
                                        DT = db.GetDataTable("pro.lst_Action", "I~S~ActionId~" + ActionId);
                                    else response.Response = db.DataError;
                                }

                                if (DT.Rows.Count > 0)
                                {
                                    if      (DT.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError)) response.Response = "No Records Found";
                                    else if (DT.Rows.Count>0)
                                    {
                                        response.Status = "Valid";
                                        response.Response = "[" + DT.Rows.Count.ToString() + "] Records Found";

                                        nexus.shared.promo.Action[] Actions = new nexus.shared.promo.Action[DT.Rows.Count];

                                        for (int i = 0; i < DT.Rows.Count; i++)
                                        {
                                            nexus.shared.promo.Action Action = new nexus.shared.promo.Action
                                            {
                                                ActionId         = helpers.ToInt(      DT.Rows[i]["ActionPk"].ToString()),
                                                ActionType       =                     DT.Rows[i]["ActionType"].ToString(),
                                                ActionState      =                     DT.Rows[i]["ActionState"].ToString(),
                                                ActionDefn       =                     DT.Rows[i]["ActionDefn"].ToString(),
                                                Description      =                     DT.Rows[i]["Action"].ToString(),
                                                Prize            = helpers.ToInt(      DT.Rows[i]["Prize"].ToString()),
                                                Account          =                     DT.Rows[i]["Account"].ToString(),
                                                Amount           = helpers.ToDecimal(  DT.Rows[i]["Amount"].ToString()),
                                                PointsRate       = helpers.ToDecimal(  DT.Rows[i]["PointsRate"].ToString()),
                                                DelayType        =                     DT.Rows[i]["DelayType"].ToString(),
                                                Delay            = helpers.ToInt(      DT.Rows[i]["Delay"].ToString()),
                                                ExpireType       =                     DT.Rows[i]["ExpiryType"].ToString(),
                                                Expiry           = helpers.ToInt(      DT.Rows[i]["Expiry"].ToString()),
                                                StoryId          = helpers.ToInt(      DT.Rows[i]["StoryId"].ToString()),
                                                StoryProperty    =                     DT.Rows[i]["StoryProperty"].ToString(),
                                                OutputType       =                     DT.Rows[i]["OutputType"].ToString(),
                                                OutputTo         =                     DT.Rows[i]["OutputTo"].ToString(),
                                                Definition       =                     DT.Rows[i]["Definition"].ToString(),
                                                ActionTypeId     = helpers.ToInt(      DT.Rows[i]["ActionTypeId"].ToString()),
                                                ActionStateId    = helpers.ToInt(      DT.Rows[i]["ActionStateId"].ToString()),
                                                DelayTypeId      = helpers.ToInt(      DT.Rows[i]["DelayTypeId"].ToString()),
                                                ExpiryTypeId     = helpers.ToInt(      DT.Rows[i]["ExpiryTypeId"].ToString()),
                                                Promotion        =                     DT.Rows[i]["Promotion"].ToString(),
                                                PromotionId      = helpers.ToInt(      DT.Rows[i]["PromotionId"].ToString()),
                                                Sequence         = helpers.ToInt(      DT.Rows[i]["Sequence"].ToString()),
                                                Sharing          = helpers.ToInt(      DT.Rows[i]["Sharing"].ToString()),
                                                ShareState       = helpers.ToInt(      DT.Rows[i]["ShareState"].ToString()),
                                                Operability      = helpers.ToInt(      DT.Rows[i]["Operability"].ToString()),
                                                Logging          = helpers.ToInt(      DT.Rows[i]["Logging"].ToString()),
                                                ScheduleId       = helpers.ToInt(      DT.Rows[i]["ScheduleId"].ToString()),
                                                SystemId         = helpers.ToInt(      DT.Rows[i]["SystemId"].ToString()),
                                                VenueId          = helpers.ToInt(      DT.Rows[i]["VenueId"].ToString()),
                                                ParentId         = helpers.ToInt(      DT.Rows[i]["ParentId"].ToString()),
                                                MediaId          = helpers.ToInt(      DT.Rows[i]["MediaId"].ToString()),
                                                OwnerId          = helpers.ToInt(      DT.Rows[i]["OwnerId"].ToString()),
                                                LogId            = helpers.ToInt(      DT.Rows[i]["LogId"].ToString())
                                            };
                                            Actions[i] = Action;
                                        }
                                        response.Actions = Actions;

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
                    helpers.WriteToLog("Action Err:" + ex.Message);
                    response.Response = ex.Message;
                }
            }

            return BadRequest(response);
        }
    }
}
