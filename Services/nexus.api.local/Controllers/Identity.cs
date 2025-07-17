
using System.Data;
using System.Diagnostics.Contracts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiIdentity : ControllerBase
    {
        [HttpPost("Identity")]
        [Authorize]
        public IActionResult Post([FromBody] IdentityRequest request)
        {
            IdentityResponse idtRes = new IdentityResponse()
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

                        string Params = "I~S~ActionType~" + request.ActionType;
                        Params       += ";I~S~Vendor~" +    request.Vendor;
                        Params       += ";I~S~Location~" +  request.Location;

                        if (request.Identity != null)
                        {
                            Identity identity = request.Identity;

                            diag = "1";

                            if (identity.HasData())
                            {
                                if (identity.IdentityId > 0)                          Params += ";I~S~IdentityId~" +         identity.IdentityId.ToString();
                                if (identity.EntityId > 0)                            Params += ";I~S~EntityId~" +           identity.EntityId.ToString();
                                if (!string.IsNullOrEmpty(identity.IdentityType))     Params += ";I~S~IdentityType~" +       identity.IdentityType;
                                if (!string.IsNullOrEmpty(identity.IdentityState))    Params += ";I~S~IdentityState~" +      identity.IdentityState;
                                if (!string.IsNullOrEmpty(identity.IdentityLocation)) Params += ";I~S~IdentityLocation~" +   identity.IdentityLocation;
                                if (!string.IsNullOrEmpty(identity.Value))            Params += ";I~S~Value~" +              identity.Value;
                                if (!string.IsNullOrEmpty(identity.Property))         Params += ";I~S~Property~" +           identity.Property;

                                diag = "2";
                                if (helpers.IsDate(identity.Expires))                 Params += ";I~S~Expires~" +            identity.Expires.ToString();

                                diag = "3";
                                if (identity.IdentityTypeId>0)                        Params += ";I~S~IdentityTypeId~" +     identity.IdentityTypeId.ToString();
                                if (identity.IdentityStateId>0)                       Params += ";I~S~IdentityStateId~" +    identity.IdentityStateId.ToString();
                                if (identity.IdentityLocationId>0)                    Params += ";I~S~IdentityLocationId~" + identity.IdentityLocationId.ToString();
                                if (identity.ImageId>0)                               Params += ";I~S~ImageId~" +            identity.ImageId.ToString();
                                if (identity.NoteId>0)                                Params += ";I~S~NoteId~" +             identity.NoteId.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiIdentity:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_Identity", Params);
                            if (dt.Rows.Count > 0)
                            {
                                idtRes.Status   = dt.Rows[0]["Status"].ToString();
                                idtRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "5";
                                Identity[] Identitys = new Identity[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "6" + i.ToString();
                                    identity = new Identity()
                                    {
                                        IdentityId         = helpers.ToInt  (dt.Rows[i]["IdentityId"].ToString()),
                                        EntityId           = helpers.ToInt  (dt.Rows[i]["EntityId"].ToString()),
                                        IdentityType       =                 dt.Rows[i]["IdentityType"].ToString(),
                                        IdentityState      =                 dt.Rows[i]["IdentityState"].ToString(),
                                        IdentityLocation   =                 dt.Rows[i]["IdentityLocation"].ToString(),
                                        Value              =                 dt.Rows[i]["Value"].ToString(),
                                        Property           =                 dt.Rows[i]["Property"].ToString(),
                                        Expires            =                 dt.Rows[i]["Expires"].ToString(),
                                        IdentityTypeId     = helpers.ToInt  (dt.Rows[i]["IdentityType"].ToString()),
                                        IdentityStateId    = helpers.ToInt  (dt.Rows[i]["IdentityState"].ToString()),
                                        IdentityLocationId = helpers.ToInt  (dt.Rows[i]["IdentityLocation"].ToString()),
                                        ImageId            = helpers.ToInt  (dt.Rows[i]["ImageId"].ToString()),
                                        NoteId             = helpers.ToInt  (dt.Rows[i]["NoteId"].ToString())
                                    };
                                    Identitys[i] = identity;
                                }
                                idtRes.Identities = Identitys;

                                if (idtRes.Status.Equals("Valid")) return Ok(idtRes);
                            }
                            else
                            {
                                idtRes.Status = "Error";
                                idtRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Identity Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(idtRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IdentityResponse getIdentities(int ActionType, string Vendor, string Location, Identity[] Identities)
        {
            return getIdentity(ActionType, Vendor, Location, Identities[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IdentityResponse getIdentity(int ActionType, string Vendor, string Location, Identity Identity)
        {
            IdentityRequest  request  = new IdentityRequest(ActionType, Vendor, Location, Identity);
            IdentityResponse response = new IdentityResponse();

            apiIdentity api = new apiIdentity();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (IdentityResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (IdentityResponse)bad.Value;
            return response;
        }

    }
}
