
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
    public class apiUser : ControllerBase
    {
        [HttpPost("User")]
        [Authorize]
        public IActionResult Post([FromBody] UserRequest request)
        {
            UserResponse cohRes = new UserResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                string diag = "";
                try
                {
                    string conStr = setting.ConnectionString;
                    using (SQLServer db = new SQLServer(conStr))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                        if (request.User != null)
                        {
                            User User = request.User;

                            if (User.HasData())
                            {
                                diag = "1";
                                if (User.UserId > 0) Params += ";I~S~UserId~" + User.UserId.ToString();

                                if (!string.IsNullOrEmpty(User.Description))  Params += ";I~S~Description~" + User.Description;
                                if (!string.IsNullOrEmpty(User.UserType))     Params += ";I~S~UserType~" +    User.UserType;
                                if (!string.IsNullOrEmpty(User.HumanType))    Params += ";I~S~HumanType~" +   User.HumanType;
                                if (!string.IsNullOrEmpty(User.DeviceType))   Params += ";I~S~DeviceType~" +  User.DeviceType;
                                if (!string.IsNullOrEmpty(User.Module))       Params += ";I~S~Module~" +      User.Module;
                                if (!string.IsNullOrEmpty(User.Function))     Params += ";I~S~Function~" +    User.Function;
                                
                                if (User.UserTypeId > 0)   Params += ";I~S~UserTypeId~" +   User.UserTypeId.ToString();
                                if (User.HumanTypeId > 0)  Params += ";I~S~HumanTypeId~" +  User.HumanTypeId.ToString();
                                if (User.DeviceTypeId > 0) Params += ";I~S~DeviceTypeId~" + User.DeviceTypeId.ToString();
                                if (User.ModuleId > 0)     Params += ";I~S~ModuleId~" +     User.ModuleId.ToString();
                                if (User.FunctionId > 0)   Params += ";I~S~FunctionId~" +   User.FunctionId.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiUser:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_User", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                User[] Users = new User[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    User = new User()
                                    {
                                        UserId        = helpers.ToInt     (dt.Rows[i]["UserId"].ToString()),

                                        Description   =                    dt.Rows[i]["User"].ToString(),
                                        UserType      =                    dt.Rows[i]["UserType"].ToString(),
                                        HumanType     =                    dt.Rows[i]["HumanType"].ToString(),
                                        DeviceType    =                    dt.Rows[i]["DeviceType"].ToString(),
                                        Module        =                    dt.Rows[i]["Module"].ToString(),
                                        Function      =                    dt.Rows[i]["Function"].ToString(),

                                        UserTypeId    = helpers.ToInt     (dt.Rows[i]["UserTypeId"].ToString()),
                                        HumanTypeId   = helpers.ToInt     (dt.Rows[i]["HumanTypeId"].ToString()),
                                        DeviceTypeId  = helpers.ToInt     (dt.Rows[i]["DeviceTypeId"].ToString()),
                                        ModuleId      = helpers.ToInt     (dt.Rows[i]["ModuleId"].ToString()),
                                        FunctionId    = helpers.ToInt     (dt.Rows[i]["FunctionId"].ToString()),
                                    };
                                    Users[i] = User;

                                }
                                cohRes.Users = Users;

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
                    helpers.WriteToLog("User Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public UserResponse getUsers(int ActionType, string Vendor, string Location, User[] Users)
        {
            return getUser(ActionType, Vendor, Location, Users[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public UserResponse getUser(int ActionType, string Vendor, string Location, User User)
        {
            UserRequest request = new UserRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                User     = User
            };

            UserResponse response = new UserResponse();

            apiUser api = new apiUser();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (UserResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (UserResponse)bad.Value;
            return response;
        }

    }
}
