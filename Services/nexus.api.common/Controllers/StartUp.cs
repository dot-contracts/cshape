
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using nexus.common;
using nexus.common.dal;
using nexus.shared.common;

namespace nexus.api.common
{
    [Route("cmn")]
    //[ApiController, Authorize]
    public class apiStartUp : ControllerBase
    {
        [HttpPost("StartUp")]
        [Authorize]
        public IActionResult Post([FromBody] StartUpRequest request)
        {
            StartUpResponse enumRes = new StartUpResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {
                    var StartUp = request.StartUp;

                    string Params  = "I~S~ActionType~"  + request.ActionType;
                           Params += ";I~S~Vendor~"     + request.Vendor;
                           Params += ";I~S~Location~"   + request.Location;

                    if (!string.IsNullOrEmpty(StartUp.Value))           Params += ";I~S~Value~"           + StartUp.Value;
                    if (StartUp.ValueId       > 0)                      Params += ";I~S~ValueId~"         + StartUp.ValueId;
                    if (StartUp.ParentId      > 0)                      Params += ";I~S~ParentId~"        + StartUp.ParentId;
                    if (StartUp.StartUpId      > 0)                      Params += ";I~S~StartUpId~"        + StartUp.StartUpId;
                    if (!string.IsNullOrEmpty(StartUp.ParentCode))      Params += ";I~S~ParentCode~"      + StartUp.ParentCode;
                    if (!string.IsNullOrEmpty(StartUp.StartUpCode))      Params += ";I~S~StartUpCode~"      + StartUp.StartUpCode;
                    if (!string.IsNullOrEmpty(StartUp.VarDefault))      Params += ";I~S~VarDefault~"      + StartUp.VarDefault;
                    if (!string.IsNullOrEmpty(StartUp.ValueText))       Params += ";I~S~ValueText~"       + StartUp.ValueText;
                    if (!string.IsNullOrEmpty(StartUp.ValueType))       Params += ";I~S~ValueType~"       + StartUp.ValueType;
                    if (StartUp.ValueTypeId   > 0)                      Params += ";I~S~ValueTypeId~"     + StartUp.ValueTypeId;

                    if (StartUp.Computer)                               Params += ";I~S~Computer~1";
                    if (StartUp.ComputerId    > 0)                      Params += ";I~S~ComputerId~"      + StartUp.ComputerId;
                    if (StartUp.ComputerRole)                           Params += ";I~S~ComputerRole~1";
                    if (StartUp.ComputerRoleId > 0)                     Params += ";I~S~ComputerRoleId~"  + StartUp.ComputerRoleId;

                    if (StartUp.Worker)                                 Params += ";I~S~Worker~1";
                    if (StartUp.WorkerId      > 0)                      Params += ";I~S~WorkerId~"        + StartUp.WorkerId;
                    if (StartUp.WorkerRole)                             Params += ";I~S~WorkerRole~1";
                    if (StartUp.WorkerRoleId  > 0)                      Params += ";I~S~WorkerRoleId~"    + StartUp.WorkerRoleId;

                    helpers.WriteToLog("apiStartUp:" + Params);
                    //DataTable dt = db.GetDataTable("cmn.api_EnumValue", Params);
                    DataTable dt = db.GetDataTable("cmn.api_StartUp",Params);
                    if (dt.Rows.Count > 0)
                    {
                        enumRes.Status   = "Valid"; // dt.Rows[0]["Status"].ToString();
                        enumRes.Response = "";      // dt.Rows[0]["Response"].ToString();

                        if (dt.Rows.Count > 0)
                        {
                            var row = dt.Rows[0];
                            
                            StartUp code = new StartUp()
                            {
                                Value           = row["Value"]?.ToString() ?? string.Empty,
                                ValueId         = helpers.ToInt(row["ValueId"].ToString()),
                                ParentId        = helpers.ToInt(row["ParentId"].ToString()),
                                StartUpId       = helpers.ToInt(row["StartUpId"].ToString()),
                                ParentCode      = row["ParentCode"]?.ToString() ?? string.Empty,
                                StartUpCode     = row["StartUpCode"]?.ToString() ?? string.Empty,
                                VarDefault      = row["VarDefault"]?.ToString() ?? string.Empty,
                                ValueText       = row["ValueText"]?.ToString() ?? string.Empty,
                                ValueType       = row["ValueType"]?.ToString() ?? string.Empty,

                                Computer        = helpers.ToBool(row["Computer"].ToString()),
                                ComputerId      = helpers.ToInt(row["ComputerId"].ToString()),
                                ComputerRole    = helpers.ToBool(row["ComputerRole"].ToString()),
                                ComputerRoleId  = helpers.ToInt(row["ComputerRoleId"].ToString()),

                                Worker          = helpers.ToBool(row["Worker"].ToString()),
                                WorkerId        = helpers.ToInt(row["WorkerId"].ToString()),
                                WorkerRole      = helpers.ToBool(row["WorkerRole"].ToString()),
                                WorkerRoleId    = helpers.ToInt(row["WorkerRoleId"].ToString()),

                                ValueTypeId     = helpers.ToInt(row["ValueTypeId"].ToString())
                            };

                            enumRes.StartUp = code;
                        }
                        if (enumRes.Status.Equals("Valid")) return Ok(enumRes);
                    }
                }
            }

            return BadRequest(enumRes);
        }


    }
}
