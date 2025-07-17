
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
using nexus.shared.application;

namespace nexus.api.application
{
    [Route("app")]
    public class apiRole : ControllerBase
    {
        [HttpPost("Role")]
        [Authorize]
        public IActionResult Post([FromBody] RoleRequest request)
        {
            RoleResponse cohRes = new RoleResponse()
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

                        if (request.Role != null)
                        {
                            Role Role = request.Role;

                            if (Role.HasData())
                            {
                                diag = "1";
                                if (Role.RoleId > 0) Params += ";I~S~RoleId~" + Role.RoleId.ToString();

                                if (!string.IsNullOrEmpty(Role.Description))  Params += ";I~S~Description~" + Role.Description;
                                if (!string.IsNullOrEmpty(Role.RoleType))     Params += ";I~S~RoleType~" +    Role.RoleType;
                                if (!string.IsNullOrEmpty(Role.HumanType))    Params += ";I~S~HumanType~" +   Role.HumanType;
                                if (!string.IsNullOrEmpty(Role.DeviceType))   Params += ";I~S~DeviceType~" +  Role.DeviceType;
                                if (!string.IsNullOrEmpty(Role.Module))       Params += ";I~S~Module~" +      Role.Module;
                                if (!string.IsNullOrEmpty(Role.Function))     Params += ";I~S~Function~" +    Role.Function;
                                
                                if (Role.RoleTypeId > 0)   Params += ";I~S~RoleTypeId~" +   Role.RoleTypeId.ToString();
                                if (Role.HumanTypeId > 0)  Params += ";I~S~HumanTypeId~" +  Role.HumanTypeId.ToString();
                                if (Role.DeviceTypeId > 0) Params += ";I~S~DeviceTypeId~" + Role.DeviceTypeId.ToString();
                                if (Role.ModuleId > 0)     Params += ";I~S~ModuleId~" +     Role.ModuleId.ToString();
                                if (Role.FunctionId > 0)   Params += ";I~S~FunctionId~" +   Role.FunctionId.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiRole:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_Role", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                Role[] Roles = new Role[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    Role = new Role()
                                    {
                                        RoleId        = helpers.ToInt     (dt.Rows[i]["RoleId"].ToString()),

                                        Description   =                    dt.Rows[i]["Role"].ToString(),
                                        RoleType      =                    dt.Rows[i]["RoleType"].ToString(),
                                        HumanType     =                    dt.Rows[i]["HumanType"].ToString(),
                                        DeviceType    =                    dt.Rows[i]["DeviceType"].ToString(),
                                        Module        =                    dt.Rows[i]["Module"].ToString(),
                                        Function      =                    dt.Rows[i]["Function"].ToString(),

                                        RoleTypeId    = helpers.ToInt     (dt.Rows[i]["RoleTypeId"].ToString()),
                                        HumanTypeId   = helpers.ToInt     (dt.Rows[i]["HumanTypeId"].ToString()),
                                        DeviceTypeId  = helpers.ToInt     (dt.Rows[i]["DeviceTypeId"].ToString()),
                                        ModuleId      = helpers.ToInt     (dt.Rows[i]["ModuleId"].ToString()),
                                        FunctionId    = helpers.ToInt     (dt.Rows[i]["FunctionId"].ToString()),
                                    };
                                    Roles[i] = Role;

                                }
                                cohRes.Roles = Roles;

                                if (cohRes.Status.Equals("Valid")) return Ok(cohRes);
                            }
                            else
                            {
                                cohRes.Status   = "Error";
                                cohRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Role Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public RoleResponse getRoles(int ActionType, string Vendor, string Location, Role[] Roles)
        {
            return getRole(ActionType, Vendor, Location, Roles[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public RoleResponse getRole(int ActionType, string Vendor, string Location, Role Role)
        {
            RoleRequest request = new RoleRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Role     = Role
            };

            RoleResponse response = new RoleResponse();

            apiRole api = new apiRole();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (RoleResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (RoleResponse)bad.Value;
            return response;
        }

    }
}
