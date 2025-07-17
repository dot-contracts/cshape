
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.web.auth.Data;
using nexus.web.auth.Model;
using nexus.common;
using nexus.common.dal;
using nexus.api.Model.Membership;

namespace nexus.api.Controller.Membership
{
    [Route("mem")]
    //[ApiController, Authorize]
    public class apiPIN : ControllerBase
    {
        [HttpPost("PIN")]
        [Authorize]
        public IActionResult Post([FromBody] PINRequest request)
        {

            PINResponse pinRes = new PINResponse()
            {
                Status   = "Error",
                Response = "Request Poorly formatted"
            };

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            if (request != null)
            {
                string diag = "";
                try
                {
                    setting.GetSettings();
                    string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
                    using (SQLServer db = new SQLServer(conStr))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                        diag = "1";
                        if (request.MemberId>0)                           Params += ";I~S~MemberId~"  + request.MemberId.ToString();
                        if (!string.IsNullOrEmpty(request.BadgeNo))       Params += ";I~S~BadgeNo~"   + request.BadgeNo;
                        if (!string.IsNullOrEmpty(request.CardNo ))       Params += ";I~S~CardNo~"    + request.CardNo;
                        if (!string.IsNullOrEmpty(request.PinNo ))        Params += ";I~S~PinNo~"     + request.PinNo;
                        if (!string.IsNullOrEmpty(request.NewPinNo))      Params += ";I~S~NewPinNo~"  + request.NewPinNo;

                        diag = "2";
                        helpers.WriteToLog("apiPIN:" + Params);
                        DataTable dt = db.GetDataTable("mem.api_PIN", Params);
                        if (dt.Rows.Count > 0)
                        {
                            diag = "3";
                            pinRes.Status   = dt.Rows[0]["Status"].ToString();
                            pinRes.Response = dt.Rows[0]["Response"].ToString();

                            if (pinRes.Status.Equals("Valid")) return Ok(pinRes);
                        }
                        else
                        {
                            pinRes.Status   = "Error";
                            pinRes.Response = "No Data Found, " + db.DataError;
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("PIN Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(pinRes);
        }


    }
}
