
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using nexus.web.auth.Cache;
using nexus.web.auth.Data;
using nexus.common;
using nexus.common.dal;
using nexus.api.Model.Membership;
//using StackExchange.Redis;

namespace nexus.web.auth.Controllers
{
    [Route("api/[controller]")]
    public class HandleMembership : ControllerBase
    {

        [HttpPost("Post")]
        [Authorize]
        public IActionResult Post([FromBody] HandleMembershipRequest request)
        {
            var currentUser = HttpContext.User;

            string responseMessage = "Bad Request";

            //if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
            //{
            //    DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
            //    spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            //}

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            setting.GetSettings();
            string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
            using (SQLServer db = new SQLServer(conStr))
            {

                string Params  =  "I~S~ActionType~" + request.ActionType;
                       Params += ";I~S~Vendor~"     + request.Vendor;
                       Params += ";I~S~Location~"   + request.Location;

                if (!string.IsNullOrEmpty(request.BadgeNo)) Params += ";I~S~BadgeNo~" + request.BadgeNo;
                if (!string.IsNullOrEmpty(request.CardNo )) Params += ";I~S~CardNo~"  + request.CardNo;

                DataTable dt = db.GetDataTable("xta.HandleMembership", Params);
                if (dt.Rows.Count > 0)
                {
                    HandleMembershipResponse response = new HandleMembershipResponse
                    {
                        CardNo      =                     dt.Rows[0]["CardNo"].ToString(),
                        BadgeNo     =                     dt.Rows[0]["BadgeNo"].ToString(),
                        FirstName   =                     dt.Rows[0]["FirstName"].ToString(),
                        LastName    =                     dt.Rows[0]["LastName"].ToString(),
                        Gender      =                     dt.Rows[0]["Gender"].ToString(),
                        BirthDate   = helpers.ToDate(     dt.Rows[0]["BirthDate"].ToString()),
                        FinancialTo = helpers.ToDate(     dt.Rows[0]["FinancialTo"].ToString()),
                        EMail       =                     dt.Rows[0]["EMail"].ToString(),
                        Mobile      =                     dt.Rows[0]["Mobile"].ToString(),
                        Address     =                     dt.Rows[0]["Address"].ToString(),
                        Suburb      =                     dt.Rows[0]["Suburb"].ToString(),
                        PostCode    =                     dt.Rows[0]["PostCode"].ToString(),
                        Promotion   =                     dt.Rows[0]["Promotion"].ToString(),
                        PointsRate  = helpers.ToDecimal(  dt.Rows[0]["PointsRate"].ToString()),
                        TierLevel   = helpers.ToInt    (  dt.Rows[0]["TierLevel"].ToString()),
                        Points      = helpers.ToDecimal ( dt.Rows[0]["Points"].ToString()),
                        Credit      = helpers.ToDecimal ( dt.Rows[0]["Credit"].ToString()),
                        Club        =                     dt.Rows[0]["Club"].ToString(),
                        ClubId      =                     dt.Rows[0]["ClubId"].ToString(),
                        ResponseCode= helpers.ToInt     ( dt.Rows[0]["ReturnCode"].ToString()),
                        Response    =                     dt.Rows[0]["ReturnDescription"].ToString()
                    };

                    string retDesc = dt.Rows[0]["ReturnDescription"].ToString();

                    if (retDesc.Contains("Valid")) return Ok      (response);
                    else                           return Accepted(response);
                }
                else
                {
                    responseMessage = "No Record Found";
                    return Ok(responseMessage);
                }
            }

            return BadRequest(responseMessage);

        }


    }
}
