
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
    public class apiEnumCode : ControllerBase
    {
        [HttpPost("EnumCode")]
        [Authorize]
        public IActionResult Post([FromBody] EnumCodeRequest request)
        {
            EnumCodeResponse enumRes = new EnumCodeResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {

                    string Params  = "I~S~ActionType~" + request.ActionType;
                           Params += ";I~S~Vendor~" +    request.Vendor;
                           Params += ";I~S~Location~" +  request.Location;
                           Params += ";I~S~ValueType~" + request.EnumCode.ValueType.ToString();

                    if (request.EnumCode.EnumPk > 0)     Params += ";I~S~EnumId~" +    request.EnumCode.EnumPk.ToString();
                    if (request.EnumCode.ValuePk > 0)    Params += ";I~S~ValueId~" +   request.EnumCode.ValuePk.ToString();

                    if (!string.IsNullOrEmpty(request.EnumCode.EnumCode))  Params += ";I~S~EnumCode~" +  request.EnumCode.EnumCode;
                    if (!string.IsNullOrEmpty(request.EnumCode.ValueCode)) Params += ";I~S~ValueCode~" + request.EnumCode.ValueCode;

                    helpers.WriteToLog("apiEnumCode:" + Params);
                    //DataTable dt = db.GetDataTable("cmn.api_EnumValue", Params);
                    DataTable dt = db.GetDataTable("cmn.lst_EnumValue", "");
                    if (dt.Rows.Count > 0)
                    {
                        enumRes.Status   = "Valid"; // dt.Rows[0]["Status"].ToString();
                        enumRes.Response = "";      // dt.Rows[0]["Response"].ToString();

                        Enumcode[] Codes = new Enumcode[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Enumcode code = new Enumcode()
                            {
                                EnumPk    = helpers.ToInt (dt.Rows[i]["EnumPk"].ToString()),
                                ValuePk   = helpers.ToInt (dt.Rows[i]["ValuePk"].ToString()),
                                ValueType = helpers.ToInt (dt.Rows[i]["ValueType"].ToString()),
                                EnumCode  =                dt.Rows[i]["EnumCode"].ToString(),
                                EnumPath  =                dt.Rows[i]["EnumPath"].ToString(),
                                EnumDesc  =                dt.Rows[i]["EnumDesc"].ToString(),
                                ValueCode =                dt.Rows[i]["ValueCode"].ToString(),
                                ValueDesc =                dt.Rows[i]["ValueDesc"].ToString(),
                                Description =              dt.Rows[i]["Description"].ToString(),
                                EnumType  =                dt.Rows[i]["EnumType"].ToString(),
                                EnumState =                dt.Rows[i]["EnumState"].ToString(),
                                Value     =                dt.Rows[i]["Value"].ToString(),
                                Group     = helpers.ToInt (dt.Rows[i]["Group"].ToString()),
                                DenoteId  = helpers.ToInt (dt.Rows[i]["DenoteId"].ToString()),
                                ParentId  = helpers.ToInt (dt.Rows[i]["ParentId"].ToString()),
                                Sequence  = helpers.ToInt (dt.Rows[i]["Sequence"].ToString())

                            };
                            Codes[i] = code;

                            enumRes.EnumCodes = Codes;
                        }

                        if (enumRes.Status.Equals("Valid")) return Ok(enumRes);
                    }
                }
            }

            return BadRequest(enumRes);
        }


    }
}
