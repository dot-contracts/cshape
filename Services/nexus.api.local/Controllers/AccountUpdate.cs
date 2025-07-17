
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiAccountUpdate : ControllerBase
    {

        [HttpPost("AccountUpdate")]
        [Authorize]
        public IActionResult Post([FromBody] AccountUpdateRequest request)
        {
            //var currentUser = HttpContext.User;

            AccountUpdateResponse acctRes = new AccountUpdateResponse()
            {
                Status   = "Error",
                Response = "Request Poorly formatted"
            };

            //if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
            //{
            //    DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
            //    spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            //}

            if (request != null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;
                        string Vendor = Params;

                        if (!string.IsNullOrEmpty(request.BadgeNo)) Params += ";I~S~BadgeNo~" + request.BadgeNo;
                        if (!string.IsNullOrEmpty(request.CardNo )) Params += ";I~S~CardNo~"  + request.CardNo;
                        if (!string.IsNullOrEmpty(request.PinNo  )) Params += ";I~S~PinNo~"   + request.PinNo;
                        if (!string.IsNullOrEmpty(request.Account)) Params += ";I~S~Account~" + request.Account;
                        if (request.Amount > 0)                     Params += ";I~S~Amount~"  + request.Amount;

                        diag = "1";

                        helpers.WriteToLog("apiAccountUpdate:" + Params);
                        DataTable dt = db.GetDataTable("loc.api_AccountUpdate", Params);
                        if (dt.Rows.Count > 0)
                        {

                            acctRes.Status   = dt.Rows[0]["Status"].ToString();
                            acctRes.Response = dt.Rows[0]["Response"].ToString();

                            diag = "2";
                            acctRes.CardNo      =                     dt.Rows[0]["CardNo"].ToString();
                            acctRes.BadgeNo     =                     dt.Rows[0]["BadgeNo"].ToString();
                            acctRes.FirstName   =                     dt.Rows[0]["FirstName"].ToString();
                            acctRes.LastName    =                     dt.Rows[0]["LastName"].ToString();
                            acctRes.CardType    =                     dt.Rows[0]["CardType"].ToString();
                            acctRes.BirthDate   = helpers.ToDate(     dt.Rows[0]["BirthDate"].ToString()).ToString("dd MMM, yyyy");
                            acctRes.FinancialTo = helpers.ToDate(     dt.Rows[0]["FinancialTo"].ToString()).ToString("dd MMM, yyyy");
                            acctRes.Promotion   =                     dt.Rows[0]["Promotion"].ToString();
                            acctRes.PointsRate  = helpers.ToDecimal(  dt.Rows[0]["PointsRate"].ToString());
                            acctRes.TierLevel   = helpers.ToInt    (  dt.Rows[0]["TierLevel"].ToString());

                            diag = "3";
                            acctRes.GAMPoints   = helpers.ToDecimal(  dt.Rows[0]["GAMPoints"].ToString());
                            acctRes.GAMBonus    = helpers.ToDecimal(  dt.Rows[0]["GAMBonus"].ToString());
                            acctRes.NGMPoints   = helpers.ToDecimal(  dt.Rows[0]["NGMPoints"].ToString());
                            acctRes.NGMBonus    = helpers.ToDecimal(  dt.Rows[0]["NGMBonus"].ToString());
                            acctRes.PROPoints   = helpers.ToDecimal(  dt.Rows[0]["PROPoints"].ToString());
                            acctRes.Points      = helpers.ToDecimal ( dt.Rows[0]["Points"].ToString());

                            diag = "4";
                            acctRes.Credit      = helpers.ToDecimal ( dt.Rows[0]["Credit"].ToString());
                            acctRes.Award       = helpers.ToDecimal ( dt.Rows[0]["Award"].ToString());
                            acctRes.Saving      = helpers.ToDecimal ( dt.Rows[0]["Saving"].ToString());
                            acctRes.Credits     = helpers.ToDecimal ( dt.Rows[0]["Credits"].ToString());

                            if (acctRes.Status.Equals("Valid")) return Ok(acctRes);

                        }
                        else
                        {
                            acctRes.Status   = "Error";
                            acctRes.Response = "No Data Found, " + db.DataError;
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("AccountUpdt Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(acctRes);
        }
    }
}
