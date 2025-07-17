
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

namespace nexus.api.gaming
{
    [Route("gam")]
    public class apiLiability : ControllerBase
    {
        [HttpPost("Liability")]
        [Authorize]
        public IActionResult Post([FromBody] LiabilityRequest request)
        {
            LiabilityResponse response = new LiabilityResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                if (helpers.IsDate(request.OpenDate) && helpers.IsDate(request.CloseDate))
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {
                        DataTable dt = db.GetDataTable("SELECT CloseDate, Value3 FROM ahis.vue_Batch WHERE isnull(Value3, 0) > 0 and(BatchType = 'day type') and CloseDate>='" + request.OpenDate + "' and CloseDate<='" + request.CloseDate + "'");
                        if (dt.Rows.Count > 0)
                        {
                            response.Status    = "Valid";
                            response.Response  = "";
                            response.CloseDate = request.CloseDate;

                            response.Opening  = helpers.ToDecimal(dt.Rows[0]["Value3"].ToString());
                            response.Closing  = helpers.ToDecimal(dt.Rows[1]["Value3"].ToString());
                            response.Movement = response.Closing - response.Opening;

                            return Ok(response);
                        }
                        else response.Response = "No Data Found, " + db.DataError;
                    }
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public LiabilityResponse getLiability(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            LiabilityRequest request = new LiabilityRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            LiabilityResponse response = new LiabilityResponse();

            apiLiability api = new apiLiability();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (LiabilityResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (LiabilityResponse)bad.Value;
            return response;
        }

    }
}
