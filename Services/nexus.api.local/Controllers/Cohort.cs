
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiCohort : ControllerBase
    {
        [HttpPost("Cohort")]
        [Authorize]
        public IActionResult Post([FromBody] CohortRequest request)
        {
            CohortResponse cohRes = new CohortResponse()
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

                        if (request.Cohort != null)
                        {
                            Cohort cohort = request.Cohort;

                            if (cohort.HasData())
                            {
                                diag = "1";
                                if (cohort.CohortId > 0) Params += ";I~S~cohortId~" + cohort.CohortId.ToString();

                                if (!string.IsNullOrEmpty(cohort.UseageType))     Params += ";I~S~UseageType~" +    cohort.UseageType;
                                if (!string.IsNullOrEmpty(cohort.CohortType))     Params += ";I~S~CohortType~" +    cohort.CohortType;
                                if (!string.IsNullOrEmpty(cohort.CohortState))    Params += ";I~S~CohortState~" +   cohort.CohortState;
                                
                                diag = "2";
                                if (cohort.Start  > 0)  Params += ";I~S~Start~"  + cohort.Start.ToString();
                                if (cohort.Finish  > 0) Params += ";I~S~Finish~" + cohort.Finish.ToString();
                                if (cohort.Level > 0)   Params += ";I~S~Level~"  + cohort.Level.ToString();
                                if (cohort.Value1 > 0)  Params += ";I~S~Value1~" + cohort.Value1.ToString();
                                if (cohort.Value2 > 0)  Params += ";I~S~Value2~" + cohort.Value2.ToString();
                                if (cohort.Value3 > 0)  Params += ";I~S~Value3~" + cohort.Value3.ToString();
                                if (cohort.Value4 > 0)  Params += ";I~S~Value4~" + cohort.Value4.ToString();

                                diag = "3";
                                if (cohort.PromotionId > 0)     Params += ";I~S~PromotionId~" +     cohort.PromotionId.ToString();
                                if (cohort.UseageTypeId > 0)    Params += ";I~S~UseageTypeId~" +    cohort.UseageTypeId.ToString();
                                if (cohort.CohortTypeId > 0)    Params += ";I~S~CohortTypeId~" +    cohort.CohortTypeId.ToString();
                                if (cohort.CohortStateId > 0)   Params += ";I~S~CohortStateId~" +   cohort.CohortStateId.ToString();
                                if (cohort.InterfaceType > 0)   Params += ";I~S~InterfaceType~" +   cohort.InterfaceType.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiCohort:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_Cohort", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                Cohort[] Cohorts = new Cohort[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    cohort = new Cohort()
                                    {
                                        CohortId        = helpers.ToInt     (dt.Rows[i]["CohortPk"].ToString()),

                                        UseageType      =                    dt.Rows[i]["UseageType"].ToString(),
                                        CohortType      =                    dt.Rows[i]["CohortType"].ToString(),
                                        CohortState     =                    dt.Rows[i]["CohortState"].ToString(),

                                        Start           = helpers.ToInt     (dt.Rows[i]["Start"].ToString()),
                                        Finish          = helpers.ToInt     (dt.Rows[i]["Finish"].ToString()),
                                        Level           = helpers.ToInt     (dt.Rows[i]["Level"].ToString()),

                                        Value1          = helpers.ToDecimal (dt.Rows[i]["Value1"].ToString()),
                                        Value2          = helpers.ToDecimal (dt.Rows[i]["Value2"].ToString()),
                                        Value3          = helpers.ToDecimal (dt.Rows[i]["Value3"].ToString()),
                                        Value4          = helpers.ToDecimal (dt.Rows[i]["Value4"].ToString()),

                                        PromotionId     = helpers.ToInt     (dt.Rows[i]["PromotionId"].ToString()),
                                        UseageTypeId    = helpers.ToInt     (dt.Rows[i]["UseageTypeId"].ToString()),
                                        CohortTypeId    = helpers.ToInt     (dt.Rows[i]["CohortTypeId"].ToString()),
                                        CohortStateId   = helpers.ToInt     (dt.Rows[i]["CohortStateId"].ToString()),
                                        InterfaceType   = helpers.ToInt     (dt.Rows[i]["InterfaceType"].ToString())
                                    };
                                    Cohorts[i] = cohort;

                                }
                                cohRes.Cohorts = Cohorts;

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
                    helpers.WriteToLog("Cohort Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CohortResponse getCohorts(int ActionType, string Vendor, string Location, Cohort[] Cohorts)
        {
            return getCohort(ActionType, Vendor, Location, Cohorts[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CohortResponse getCohort(int ActionType, string Vendor, string Location, Cohort Cohort)
        {
            CohortRequest request = new CohortRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Cohort     = Cohort
            };

            CohortResponse response = new CohortResponse();

            apiCohort api = new apiCohort();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (CohortResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (CohortResponse)bad.Value;
            return response;
        }

    }
}
