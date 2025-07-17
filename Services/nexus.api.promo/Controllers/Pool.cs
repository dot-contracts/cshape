
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
    public class apiPool : ControllerBase
    {
        [HttpPost("Pool")]
        [Authorize]
        public IActionResult Post([FromBody] PoolRequest request)
        {

            PoolResponse proRes = new PoolResponse()
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
                        if (request.Pool.PoolId>0)                         Params += ";I~S~PoolId~"    + request.Pool.PoolId;
                        if (!string.IsNullOrEmpty(request.Pool.PoolType))  Params += ";I~S~PoolType~"  + request.Pool.PoolType;
                        if (!string.IsNullOrEmpty(request.Pool.PoolState)) Params += ";I~S~PoolState~" + request.Pool.PoolState;

                        diag = "2";
                        helpers.WriteToLog("Pool:" + Params);
                        DataTable dt = db.GetDataTable("pro.Pool", Params);

                        if (dt.Rows.Count.Equals(0) && string.IsNullOrEmpty(db.DataError))
                        {
                            proRes.Status   = "Error";
                            proRes.Response = "No Records Found";
                        }
                        else if (dt.Rows.Count>0)
                        {
                            diag = "3";
                            Pool[] Pools = new Pool[dt.Rows.Count];

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                diag = "4" + i.ToString();
                                Pool Pool = new Pool
                                {
                                    PoolId           = helpers.ToInt(      dt.Rows[0]["PoolId"].ToString()),
                                    PoolType         =                     dt.Rows[0]["PoolType"].ToString(),
                                    PoolState        =                     dt.Rows[0]["PoolState"].ToString(),
                                    Promotion        =                     dt.Rows[0]["Promotion"].ToString(),
                                    PromotionType    =                     dt.Rows[0]["PromotionType"].ToString(),
                                    Worker           =                     dt.Rows[0]["Worker"].ToString(),
                                    Start            =                     dt.Rows[0]["Start"].ToString(),
                                    Finish           =                     dt.Rows[0]["Finish"].ToString(),
                                    Ranges           =                     dt.Rows[0]["Ranges"].ToString(),
                                    StartRange       =                     dt.Rows[0]["StartRange"].ToString(),
                                    FinishRange      =                     dt.Rows[0]["FinishRange"].ToString(),
                                    PromotionId      = helpers.ToInt(      dt.Rows[0]["PromotionId"].ToString()),
                                    WorkerId         = helpers.ToInt(      dt.Rows[0]["WorkerId"].ToString()),
                                    PoolTypeId       = helpers.ToInt(      dt.Rows[0]["PoolTypeId"].ToString()),
                                    PoolStateId      = helpers.ToInt(      dt.Rows[0]["PoolStateId"].ToString()),
                                    ScheduleId       = helpers.ToInt(      dt.Rows[0]["ScheduleId"].ToString())
                                };

                                Pools[i] = Pool;
                            }
                            proRes.Pools = Pools;

                            if (proRes.Status.Equals("Valid")) return Ok(proRes);
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Pool Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(proRes);
        }
    }
}
