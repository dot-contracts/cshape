
using System.Text;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.gaming;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace nexus.api.gaming
{
    [Route("gam")]
    public class apiPlayer : ControllerBase
    {
        [HttpPost("Player")]
        [Authorize]
        public IActionResult Post([FromBody] PlayerRequest request)
        {
            PlayerResponse response = new PlayerResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string SQL = "";

                        if (request.ActionType.Equals(0))
                            SQL = "select " + ( request.LimitRows>0? " Top " + request.LimitRows.ToString() : "") + " m.badgeno + ', ' + e.Description as Name, o.Description as Tier, p.TurnOver, p.AvgBet, p.Duration, p.CreditSpent, p.CreditEarned, l.Description as LastPlayedAt, c.HumanId, p.PlayTime " +
                                  "from acur.player p left join loc.card c on p.cardid = c.CardPK left join loc.human_member m on c.humanid = m.MemberPK left join loc.entity e on c.humanid = e.EntityPK left join loc.entity o on c.CohortId = o.EntityPK left join loc.entity l on p.LastPlayedAt = l.EntityPK " +
                                  "where isnull(m.BadgeNo ,'')<> '' order by Turnover Desc";
                        else
                            SQL = "select " + (request.LimitRows > 0 ? " Top " + request.LimitRows.ToString() : "") + " m.badgeno + ', ' + e.Description as Name, o.Description as Tier, p.TurnOver, p.AvgBet, p.Duration, 0 as CreditSpent, 0 as CreditEarned, l.Description as LastPlayedAt, c.HumanId, p.PlayTime " +
                                  "from acur.card p left join loc.card c on p.cardid = c.CardPK left join loc.human_member m on c.humanid = m.MemberPK left join loc.entity e on c.humanid = e.EntityPK left join loc.entity o on c.CohortId = o.EntityPK left join loc.entity l on c.LastUsedAt = l.EntityPK " +
                                  "where isnull(m.BadgeNo ,'')<> '' order by Turnover Desc";

                        DataTable DT = db.GetDataTable(SQL);

                        if (DT.Rows.Count > 0)
                        {
                            response.Status   = "Valid";
                            response.Response = "";

                            Player[] Results = new Player[DT.Rows.Count];
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                Player Result = new Player()
                                {
                                    Name     =                DT.Rows[i]["Name"].ToString(),
                                    Tier     =                DT.Rows[i]["Tier"].ToString(),
                                    Turnover = helpers.ToDbl (DT.Rows[i]["Turnover"].ToString()),
                                    AvgBet   = helpers.ToDbl (DT.Rows[i]["AvgBet"].ToString()),
                                    Duration =                DT.Rows[i]["Duration"].ToString(),
                                    Spent    = helpers.ToDbl (DT.Rows[i]["CreditSpent"].ToString()),
                                    Earned   = helpers.ToDbl (DT.Rows[i]["CreditEarned"].ToString()),
                                    LastPlayedAt =            DT.Rows[i]["LastPlayedAt"].ToString(),
                                    HumanId  = helpers.ToInt (DT.Rows[i]["HumanId"].ToString()),
                                    PlayTime = helpers.ToInt (DT.Rows[i]["PlayTime"].ToString())
                                };
                                Results[i] = Result;
                            }
                            response.Results = Results;

                            if (response.Status.Equals("Valid")) return Ok(response);
                        }
                        else    response.Response = "No Data Found, " + db.DataError;

                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Player Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public PlayerResponse getPlayer(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            PlayerRequest request = new PlayerRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            PlayerResponse response = new PlayerResponse();

            apiPlayer api = new apiPlayer();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (PlayerResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (PlayerResponse)bad.Value;
            return response;
        }

    }
}
