
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
    public class apiMemberClubDelivery : ControllerBase
    {
        [HttpPost("MemberClubDelivery")]
        [Authorize]
        public IActionResult Post([FromBody] MemberClubDeliveryRequest request)
        {
            MemberClubDeliveryResponse MemberClubDeliveryRes = new MemberClubDeliveryResponse()
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
                        if (request.MemberClubDelivery != null)
                        {
                            MemberClubDelivery mc = request.MemberClubDelivery;

                            if (mc.HasData())
                            {
                                diag = "2";
                                if (mc.MemberClubDeliveryId > 0)             Params += ";I~S~MemberClubDeliveryId~" + mc.MemberClubDeliveryId.ToString();
                                if (!string.IsNullOrEmpty(mc.DeliveryType))  Params += ";I~S~DeliveryType~" +     mc.DeliveryType;

                                diag = "3";
                                if (mc.MemberId > 0)         Params += ";I~S~MemberId~" +         mc.MemberId.ToString();
                                if (mc.ClubId > 0)           Params += ";I~S~ClubId~" +           mc.ClubId.ToString();
                                if (mc.EDM > 0)              Params += ";I~S~EDM~" +              mc.EDM.ToString();
                                if (mc.SMS > 0)              Params += ";I~S~SMS~" +              mc.SMS.ToString();
                                if (mc.POST> 0)              Params += ";I~S~POST~" +             mc.POST.ToString();
                                if (mc.DeliveryTypeId > 0)   Params += ";I~S~DeliveryTypeId~" +   mc.DeliveryTypeId.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiMemberClubDelivery:" + Params);
                            DataTable dt = db.GetDataTable("mem.api_MemberClubDelivery", Params);
                            if (dt.Rows.Count > 0)
                            {
                                MemberClubDeliveryRes.Status   = dt.Rows[0]["Status"].ToString();
                                MemberClubDeliveryRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "5";
                                MemberClubDelivery[] MemberClubDeliverys = new MemberClubDelivery[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "6" + i.ToString();
                                    MemberClubDelivery MemberClubDelivery = new MemberClubDelivery()
                                    {
                                        MemberClubDeliveryId = helpers.ToInt(dt.Rows[i]["MemberClubDeliveryId"].ToString()),
                                        DeliveryType         =               dt.Rows[i]["DeliveryType"].ToString(),
                                        MemberId         = helpers.ToInt   ( dt.Rows[i]["MemberId"].ToString()),
                                        ClubId           = helpers.ToInt   ( dt.Rows[i]["ClubId"].ToString()),
                                        EDM              = helpers.ToInt   ( dt.Rows[i]["EDM"].ToString()),
                                        SMS              = helpers.ToInt   ( dt.Rows[i]["SMS"].ToString()),
                                        POST             = helpers.ToInt   ( dt.Rows[i]["POST"].ToString()),
                                        DeliveryTypeId   = helpers.ToInt   ( dt.Rows[i]["DeliveryTypeId"].ToString()),
                                    };
                                    MemberClubDeliverys[i] = MemberClubDelivery;
                                }

                                MemberClubDeliveryRes.MemberClubDeliveries = MemberClubDeliverys;

                                if (MemberClubDeliveryRes.Status.Equals("Valid")) return Ok(MemberClubDeliveryRes);
                            }
                            else
                            {
                                MemberClubDeliveryRes.Status   = "Error";
                                MemberClubDeliveryRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("MemClubDel Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(MemberClubDeliveryRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberClubDeliveryResponse getMemberClubDeliverys(int ActionType, string Vendor, string Location, MemberClubDelivery[] MemberClubDeliverys)
        {
            return  getMemberClubDelivery(ActionType, Vendor, Location, MemberClubDeliverys[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberClubDeliveryResponse getMemberClubDelivery(int ActionType, string Vendor, string Location, MemberClubDelivery MemberClubDelivery)
        {
            MemberClubDeliveryRequest request  = new MemberClubDeliveryRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                MemberClubDelivery = MemberClubDelivery
            };

            MemberClubDeliveryResponse response = new MemberClubDeliveryResponse();

            apiMemberClubDelivery apiMemberClubDelivery = new apiMemberClubDelivery();
            IActionResult action = apiMemberClubDelivery.Post(request);
            if (action is OkObjectResult ok) response = (MemberClubDeliveryResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (MemberClubDeliveryResponse)bad.Value;
            return response;
        }

    }
}
