
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
using Newtonsoft.Json.Linq;

namespace nexus.api.Controller.Membership
{
    [Route("mem")]
    public class apiMemberClub : ControllerBase
    {
        [HttpPost("MemberClub")]
        [Authorize]
        public IActionResult Post([FromBody] MemberClubRequest request)
        {
            MemberClubResponse MemberClubRes = new MemberClubResponse()
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
                        if (request.MemberClub != null)
                        {
                            MemberClub mc = request.MemberClub;

                            if (mc.HasData())
                            {
                                diag = "2";
                                if (mc.MemberClubId > 0) Params += ";I~S~MemberClubId~" + mc.MemberClubId.ToString();
                                else
                                {
                                    if (mc.MemberId > 0) Params += ";I~S~MemberId~" + mc.MemberId.ToString();
                                    if (mc.ClubId > 0)   Params += ";I~S~ClubId~"   + mc.ClubId.ToString();
                                }

                                diag = "3";
                                if (!string.IsNullOrEmpty(mc.MemberType))          Params += ";I~S~MemberType~"   +     mc.MemberType;
                                else if                       (mc.MemberTypeId > 0)     Params += ";I~S~MemberTypeId~" +     mc.MemberTypeId.ToString();

                                if      (!string.IsNullOrEmpty(mc.MemberState))         Params += ";I~S~MemberState~" +      mc.MemberState;
                                else if                       (mc.MemberStateId > 0)    Params += ";I~S~MemberStateId~" +    mc.MemberStateId.ToString();

                                if      (!string.IsNullOrEmpty(mc.FinancialState))      Params += ";I~S~FinancialState~" +   mc.FinancialState;
                                else if                       (mc.FinancialStateId > 0) Params += ";I~S~FinancialStateId~" + mc.FinancialStateId.ToString();


                                diag = "4";
                                if (!string.IsNullOrEmpty(mc.BadgeNo))         Params += ";I~S~BadgeNo~" +        mc.BadgeNo;

                                if (!string.IsNullOrEmpty(mc.FinancialTo))     Params += ";I~S~FinancialTo~" +    helpers.ToDate(mc.FinancialTo).ToString("dd MMM, yyyy");
                                if (!string.IsNullOrEmpty(mc.Joined))          Params += ";I~S~Joined~" +         helpers.ToDate(mc.Joined).ToString("dd MMM, yyyy");
                                if (!string.IsNullOrEmpty(mc.Approved))        Params += ";I~S~Approved~" +       helpers.ToDate(mc.Approved).ToString("dd MMM, yyyy");
                                if (!string.IsNullOrEmpty(mc.LastPayment))     Params += ";I~S~LastPayment~" +    helpers.ToDate(mc.LastPayment).ToString("dd MMM, yyyy");

                                diag = "5";
                                if (mc.FeeId > 0)            Params += ";I~S~FeeId~" +            mc.FeeId.ToString();
                                if (mc.PromotionId > 0)      Params += ";I~S~PromotionId~" +      mc.PromotionId.ToString();
                                if (mc.BatchId  > 0)         Params += ";I~S~BatchId~" +          mc.BatchId.ToString();
                                if (mc.Voting >0)            Params += ";I~S~Voting~" +           mc.Voting.ToString();
                                if (mc.RenewMode > 0)        Params += ";I~S~RenewMode~" +        mc.RenewMode.ToString();
                            }

                            diag = "6";
                            helpers.WriteToLog("apiMemberClub:" + Params);
                            DataTable dt = db.GetDataTable("mem.api_MemberClub", Params);
                            if (dt.Rows.Count > 0)
                            {
                                MemberClubRes.Status   = dt.Rows[0]["Status"].ToString();
                                MemberClubRes.Response = dt.Rows[0]["Response"].ToString();

                                diag = "7";
                                MemberClub[] MemberClubs = new MemberClub[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "8" + i.ToString();
                                    MemberClub MemberClub = new MemberClub()
                                    {
                                        Member           =                  dt.Rows[i]["Member"].ToString(),
                                        Club             =                  dt.Rows[i]["Club"].ToString(),
                                        MemberType       =                  dt.Rows[i]["MemberType"].ToString(),
                                        MemberState      =                  dt.Rows[i]["MemberState"].ToString(),
                                        FinancialState   =                  dt.Rows[i]["FinancialState"].ToString(),
                                        BadgeNo          =                  dt.Rows[i]["BadgeNo"].ToString(),
                                        FinancialTo      = helpers.ToDate  (dt.Rows[i]["FinancialTo"].ToString()).ToString("dd MMM, yyyy"),
                                        Joined           = helpers.ToDate (dt.Rows[i]["Joined"].ToString()).ToString("dd MMM, yyyy"),
                                        Approved         = helpers.ToDate (dt.Rows[i]["Approved"].ToString()).ToString("dd MMM, yyyy"),
                                        LastPayment      = helpers.ToDate (dt.Rows[i]["LastPayment"].ToString()).ToString("dd MMM, yyyy"),
                                        MemberClubId     = helpers.ToInt   (dt.Rows[i]["MemberClubId"].ToString()),
                                        MemberId         = helpers.ToInt   (dt.Rows[i]["MemberId"].ToString()),
                                        ClubId           = helpers.ToInt   (dt.Rows[i]["ClubId"].ToString()),
                                        FeeId            = helpers.ToInt   (dt.Rows[i]["FeeId"].ToString()),
                                        PromotionId      = helpers.ToInt   (dt.Rows[i]["PromotionId"].ToString()),
                                        BatchId          = helpers.ToInt   (dt.Rows[i]["BatchId"].ToString()),
                                        Voting           = helpers.ToInt   (dt.Rows[i]["Voting"].ToString()),
                                        RenewMode        = helpers.ToInt   (dt.Rows[i]["RenewMode"].ToString()),
                                        MemberTypeId     = helpers.ToInt   (dt.Rows[i]["MemberTypeId"].ToString()),
                                        MemberStateId    = helpers.ToInt   (dt.Rows[i]["MemberStateId"].ToString()),
                                        FinancialStateId = helpers.ToInt   (dt.Rows[i]["FinancialStateId"].ToString()),
                                    };
                                    MemberClubs[i] = MemberClub;
                                }

                                MemberClubRes.MemberClubs = MemberClubs;

                                if (MemberClubRes.Status.Equals("Valid")) return Ok(MemberClubRes);
                            }
                            else
                            {
                                MemberClubRes.Status   = "Error";
                                MemberClubRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("MemClub Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(MemberClubRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberClubResponse getMemberClubs(int ActionType, string Vendor, string Location, MemberClub[] memberClubs)
        {
            return  getMemberClub(ActionType, Vendor, Location, memberClubs[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberClubResponse getMemberClub(int ActionType, string Vendor, string Location, MemberClub memberClub)
        {
            MemberClubRequest  request  = new MemberClubRequest(ActionType, Vendor, Location, memberClub);
            MemberClubResponse response = new MemberClubResponse();

            apiMemberClub apiMemberClub = new apiMemberClub();
            IActionResult action = apiMemberClub.Post(request);
            if (action is OkObjectResult ok) response = (MemberClubResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (MemberClubResponse)bad.Value;
            return response;
        }

    }
}
