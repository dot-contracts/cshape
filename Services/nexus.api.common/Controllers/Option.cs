
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
    public class apiOption : ControllerBase
    {
        [HttpPost("Option")]
        [Authorize]
        public IActionResult Post([FromBody] OptionRequest request)
        {
            OptionResponse enumRes = new OptionResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {
                    var option = request.Options[0];

                    string Params  = "I~S~ActionType~"  + request.ActionType;
                           Params += ";I~S~Vendor~"     + request.Vendor;
                           Params += ";I~S~Location~"   + request.Location;

                    if (!string.IsNullOrEmpty(option.Value))           Params += ";I~S~Value~"           + option.Value;
                    if (option.ValueId       > 0)                      Params += ";I~S~ValueId~"         + option.ValueId;
                    if (option.ParentId      > 0)                      Params += ";I~S~ParentId~"        + option.ParentId;
                    if (option.OptionId      > 0)                      Params += ";I~S~OptionId~"        + option.OptionId;
                    if (!string.IsNullOrEmpty(option.ParentCode))      Params += ";I~S~ParentCode~"      + option.ParentCode;
                    if (!string.IsNullOrEmpty(option.OptionCode))      Params += ";I~S~OptionCode~"      + option.OptionCode;
                    if (!string.IsNullOrEmpty(option.VarDefault))      Params += ";I~S~VarDefault~"      + option.VarDefault;
                    if (!string.IsNullOrEmpty(option.ValueText))       Params += ";I~S~ValueText~"       + option.ValueText;
                    if (!string.IsNullOrEmpty(option.ValueType))       Params += ";I~S~ValueType~"       + option.ValueType;
                    if (option.ValueTypeId   > 0)                      Params += ";I~S~ValueTypeId~"     + option.ValueTypeId;

                    if (option.Computer)                               Params += ";I~S~Computer~1";
                    if (option.ComputerId    > 0)                      Params += ";I~S~ComputerId~"      + option.ComputerId;
                    if (option.ComputerRole)                           Params += ";I~S~ComputerRole~1";
                    if (option.ComputerRoleId > 0)                     Params += ";I~S~ComputerRoleId~"  + option.ComputerRoleId;

                    if (option.Worker)                                 Params += ";I~S~Worker~1";
                    if (option.WorkerId      > 0)                      Params += ";I~S~WorkerId~"        + option.WorkerId;
                    if (option.WorkerRole)                             Params += ";I~S~WorkerRole~1";
                    if (option.WorkerRoleId  > 0)                      Params += ";I~S~WorkerRoleId~"    + option.WorkerRoleId;

                    helpers.WriteToLog("apiOption:" + Params);
                    DataTable dt = db.GetDataTable("cmn.api_option",Params);
                    enumRes.Response = db.DataError;

                    if (dt.Rows.Count > 0)
                    {
                        enumRes.Status   = "Valid"; // dt.Rows[0]["Status"].ToString();
                        enumRes.Response = "";      // dt.Rows[0]["Response"].ToString();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];

                            nexus.shared.common.Option opt = new nexus.shared.common.Option()
                            {
                                Value           =                row["Value"]?.ToString() ?? string.Empty,
                                ValueId         = helpers.ToInt (row["ValueId"].ToString()),
                                ParentId        = helpers.ToInt (row["ParentId"].ToString()),
                                OptionId        = helpers.ToInt (row["OptionId"].ToString()),
                                ParentCode      =                row["ParentCode"]?.ToString() ?? string.Empty,
                                OptionCode      =                row["OptionCode"]?.ToString() ?? string.Empty,
                                VarDefault      =                row["VarDefault"]?.ToString() ?? string.Empty,
                                ValueText       =                row["ValueText"]?.ToString() ?? string.Empty,
                                ValueType       =                row["ValueType"]?.ToString() ?? string.Empty,
 
                                Computer        = helpers.ToBool(row["Computer"].ToString()),
                                ComputerId      = helpers.ToInt (row["ComputerId"].ToString()),
                                ComputerRole    = helpers.ToBool(row["ComputerRole"].ToString()),
                                ComputerRoleId  = helpers.ToInt (row["ComputerRoleId"].ToString()),

                                Worker          = helpers.ToBool(row["Worker"].ToString()),
                                WorkerId        = helpers.ToInt (row["WorkerId"].ToString()),
                                WorkerRole      = helpers.ToBool(row["WorkerRole"].ToString()),
                                WorkerRoleId    = helpers.ToInt (row["WorkerRoleId"].ToString()),

                                ValueTypeId     = helpers.ToInt (row["ValueTypeId"].ToString())
                            };
                            enumRes.Options.Add(opt);
                        }

                        if (enumRes.Status.Equals("Valid")) return Ok(enumRes);
                    }
                    else if (string.IsNullOrEmpty(db.DataError)) enumRes.Response = "no data";
                }
            }

            return BadRequest(enumRes);
        }


    }
}
