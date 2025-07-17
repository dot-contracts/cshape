
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using nexus.web.auth.Cache;
using nexus.web.auth.Data;
using nexus.web.auth.Model;
using nexus.common;
using nexus.common.dal;

using nexus.api.Model.Local;
using nexus.api.Model.Membership;

namespace nexus.api.Controller.Membership
{
    [Route("mem")]
    public class apiBadge : ControllerBase
    {
        [HttpPost("Badge")]
        [Authorize]
        public IActionResult Post([FromBody] BadgeRequest request)
        {
            BadgeResponse bgRes = new BadgeResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))
            if (request != null)
            {
                string diag = "";
                try
                {
                    string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
                    using (SQLServer db = new SQLServer(conStr))
                    {
                        diag = "1";

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                            if (request.Badge != null)
                            {
                                Badge badge = request.Badge;
                                if (badge.HasData())
                                {
                                    if (!string.IsNullOrEmpty(badge.BadgeNo))    Params += ";I~S~BadgeNo~" +     badge.BadgeNo;
                                    if (!string.IsNullOrEmpty(badge.BadgeState)) Params += ";I~S~BadgeStatus~" + badge.BadgeState;
                                    if (!string.IsNullOrEmpty(badge.Member))     Params += ";I~S~Member~" +      badge.Member;
                                    diag = "2";

                                if (badge.MemberId > 0)     Params += ";I~S~HumanId~" +      badge.MemberId.ToString();
                                    if (badge.BadgeStateId > 0) Params += ";I~S~BadgeStateId~" +  badge.BadgeStateId.ToString();
                                }

                            diag = "3";
                            helpers.WriteToLog("apiBadge:" + Params);
                            DataTable dt = db.GetDataTable("mem.api_Badge", Params);
                            if (dt.Rows.Count > 0)
                            {
                                bgRes.Status   = dt.Rows[0]["Status"].ToString();
                                bgRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "4";
                                Badge[] Badges = new Badge[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    badge = new Badge()
                                    {
                                        BadgeNo      =               dt.Rows[i]["BadgeNo"].ToString(),
                                        BadgeState   =               dt.Rows[i]["BadgeState"].ToString(),
                                        Member       =               dt.Rows[i]["member"].ToString(),
                                        MemberId     = helpers.ToInt(dt.Rows[i]["memberId"].ToString()),
                                        BadgeStateId = helpers.ToInt(dt.Rows[i]["BadgeStateId"].ToString())
                                    };
                                    Badges[i] = badge;
                                }
                                bgRes.Badges = Badges;

                                if (bgRes.Status.Equals("Valid")) return Ok(bgRes);
                            }
                            else
                            {
                                bgRes.Status   = "Error";
                                bgRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Badge Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(bgRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public BadgeResponse getBadges(int ActionType, string Vendor, string Location, Badge[] Badges)
        {
            return getBadge(ActionType, Vendor, Location, Badges[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public BadgeResponse getBadge(int ActionType, string Vendor, string Location, Badge Badge)
        {
            BadgeRequest request = new BadgeRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Badge      = Badge
            };

            BadgeResponse response = new BadgeResponse();

            apiBadge api = new apiBadge();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (BadgeResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (BadgeResponse)bad.Value;
            return response;
        }
    }
}
