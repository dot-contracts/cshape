
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
    public class apiLookUp : ControllerBase
    {
        [HttpPost("LookUp")]
        [Authorize]
        public IActionResult Post([FromBody] LookUpRequest request)
        {
            LookUpResponse response = new LookUpResponse()
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
                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;


                            if (!string.IsNullOrEmpty(request.Search))    Params += ";I~S~Search~" +  request.Search;
                            if (!string.IsNullOrEmpty(request.Value))     Params += ";I~S~Value~"  +  request.Value;

                            helpers.WriteToLog("apiLookUp:" + Params);
                            DataTable dt = db.GetDataTable("xta.gam_LookUp", Params);
                            if (dt.Rows.Count > 0)
                            {
                                response.Status   = "Valid";
                                response.Response = "";

                                LookUp[] LookUps = new LookUp[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    LookUp LookUp = new LookUp()
                                    {
                                        Description =  dt.Rows[i]["Description"].ToString(),
                                        Id          =  dt.Rows[i]["Id"].ToString(),
                                    };
                                    LookUps[i] = LookUp;
                                }
                                response.Results = LookUps;

                                if (response.Status.Equals("Valid")) return Ok(response);
                            }
                            else
                            {
                                response.Status   = "Error";
                                response.Response = "No Data Found, " + db.DataError;
                            }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("LookUp Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public LookUpResponse getLookUp(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            LookUpRequest request = new LookUpRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            LookUpResponse response = new LookUpResponse();

            apiLookUp api = new apiLookUp();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (LookUpResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (LookUpResponse)bad.Value;
            return response;
        }

    }
}
