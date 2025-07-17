
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
    public class apiClub : ControllerBase
    {
        [HttpPost("Club")]
        [Authorize]
        public IActionResult Post([FromBody] ClubRequest request)
        {
            ClubResponse clubRes = new ClubResponse()
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

                        if (request.Club != null)
                        {
                            Club club = request.Club;

                            if (club.HasData())
                            {
                                diag = "1";
                                if (!string.IsNullOrEmpty(club.Description))   Params += ";I~S~Description~" + club.Description;
                                if (!string.IsNullOrEmpty(club.ClubType))      Params += ";I~S~ClubType~" +    club.ClubType;
                                if (!string.IsNullOrEmpty(club.ClubCode))      Params += ";I~S~ClubCode~" +    club.ClubCode;
                                if (!string.IsNullOrEmpty(club.Gender))        Params += ";I~S~Gender~" +      club.Gender;
                                if (!string.IsNullOrEmpty(club.RollType))      Params += ";I~S~RollType~" +    club.RollType;
                                if (!string.IsNullOrEmpty(club.ProcessDays))   Params += ";I~S~ProcessDays~" + club.ProcessDays;

                                diag = "2";
                                if (club.ClubId > 0)       Params += ";I~S~ClubId~" +       club.ClubId.ToString();
                                if (club.AutoRenew > 0)    Params += ";I~S~AutoRenew~" +    club.AutoRenew.ToString();
                                if (club.Prorata > 0)      Params += ";I~S~Prorata~" +      club.Prorata.ToString();
                                if (club.UniqueFees > 0)   Params += ";I~S~UniqueFees~" +   club.UniqueFees.ToString();
                                if (club.Grace > 0)        Params += ";I~S~Grace~" +        club.Grace.ToString();
                                if (club.Lapse > 0)        Params += ";I~S~Lapse~" +        club.Lapse.ToString();
                                if (club.UnFinancial > 0)  Params += ";I~S~UnFinancial~" +  club.UnFinancial.ToString();
                                if (club.UnFinCnt > 0)     Params += ";I~S~UnFinCnt~" +     club.UnFinCnt.ToString();
                                if (club.RenewalMonth > 0) Params += ";I~S~RenewalMonth~" + club.RenewalMonth.ToString();
                                if (club.CohortId > 0)     Params += ";I~S~CohortId~" +     club.CohortId.ToString();
                                if (club.ParentId > 0)     Params += ";I~S~ParentId~" +     club.ParentId.ToString();
                                if (club.DaysBeforeExpire > 0) Params += ";I~S~DaysBeforeExpire~" + club.DaysBeforeExpire.ToString();
                            }

                            diag = "3";
                            helpers.WriteToLog("apiClub:" + Params);
                            DataTable dt = db.GetDataTable("mem.api_Club", Params);
                            if (dt.Rows.Count > 0)
                            {
                                diag = "3";
                                clubRes.Status   = dt.Rows[0]["Status"].ToString();
                                clubRes.Response = dt.Rows[0]["Response"].ToString();

                                Club[] Clubs = new Club[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "4" + i.ToString();

                                    club = new Club()
                                    {
                                        ClubId       = helpers.ToInt (dt.Rows[i]["ClubId"].ToString()),
                                        Description  =                dt.Rows[i]["Description"].ToString(),
                                        ClubType     =                dt.Rows[i]["ClubType"].ToString(),
                                        ClubCode     =                dt.Rows[i]["ClubCode"].ToString(),
                                        Gender       =                dt.Rows[i]["Gender"].ToString(),
                                        Parent       =                dt.Rows[i]["Parent"].ToString(),
                                        AutoRenew    = helpers.ToInt (dt.Rows[i]["AutoRenew"].ToString()),
                                        Prorata      = helpers.ToInt (dt.Rows[i]["ProRata"].ToString()),
                                        UniqueFees   = helpers.ToInt (dt.Rows[i]["UniqueFees"].ToString()),
                                        RollType     =                dt.Rows[i]["RollType"].ToString(),
                                        Grace        = helpers.ToInt (dt.Rows[i]["Grace"].ToString()),
                                        Lapse        = helpers.ToInt (dt.Rows[i]["Lapse"].ToString()),
                                        UnFinancial  = helpers.ToInt (dt.Rows[i]["UnFinancial"].ToString()),
                                        UnFinCnt     = helpers.ToInt (dt.Rows[i]["UnFinCnt"].ToString()),
                                        RenewalMonth = helpers.ToInt (dt.Rows[i]["RenewalMonth"].ToString()),
                                        ProcessDays  =                dt.Rows[i]["ProcessDays"].ToString(),
                                        CohortId     = helpers.ToInt (dt.Rows[i]["CohortId"].ToString()),
                                        ParentId     = helpers.ToInt (dt.Rows[i]["ParentId"].ToString()),
                                        DaysBeforeExpire = helpers.ToInt(dt.Rows[i]["DaysBeforeExpire"].ToString())
                                    };
                                    Clubs[i] = club;
                                }
                                clubRes.Clubs = Clubs;

                                if (clubRes.Status.Equals("Valid")) return Ok(clubRes);
                            }
                            else
                            {
                                clubRes.Status   = "Error";
                                clubRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Club Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(clubRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ClubResponse getClubs(int ActionType, string Vendor, string Location, Club[] Clubs)
        {
            return getClub(ActionType, Vendor, Location, Clubs[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ClubResponse getClub(int ActionType, string Vendor, string Location, Club Club)
        {
            ClubRequest request = new ClubRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Club       = Club
            };

            ClubResponse response = new ClubResponse();

            apiClub api = new apiClub();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (ClubResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (ClubResponse)bad.Value;
            return response;
        }
    }
}
