
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
    public class apiWinner : ControllerBase
    {
        [HttpPost("Winner")]
        [Authorize]
        public IActionResult Post([FromBody] WinnerRequest request)
        {

            WinnerResponse response = new WinnerResponse()
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
                                if (request.Winner.WinnerId>0)                         Params += ";I~S~WinnerId~"    + request.Winner.WinnerId;
                                //if (!string.IsNullOrEmpty(request.Winner.WinnerType))  Params += ";I~S~WinnerType~"  + request.Winner.WinnerTypeId;
                                //if (!string.IsNullOrEmpty(request.Winner.WinnerState)) Params += ";I~S~WinnerState~" + request.Winner.WinnerStateId;

                                if (request.ActionType.Equals(0))    DT = db.GetDataTable("pro.lst_Winner", Params);
                                else
                                {
                                    if (request.Winner != null && request.Winner.HasData())
                                    {
                                        var win = request.Winner;

                                        if (win.WinnerId        > 0)                    Params += ";I~S~WinnerId~"       + win.WinnerId;
                                        if (!string.IsNullOrEmpty(win.WinDate))         Params += ";I~S~WinDate~"        + win.WinDate;
                                        if (!string.IsNullOrEmpty(win.WinState))        Params += ";I~S~WinState~"       + win.WinState;
                                        if (!string.IsNullOrEmpty(win.Entry))           Params += ";I~S~Entry~"          + win.Entry;
                                        if (!string.IsNullOrEmpty(win.DrawDate))        Params += ";I~S~DrawDate~"       + win.DrawDate;
                                        if (win.Sequence        > 0)                    Params += ";I~S~Sequence~"       + win.Sequence;
                                        if (!string.IsNullOrEmpty(win.Member))          Params += ";I~S~Member~"         + win.Member;
                                        if (!string.IsNullOrEmpty(win.BadgeNo))         Params += ";I~S~BadgeNo~"        + win.BadgeNo;
                                        if (!string.IsNullOrEmpty(win.EGM))             Params += ";I~S~EGM~"            + win.EGM;
                                        if (win.PoolId          > 0)                    Params += ";I~S~PoolId~"         + win.PoolId;
                                        if (win.PoolItemId      > 0)                    Params += ";I~S~PoolItemId~"     + win.PoolItemId;
                                        if (win.PromotionId     > 0)                    Params += ";I~S~PromotionId~"    + win.PromotionId;
                                        if (win.TransactionId   > 0)                    Params += ";I~S~TransactionId~"  + win.TransactionId;
                                        if (win.MemberId        > 0)                    Params += ";I~S~MemberId~"       + win.MemberId;
                                        if (win.LocationId      > 0)                    Params += ";I~S~LocationId~"     + win.LocationId;
                                        if (win.WinStateId      > 0)                    Params += ";I~S~WinStateId~"     + win.WinStateId;

                                    }
                                    string WinnerId = db.ExecLookup("pro.mod_Winner", Params);

                                    if (helpers.ToInt(WinnerId) >0)
                                        DT = db.GetDataTable("pro.lst_Winner", "I~S~WinnerId~" + WinnerId);
                                    else response.Response = db.DataError;
                                }

                                if (DT.Rows.Count > 0)
                                {
                                    if      (DT.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError)) response.Response = "No Records Found";
                                    else if (DT.Rows.Count>0)
                                    {
                                        response.Status = "Valid";
                                        response.Response = "[" + DT.Rows.Count.ToString() + "] Records Found";

                                        nexus.shared.promo.Winner[] Winners = new nexus.shared.promo.Winner[DT.Rows.Count];

                                        for (int i = 0; i < DT.Rows.Count; i++)
                                        {
                                            nexus.shared.promo.Winner Winner = new nexus.shared.promo.Winner
                                            {
                                                WinnerId         = helpers.ToInt(     DT.Rows[i]["WinnerPk"]       .ToString()),
                                                WinDate          =                    DT.Rows[i]["WinDate"]        .ToString(),
                                                WinState         =                    DT.Rows[i]["WinState"]       .ToString(),
                                                Entry            =                    DT.Rows[i]["Entry"]          .ToString(),
                                                DrawDate         =                    DT.Rows[i]["DrawDate"]       .ToString(),
                                                Sequence         = helpers.ToInt(     DT.Rows[i]["Sequence"]       .ToString()),
                                                Member           =                    DT.Rows[i]["Member"]         .ToString(),
                                                BadgeNo          =                    DT.Rows[i]["BadgeNo"]        .ToString(),
                                                EGM              =                    DT.Rows[i]["EGM"]            .ToString(),
                                                PoolId           = helpers.ToInt(     DT.Rows[i]["PoolId"]         .ToString()),
                                                PoolItemId       = helpers.ToInt(     DT.Rows[i]["PoolItemId"]     .ToString()),
                                                PromotionId      = helpers.ToInt(     DT.Rows[i]["PromotionId"]    .ToString()),
                                                TransactionId    = helpers.ToInt(     DT.Rows[i]["TransactionId"]  .ToString()),
                                                MemberId         = helpers.ToInt(     DT.Rows[i]["MemberId"]       .ToString()),
                                                LocationId       = helpers.ToInt(     DT.Rows[i]["LocationId"]     .ToString()),
                                                WinStateId       = helpers.ToInt(     DT.Rows[i]["WinStateId"]     .ToString())
                                            };
                                            Winners[i] = Winner;
                                        }
                                        response.Winners = Winners;

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
                    helpers.WriteToLog("Winner Err:" + ex.Message);
                    response.Response = ex.Message;
                }
            }

            return BadRequest(response);
        }
    }
}
