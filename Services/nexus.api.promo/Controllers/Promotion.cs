
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
    public class apiPromotion : ControllerBase
    {
        [HttpPost("Promotion")]
        [Authorize]
        public IActionResult Post([FromBody] PromotionRequest request)
        {

            PromotionResponse response = new PromotionResponse()
            {
                Status   = "Error",
                Response = "Request Poorly formatted"
            };

            if (request!=null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {
                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;
                               Params += ";I~S~QuietMode~0;I~S~ActionTypes~0";

                        DataTable DT = db.GetDataTable("xta.ValidateUse", Params);
                        if (DT.Rows.Count>0)
                        {
                            if (helpers.ToInt(DT.Rows[0]["ReturnCode"].ToString()).Equals(0))
                            {
                                Params = "";
                                if (request.Promotion != null)
                                {
                                    if                       (request.Promotion.PromotionId > 0) Params += ";I~S~PromotionId~" +    request.Promotion.PromotionId;
                                    if (!string.IsNullOrEmpty(request.Promotion.PromotionType))  Params += ";I~S~PromotionType~" +  request.Promotion.PromotionType;
                                    if (!string.IsNullOrEmpty(request.Promotion.PromotionState)) Params += ";I~S~PromotionState~" + request.Promotion.PromotionState;
                                }

                                if (request.ActionType.Equals(0))    DT = db.GetDataTable("pro.lst_Promotion", Params);
                                else
                                {
                                    if (request.Promotion != null)
                                    {
                                        if (!string.IsNullOrEmpty(request.Promotion.Description))      Params += ";I~S~Description~"    + request.Promotion.Description;
                                        if (request.Promotion.BudgetId > 0)                            Params += ";I~S~BudgetId~"      + request.Promotion.BudgetId;
                                        if (request.Promotion.AdvertId > 0)                            Params += ";I~S~AdvertId~"      + request.Promotion.AdvertId;
                                        if (request.Promotion.StartPageId > 0)                         Params += ";I~S~StartPageId~"   + request.Promotion.StartPageId;
                                        if (request.Promotion.PresenterId > 0)                         Params += ";I~S~PresenterId~"   + request.Promotion.PresenterId;
                                        if (request.Promotion.AwardId > 0)                             Params += ";I~S~AwardId~"       + request.Promotion.AwardId;
                                        if (request.Promotion.SponsorId > 0)                           Params += ";I~S~SponsorId~"     + request.Promotion.SponsorId;
                                        if (request.Promotion.GameTimeout > 0)                         Params += ";I~S~GameTimeout~"   + request.Promotion.GameTimeout;
                                        if (request.Promotion.MaxDraws > 0)                            Params += ";I~S~MaxDraws~"      + request.Promotion.MaxDraws;
                                        if (request.Promotion.AutoDraw.HasValue)                       Params += ";I~S~AutoDraw~"      + request.Promotion.AutoDraw.Value;
                                        if (request.Promotion.DrawInterval > 0)                        Params += ";I~S~DrawInterval~"  + request.Promotion.DrawInterval;
                                        if (request.Promotion.MaxZones > 0)                            Params += ";I~S~MaxZones~"      + request.Promotion.MaxZones;
                                        if (request.Promotion.PoolId > 0)                              Params += ";I~S~PoolId~"        + request.Promotion.PoolId;
                                        if (request.Promotion.TargetId > 0)                            Params += ";I~S~TargetId~"      + request.Promotion.TargetId;
                                        if (request.Promotion.PromotionTypeId > 0)                     Params += ";I~S~PromotionTypeId~"  + request.Promotion.PromotionTypeId;
                                        if (request.Promotion.PromotionStateId > 0)                    Params += ";I~S~PromotionStateId~" + request.Promotion.PromotionStateId;

                                        Params += ScheduleHelpers.GetParams(request.Promotion.Schedule);
                                    }

                                    string PromoId = db.ExecLookup("pro.mod_Promotion", Params);

                                    if (helpers.ToInt(PromoId)>0)
                                        DT = db.GetDataTable("pro.lst_Promotion", "I~S~PromotionId~" + PromoId);
                                    else response.Response = db.DataError;
                                }

                                if (DT.Rows.Count > 0)
                                {
                                    if      (DT.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError)) response.Response = "No Records Found";
                                    else if (DT.Rows.Count>0)
                                    {
                                        response.Status = "Valid";
                                        response.Response = "[" + DT.Rows.Count.ToString() + "] Records Found";

                                        Promotion[] Promotions = new Promotion[DT.Rows.Count];

                                        for (int i = 0; i < DT.Rows.Count; i++)
                                        {
                                            Promotion Promotion = new Promotion
                                            {
                                                PromotionId        = helpers.ToInt     (DT.Rows[i]["PromotionPk"].ToString()),
                                                PromotionType      =                   DT.Rows[i]["PromotionType"].ToString(),
                                                PromotionState     =                   DT.Rows[i]["PromotionState"].ToString(),
                                                Description        =                   DT.Rows[i]["Description"].ToString(),
                                                Budget             =                   DT.Rows[i]["ProBudget"].ToString(),
                                                BudgetId           = helpers.ToInt    (DT.Rows[i]["BudgetId"].ToString()),
                                                Advert             =                   DT.Rows[i]["Advert"].ToString(),
                                                AdvertId           = helpers.ToInt    (DT.Rows[i]["AdvertId"].ToString()),
                                                Award              =                   DT.Rows[i]["Award"].ToString(),
                                                AwardId            = helpers.ToInt    (DT.Rows[i]["AwardId"].ToString()),

                                                Trigger            =                   DT.Rows[i]["Trigger"].ToString(),
                                                TriggerType        =                   DT.Rows[i]["TriggerType"].ToString(),
                                                TriggerValue       = helpers.ToDecimal(DT.Rows[i]["TriggerValue"].ToString()),
                                                TriggerId          = helpers.ToInt    (DT.Rows[i]["TriggerId"].ToString()),

                                                Action             =                   DT.Rows[i]["Action"].ToString(),
                                                ActionType         =                   DT.Rows[i]["ActionType"].ToString(),
                                                ActionAmount       = helpers.ToDecimal(DT.Rows[i]["ActionAmount"].ToString()),
                                                ActionId           = helpers.ToInt    (DT.Rows[i]["ActionId"].ToString()),


                                                // Optional custom fields (placeholders kept as-is)
                                                StartPage =               DT.Rows[i]["StartPage"]?.ToString(),
                                                //StartPageId        = helpers.ToInt(DT.Rows[i]["StartPageId"]?.ToString()),
                                                Presenter          =               DT.Rows[i]["Presentor"]?.ToString(),
                                                //PresenterId        = helpers.ToInt(DT.Rows[i]["PresenterId"]?.ToString()),
                                                Sponsor            =               DT.Rows[i]["Sponsor"]?.ToString(),
                                                //SponsorId          = helpers.ToInt(DT.Rows[i]["SponsorId"]?.ToString()),

                                                GameTimeout        = helpers.ToInt(DT.Rows[i]["GameTimeout"]?.ToString()),
                                                MaxDraws           = helpers.ToInt(DT.Rows[i]["MaxDraws"]?.ToString()),
                                                AutoDraw           =               DT.Rows[i]["AutoDraw"]?.ToString().Equals("true", StringComparison.OrdinalIgnoreCase),
                                                DrawInterval      = helpers.ToInt(DT.Rows[i]["DrawInterval"]?.ToString()),
                                                MaxZones           = helpers.ToInt(DT.Rows[i]["MaxZones"]?.ToString()),
                                                PoolId             = helpers.ToInt(DT.Rows[i]["PoolId"]?.ToString()),
                                                TargetId           = helpers.ToInt(DT.Rows[i]["TargetId"]?.ToString()),
                                                PromotionTypeId    = helpers.ToInt(DT.Rows[i]["PromotionTypeId"]?.ToString()),
                                                PromotionStateId   = helpers.ToInt(DT.Rows[i]["PromotionStateId"]?.ToString()),
                                            };

                                            if (helpers.ToInt(DT.Rows[i]["ScheduleId"].ToString())>0)
                                                Promotion.Schedule = ScheduleHelpers.LoadFromRow(DT.Rows[i]);
                                            else
                                            {
                                                Promotion.Schedule.ScheduleDesc = "Runs Always, No Schedule";
                                                Promotion.Schedule.NextRun      = DateTime.Now.ToString("dd MMM, yyyy HH:mm:ss");
                                            }

                                            Promotions[i] = Promotion;
                                        }
                                        response.Promotions = Promotions;

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
                    helpers.WriteToLog("Pool Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }
    }
}
