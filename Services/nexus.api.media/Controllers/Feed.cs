

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
    public class apiFeed : ControllerBase
    {
        [HttpPost("Feed")]
        [Authorize]
        public IActionResult Post([FromBody] FeedRequest request)
        {
            FeedResponse cohRes = new FeedResponse()
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

                        if (request.Feed != null)
                        {
                            Feed Feed = request.Feed;

                            if (Feed.HasData())
                            {
                                diag = "1";
                                if (Feed.FeedId > 0) Params += ";I~S~FeedId~" + Feed.FeedId.ToString();

                                if (!string.IsNullOrEmpty(Feed.FeedType))     Params += ";I~S~FeedType~" +    Feed.FeedType;
                                if (!string.IsNullOrEmpty(Feed.FeedState))    Params += ";I~S~FeedState~" +   Feed.FeedState;

                                if (Feed.FeedTypeId > 0)    Params += ";I~S~FeedTypeId~" +    Feed.FeedTypeId.ToString();
                                if (Feed.FeedStateId > 0)   Params += ";I~S~FeedStateId~" +   Feed.FeedStateId.ToString();

                                Params += ScheduleHelpers.GetParams(Feed.Schedule);
                            }

                            diag = "4";
                            helpers.WriteToLog("apiFeed:" + Params);
                            DataTable dt = db.GetDataTable("med.api_Feed", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                Feed[] Feeds = new Feed[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    Feed = new Feed()
                                    {
                                        FeedId = helpers.ToInt(dt.Rows[i]["FeedPk"].ToString()),

                                        FeedType    = dt.Rows[i]["FeedType"].ToString(),
                                        FeedState   = dt.Rows[i]["FeedState"].ToString(),
                                        Description = dt.Rows[i]["Description"].ToString(),

                                        FeedTypeId  = helpers.ToInt(dt.Rows[i]["FeedTypeId"].ToString()),
                                        FeedStateId = helpers.ToInt(dt.Rows[i]["FeedStateId"].ToString()),
                                    };

                                    if (helpers.ToInt(dt.Rows[i]["ScheduleId"].ToString()) > 0)
                                        Feed.Schedule = ScheduleHelpers.LoadFromRow(dt.Rows[i]);

                                    Feeds[i] = Feed;
                                }
                                cohRes.Feeds = Feeds;

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
                    helpers.WriteToLog("Feed Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedResponse getFeeds(int ActionType, string Vendor, string Location, Feed[] Feeds)
        {
            return getFeed(ActionType, Vendor, Location, Feeds[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedResponse getFeed(int ActionType, string Vendor, string Location, Feed Feed)
        {
            FeedRequest request = new FeedRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Feed     = Feed
            };

            FeedResponse response = new FeedResponse();

            apiFeed api = new apiFeed();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (FeedResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (FeedResponse)bad.Value;
            return response;
        }

    }
}
