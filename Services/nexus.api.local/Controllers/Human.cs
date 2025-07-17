
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiHuman : ControllerBase
    {
        [HttpPost("Human")]
        [Authorize]
        public IActionResult Post([FromBody] HumanRequest request)
        {

            HumanResponse humRes = new HumanResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

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

                        if (!string.IsNullOrEmpty(request.BadgeNo)) Params += ";I~S~BadgeNo~" + request.BadgeNo;
                        if (!string.IsNullOrEmpty(request.CardNo))  Params += ";I~S~CardNo~" + request.CardNo;

                        if (request.Human != null)
                        {
                            Human human = request.Human;

                            if (human.HasData())
                            {
                                diag = "1";
                                if (human.HumanId > 0)                          Params += ";I~S~HumanId~" +      human.HumanId.ToString();
                                Params += HumanHelpers.GetParams(human);
                            }

                            diag = "2";
                            helpers.WriteToLog("apiHuman:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_Human", Params);
                            if (dt.Rows.Count > 0)
                            {
                                humRes.Status   = dt.Rows[0]["Status"].ToString();
                                humRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "3";
                                Human[] Humans = new Human[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "4" + i.ToString();

                                    human = new Human();
                                    human = HumanHelpers.LoadFromRow(dt.Rows[i]);
                                    human.HumanId = helpers.ToInt(dt.Rows[i]["HumanPk"].ToString());
                                    Humans[i] = human;
                                }
                                humRes.Humans = Humans;

                                if (humRes.Status.Equals("Valid")) return Ok(humRes);
                            }
                            else
                            {
                                humRes.Status   = "Error";
                                humRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Human Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(humRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public HumanResponse getHumans(int ActionType, string Vendor, string Location, Human[] Humans)
        {
            return getHuman(ActionType, Vendor, Location, Humans[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public HumanResponse getHuman(int ActionType, string Vendor, string Location, Human Human)
        {
            HumanRequest request = new HumanRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Human      = Human
            };

            HumanResponse response = new HumanResponse();

            apiHuman api = new apiHuman();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (HumanResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (HumanResponse)bad.Value;
            return response;
        }

    }
}
