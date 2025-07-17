
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using nexus.web.auth.Cache;
using nexus.web.auth.Data;
using nexus.web.auth.Model;
using nexus.common;
using nexus.common.dal;

using nexus.api.Model.Local;
using nexus.api.Model.Membership;

namespace nexus.api.Controller.Membership
{
    [Route("mem")]
    public class apiFee : ControllerBase
    {
        [HttpPost("ClubFee")]
        [Authorize]
        public IActionResult Post([FromBody] FeeRequest request)
        {
            FeeResponse feeRes = new FeeResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
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
                        if (request.Fee != null)
                        {
                            Fee fee = request.Fee;
                            if (fee.HasData())
                            {
                                if (!string.IsNullOrEmpty(fee.Description))   Params += ";I~S~Description~" +  fee.Description;
                                if (!string.IsNullOrEmpty(fee.FeeType))       Params += ";I~S~FeeType~" +      fee.FeeType;
                                if (!string.IsNullOrEmpty(fee.FeeCode))       Params += ";I~S~FeeCode~" +      fee.FeeCode;
                                if (!string.IsNullOrEmpty(fee.MemberType))    Params += ";I~S~MemberType~" +   fee.MemberType;
                                if (!string.IsNullOrEmpty(fee.DurationType))  Params += ";I~S~DurationType~" + fee.DurationType;
                                if (!string.IsNullOrEmpty(fee.SPName))        Params += ";I~S~SPName~"       + fee.SPName;
                                if (!string.IsNullOrEmpty(fee.Property))      Params += ";I~S~Property~" +     fee.Property;
                                diag = "2";

                                if (fee.FeeId > 0)        Params += ";I~S~FeeId~" +     fee.FeeId.ToString();
                                if (fee.ClubId > 0)       Params += ";I~S~ClubId~" +    fee.ClubId.ToString();
                                if (fee.Value > 0)        Params += ";I~S~Value~" +     fee.Value.ToString();
                                if (fee.MinValue > 0)     Params += ";I~S~MinValue~" +  fee.MinValue.ToString();
                                if (fee.GSTInc > 0)       Params += ";I~S~GSTInc~" +    fee.GSTInc.ToString();
                                if (fee.Duration> 0)      Params += ";I~S~Duration~" +  fee.Duration.ToString();
                            }

                            diag = "3";
                            helpers.WriteToLog("apiFee:" + Params);
                            DataTable dt = db.GetDataTable("mem.api_ClubFee", Params);
                            if (dt.Rows.Count > 0)
                            {
                                feeRes.Status   = dt.Rows[0]["Status"].ToString();
                                feeRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "4";
                                Fee[] Fees = new Fee[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    fee = new Fee()
                                    {
                                        FeeId        = helpers.ToInt     (dt.Rows[i]["FeeId"].ToString()),
                                        Description  =                    dt.Rows[i]["Description"].ToString(),
                                        FeeType      =                    dt.Rows[i]["FeeType"].ToString(),
                                        FeeCode      =                    dt.Rows[i]["FeeCode"].ToString(),
                                        MemberType   =                    dt.Rows[i]["MemberType"].ToString(),
                                        Club         =                    dt.Rows[i]["Club"].ToString(),
                                        ClubId       = helpers.ToInt     (dt.Rows[i]["ClubId"].ToString()),
                                        Value        = helpers.ToDecimal (dt.Rows[i]["Value"].ToString()),
                                        MinValue     = helpers.ToDecimal (dt.Rows[i]["MinValue"].ToString()),
                                        GSTInc       = helpers.ToInt     (dt.Rows[i]["GSTInc"].ToString()),
                                        DurationType =                    dt.Rows[i]["DurationType"].ToString(),
                                        Duration     = helpers.ToInt     (dt.Rows[i]["Duration"].ToString()),
                                        SPName       =                    dt.Rows[i]["SPName"].ToString(),
                                        Property     =                    dt.Rows[i]["Property"].ToString()
                                    };
                                    Fees[i] = fee;
                                }

                                feeRes.Fees = Fees;

                                if (feeRes.Status.Equals("Valid")) return Ok(feeRes);
                            }
                            else
                            {
                                feeRes.Status   = "Error";
                                feeRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Fee Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(feeRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeeResponse getFees(int ActionType, string Vendor, string Location, Fee[] Fees)
        {
            return getFee(ActionType, Vendor, Location, Fees[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeeResponse getFee(int ActionType, string Vendor, string Location, Fee Fee)
        {
            FeeRequest request = new FeeRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Fee        = Fee
            };

            FeeResponse response = new FeeResponse();

            apiFee api = new apiFee();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (FeeResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (FeeResponse)bad.Value;
            return response;
        }

    }
}
