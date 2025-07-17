

using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.media;

namespace nexus.api.media
{
    [Route("med")]
    public class apiFeedSource : ControllerBase
    {
        [HttpPost("FeedSource")]
        [Authorize]
        public IActionResult Post([FromBody] FeedSourceRequest request)
        {
            FeedSourceResponse cohRes = new FeedSourceResponse()
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

                        if (request.FeedSource != null)
                        {
                            FeedSource FeedSource = request.FeedSource;

                            if (FeedSource.HasData())
                            {
                                diag = "1";
                                if (FeedSource.FeedSourceId > 0) Params += ";I~S~FeedSourceId~" + FeedSource.FeedSourceId.ToString();

                                if (!string.IsNullOrEmpty(FeedSource.FeedSourceType))     Params += ";I~S~FeedSourceType~" +      FeedSource.FeedSourceType;
                                if (!string.IsNullOrEmpty(FeedSource.FeedSourceState))    Params += ";I~S~FeedSourceState~" +     FeedSource.FeedSourceState;
                                if (!string.IsNullOrEmpty(FeedSource.LocalSource))        Params += ";I~S~MediaType~" +           FeedSource.LocalSource;
                                if (!string.IsNullOrEmpty(FeedSource.RemoteSource))       Params += ";I~S~Description~" +         FeedSource.RemoteSource;

                                if (FeedSource.FeedSourceId > 0)                          Params += ";I~S~FeedSourceId~" +        FeedSource.FeedSourceId.ToString();
                                if (FeedSource.FeedSourceTypeId > 0)                      Params += ";I~S~FeedSourceTypeId~" +    FeedSource.FeedSourceTypeId.ToString();
                                if (FeedSource.FeedSourceStateId > 0)                     Params += ";I~S~FeedSourceStateId~" +   FeedSource.FeedSourceStateId.ToString();

                                Params += ScheduleHelpers.GetParams(FeedSource.Schedule);
                            }

                            diag = "4";
                            helpers.WriteToLog("apiFeedSource:" + Params);
                            DataTable dt = db.GetDataTable("med.api_FeedSource", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                FeedSource[] FeedSources = new FeedSource[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();

                                    FeedSource = new FeedSource()
                                    {
                                        FeedSourceType    =               dt.Rows[i]["FeedSourceType"].ToString(),
                                        FeedSourceState   =               dt.Rows[i]["FeedSourceState"].ToString(),
                                        LocalSource       =               dt.Rows[i]["LocalSource"].ToString(),
                                        RemoteSource      =               dt.Rows[i]["RemoteSource"].ToString(),

                                        FeedSourceId      = helpers.ToInt(dt.Rows[i]["FeedSourcePk"].ToString()),
                                        FeedSourceTypeId  = helpers.ToInt(dt.Rows[i]["FeedSourceTypeId"].ToString()),
                                        FeedSourceStateId = helpers.ToInt(dt.Rows[i]["FeedSourceStateId"].ToString()),

                                    };

                                    if (helpers.ToInt(dt.Rows[i]["ScheduleId"].ToString()) > 0)
                                        FeedSource.Schedule = ScheduleHelpers.LoadFromRow(dt.Rows[i]);

                                    FeedSources[i] = FeedSource;
                                }
                                cohRes.FeedSources = FeedSources;

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
                    helpers.WriteToLog("FeedSource Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedSourceResponse getFeedSources(int ActionType, string Vendor, string Location, FeedSource[] FeedSources)
        {
            return getFeedSource(ActionType, Vendor, Location, FeedSources[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedSourceResponse getFeedSource(int ActionType, string Vendor, string Location, FeedSource FeedSource)
        {
            FeedSourceRequest request = new FeedSourceRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                FeedSource     = FeedSource
            };

            FeedSourceResponse response = new FeedSourceResponse();

            apiFeedSource api = new apiFeedSource();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (FeedSourceResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (FeedSourceResponse)bad.Value;
            return response;
        }

    }
}
