
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

namespace nexus.api.gaming
{
    [Route("gam")]
    public class apiCardPlay : ControllerBase
    {
        [HttpPost("CardPlay")]
        [Authorize]
        public IActionResult Post([FromBody] CardPlayRequest request)
        {
            CardPlayResponse response = new CardPlayResponse()
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
                        if (request.ActionType.Equals(3))
                        {
                            string SQL = "SELECT c.OpenDate, isnull(egm.Description,'') AS GameName, isnull(mem.badgeno,'') + ', ' + hum.Description AS FullName, crd.CardNo, " +
                                        "(CONVERT([varchar], dateadd(millisecond, DATEDIFF(ss, c.OpenDate, c.CloseDate) * (1000), (0)), (108))) AS Duration, (cmt.TurnOver - omt.TurnOver) AS TurnOver, " +
                                        "(cmt.Stroke - omt.Stroke) AS Stroke, cast(CASE WHEN (cmt.Stroke - omt.Stroke) <> 0 THEN cast(((cmt.TurnOver - omt.TurnOver) / (cmt.Stroke - omt.Stroke)) AS decimal(18, 2)) ELSE 0 END AS decimal(18, 2)) AS avgBet " +
                                        "FROM acur.Card AS c LEFT JOIN loc.entity AS egm ON c.LocationId = egm.EntityPk LEFT JOIN loc.card AS crd ON c.CardID = crd.CardPk LEFT JOIN acur.vue_Lineitem_EgmMeter AS omt ON c.OpenMeters = omt.MeterPK LEFT JOIN acur.vue_Lineitem_EgmMeter AS cmt ON c.CloseMeters = cmt.MeterPK LEFT JOIN " +
                                        "loc.entity AS hum ON crd.HumanId = hum.EntityPk LEFT JOIN loc.human_member AS mem ON hum.entitypk = mem.memberpk LEFT JOIN loc.device AS dd ON c.LocationId = dd.DevicePk " +
                                        "order by convert([varchar],dateadd(millisecond, DATEDIFF(ss, c.OpenDate, c.CloseDate) * 1000, 0), 108) desc";

                            DataTable dt = db.GetDataTable(SQL);
                            if (dt.Rows.Count > 0)
                            {
                                response.Status = "Valid";
                                response.Response = "";

                                CardPlay[] CardPlays = new CardPlay[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    CardPlay CardPlay = new CardPlay()
                                    {
                                        OpenDate       =               dt.Rows[i]["OpenDate"].ToString(),
                                        GameName       =               dt.Rows[i]["GameName"].ToString(),
                                        FullName       =               dt.Rows[i]["FullName"].ToString(),
                                        CardNo         =               dt.Rows[i]["CardNo"].ToString(),
                                        Duration       =               dt.Rows[i]["Duration"].ToString(),
                                        Turnover       = helpers.ToDbl(dt.Rows[i]["Turnover"].ToString()),
                                        Stroke         = helpers.ToDbl(dt.Rows[i]["Stroke"].ToString()),
                                        AvgBet         = helpers.ToDbl(dt.Rows[i]["AvgBet"].ToString()),
                                    };
                                    CardPlays[i] = CardPlay;
                                }
                                response.Results = CardPlays;

                                if (response.Status.Equals("Valid")) return Ok(response);
                            }
                            else
                            {
                                response.Status   = "Error";
                                response.Response = "No Data Found, " + db.DataError;
                            }

                        }
                        else
                        {
                            string Params = "I~S~ActionType~"   + request.ActionType;
                                   Params += ";I~S~Vendor~"     + request.Vendor;
                                   Params += ";I~S~Location~"   + request.Location;

                            if (!string.IsNullOrEmpty(request.CardNo))    Params += ";I~S~CardNo~" +    request.CardNo;
                            if (!string.IsNullOrEmpty(request.BadgeNo))   Params += ";I~S~BadgeNo~" +   request.BadgeNo;
                            if (!string.IsNullOrEmpty(request.HouseNo))   Params += ";I~S~HouseNo~" +   request.HouseNo;
                            if (!string.IsNullOrEmpty(request.OpenDate))  Params += ";I~S~OpenDate~" +  request.OpenDate;
                            if (!string.IsNullOrEmpty(request.CloseDate)) Params += ";I~S~CloseDate~" + request.CloseDate;

                            helpers.WriteToLog("apiCardPlay:" + Params);
                            DataTable dt = db.GetDataTable("xta.gam_Cardplay", Params);
                            if (dt.Rows.Count > 0)
                            {
                                response.Status = "Valid";
                                response.Response = "";

                                CardPlay[] CardPlays = new CardPlay[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    CardPlay CardPlay = new CardPlay()
                                    {
                                        BadgeNo        =               dt.Rows[i]["BadgeNo"].ToString(),
                                        CardNo         =               dt.Rows[i]["CardNo"].ToString(),
                                        FirstName      =               dt.Rows[i]["FirstName"].ToString(),
                                        LastName       =               dt.Rows[i]["LastName"].ToString(),
                                        FullName       =               dt.Rows[i]["FullName"].ToString(),
                                        BirthDate      =               dt.Rows[i]["BirthDate"].ToString(),
                                        MemberType     =               dt.Rows[i]["MemberType"].ToString(),
                                        MemberState    =               dt.Rows[i]["MemberState"].ToString(),
                                        FinancialState =               dt.Rows[i]["FinancialState"].ToString(),
                                        Promotion      =               dt.Rows[i]["Promotion"].ToString(),
                                        TierLevel      =               dt.Rows[i]["TierLevel"].ToString(),
                                        PointsRate     = helpers.ToDbl(dt.Rows[i]["PointsRate"].ToString()),
                                        LastPlay       =               dt.Rows[i]["LastPlay"].ToString(),
                                        Points         = helpers.ToDbl(dt.Rows[i]["Points"].ToString()),
                                        Credit         = helpers.ToDbl(dt.Rows[i]["Credit"].ToString()),
                                        HouseNo        =               dt.Rows[i]["HouseNo"].ToString(),
                                        GameName       =               dt.Rows[i]["GameName"].ToString(),
                                        OpenDate       =               dt.Rows[i]["OpenDate"].ToString(),
                                        Turnover       = helpers.ToDbl(dt.Rows[i]["Turnover"].ToString()),
                                        TotalWin       = helpers.ToDbl(dt.Rows[i]["TotalWin"].ToString()),
                                        Stroke         = helpers.ToDbl(dt.Rows[i]["Stroke"].ToString()),
                                        Duration       =               dt.Rows[i]["Duration"].ToString(),
                                        AvgBet         = helpers.ToDbl(dt.Rows[i]["AvgBet"].ToString()),
                                        PlayerTO       = helpers.ToDbl(dt.Rows[i]["PlayerTO"].ToString()),
                                        PlayerTW       = helpers.ToDbl(dt.Rows[i]["PlayerTW"].ToString()),
                                        PlayerStroke   = helpers.ToDbl(dt.Rows[i]["PlayerStroke"].ToString()),
                                        PlayerAvgBet   = helpers.ToDbl(dt.Rows[i]["PlayerAvgBet"].ToString()),
                                        PlayerDur      =               dt.Rows[i]["PlayerDur"].ToString(),
                                        TodayTO        = helpers.ToDbl(dt.Rows[i]["TodayTO"].ToString()),
                                        TodayTW        = helpers.ToDbl(dt.Rows[i]["TodayTW"].ToString()),
                                        TodayStroke    = helpers.ToDbl(dt.Rows[i]["TodayStroke"].ToString()),
                                        TodayAvgBet    = helpers.ToDbl(dt.Rows[i]["TodayAvgBet"].ToString()),
                                        TodayDur       =               dt.Rows[i]["TodayDur"].ToString(),
                                    };
                                    CardPlays[i] = CardPlay;
                                }
                                response.Results = CardPlays;

                                if (response.Status.Equals("Valid")) return Ok(response);
                            }
                            else
                            {
                                response.Status   = "Error";
                                response.Response = "No Data Found, " + db.DataError;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("CardPlay Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CardPlayResponse getCardPlay(int ActionType, string Vendor, string Location, string CardNo = "", string BadgeNo = "", string HouseNo = "", string OpenDate = "", string CloseDate = "")
        {
            CardPlayRequest request = new CardPlayRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                CardNo     = CardNo,
                BadgeNo    = BadgeNo,
                HouseNo    = HouseNo,
                OpenDate   = OpenDate,
                CloseDate  = CloseDate
            };

            CardPlayResponse response = new CardPlayResponse();

            apiCardPlay api = new apiCardPlay();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (CardPlayResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (CardPlayResponse)bad.Value;
            return response;
        }

    }
}
