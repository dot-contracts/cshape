
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.promo;

namespace nexus.api.promo
{
    [Route("pro")]
    public class apiPoolItem : ControllerBase
    {
        [HttpPost("PoolItem")]
        [Authorize]
        public IActionResult Post([FromBody] PoolItemRequest request)
        {

            PoolItemResponse proRes = new PoolItemResponse()
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

                        diag = "1";
                        if (!string.IsNullOrEmpty(request.PoolItem.CardNo))        Params += ";I~S~CardNo~"        + request.PoolItem.CardNo;
                        if (!string.IsNullOrEmpty(request.PoolItem.BadgeNo))       Params += ";I~S~BadgeNo~"       + request.PoolItem.BadgeNo;
                        if (!string.IsNullOrEmpty(request.PoolItem.PoolItemState)) Params += ";I~S~PoolItemState~" + request.PoolItem.PoolItemState;

                        diag = "2";
                        if (request.PoolItem.PoolItemId>0)                         Params += ";I~S~PoolItemId~"    + request.PoolItem.PoolItemId;
                        if (request.PoolItem.MemberId>0)                           Params += ";I~S~MemberId~"      + request.PoolItem.MemberId;
                        if (request.PoolItem.PromotionId>0)                        Params += ";I~S~PromotionId~"   + request.PoolItem.PromotionId;
                        if (request.PoolItem.PoolId>0)                             Params += ";I~S~PoolId~"        + request.PoolItem.PoolId;
                        if (request.PoolItem.TransactionId>0)                      Params += ";I~S~TransactionId~" + request.PoolItem.TransactionId;

                        diag = "3";
                        helpers.WriteToLog("PoolItem:" + Params);
                        DataTable dt = db.GetDataTable("[xta].[HandlePromoPoolItem]", Params);

                        if (dt.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError))
                        {
                            proRes.Status   = "Error";
                            proRes.Response = "No Records Found";
                        }
                        else if (dt.Rows.Count.Equals(0) && !string.IsNullOrEmpty(db.DataError))
                        {
                            proRes.Status   = "Error";
                            proRes.Response = db.DataError;
                        }
                        else if (dt.Rows.Count>0)
                        {

                            proRes.Status   = dt.Rows[0]["Status"].ToString();
                            proRes.Response = dt.Rows[0]["Response"].ToString();

                            diag = "4";
                            PoolItem[] PoolItems = new PoolItem[dt.Rows.Count];

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                diag = "5" + i.ToString();

                                PoolItem PoolItem = new PoolItem
                                {
                                    PoolItemId     = helpers.ToInt( dt.Rows[i]["PoolItemId"].ToString()),
                                    PoolItemState  =                dt.Rows[i]["PoolItemState"].ToString(),
                                    Entry          =                dt.Rows[i]["Entry"].ToString(),
                                    DrawDate       =                dt.Rows[i]["DrawDate"].ToString(),
                                    Member         =                dt.Rows[i]["Member"].ToString(),
                                    BadgeNo        =                dt.Rows[i]["BadgeNo"].ToString(),
                                    EGM            =                dt.Rows[i]["EGM"].ToString(),
                                    PromotionId    = helpers.ToInt( dt.Rows[i]["PromotionId"].ToString()),
                                    PoolId         = helpers.ToInt( dt.Rows[i]["PoolId"].ToString()),
                                    TransactionId  = helpers.ToInt( dt.Rows[i]["TransactionId"].ToString()),
                                    MemberId       = helpers.ToInt( dt.Rows[i]["MemberId"].ToString()),
                                    Sequence       = helpers.ToInt( dt.Rows[i]["Sequence"].ToString()),
                                    Entries        = helpers.ToInt( dt.Rows[i]["Entries"].ToString()),
                                    EgmId          = helpers.ToInt( dt.Rows[i]["LocationId"].ToString())
                                };
                                PoolItems[i] = PoolItem;
                            }
                            proRes.PoolItems = PoolItems;

                            if (proRes.Status.Equals("Valid")) return Ok(proRes);
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("PoolItem Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(proRes);
        }
    }
}
