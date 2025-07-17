

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
    public class apiFeedItem : ControllerBase
    {
        [HttpPost("FeedItem")]
        [Authorize]
        public IActionResult Post([FromBody] FeedItemRequest request)
        {
            FeedItemResponse cohRes = new FeedItemResponse()
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

                        if (request.FeedItem != null)
                        {
                            FeedItem FeedItem = request.FeedItem;

                            if (FeedItem.HasData())
                            {
                                diag = "1";
                                if (FeedItem.FeedItemId > 0) Params += ";I~S~FeedItemId~" + FeedItem.FeedItemId.ToString();

                                if (!string.IsNullOrEmpty(FeedItem.MediaType))        Params += ";I~S~MediaType~" +         FeedItem.MediaType;
                                if (!string.IsNullOrEmpty(FeedItem.Description))      Params += ";I~S~Description~" +       FeedItem.Description;
                                if (!string.IsNullOrEmpty(FeedItem.FeedType))         Params += ";I~S~FeedType~" +          FeedItem.FeedType;
                                if (!string.IsNullOrEmpty(FeedItem.FeedState))        Params += ";I~S~FeedState~" +         FeedItem.FeedState;
                                if (!string.IsNullOrEmpty(FeedItem.FeedItemType))     Params += ";I~S~FeedItemType~" +      FeedItem.FeedItemType;
                                if (!string.IsNullOrEmpty(FeedItem.FeedItemState))    Params += ";I~S~FeedItemState~" +     FeedItem.FeedItemState;

                                if (FeedItem.Duration > 0)                            Params += ";I~S~Duration~" +          FeedItem.Duration.ToString();
                                if (!string.IsNullOrEmpty(FeedItem.FileName))         Params += ";I~S~FileName~" +          FeedItem.FileName;
                                if (!string.IsNullOrEmpty(FeedItem.FileDate))         Params += ";I~S~FileDate~" +          FeedItem.FileDate;
                                if (!string.IsNullOrEmpty(FeedItem.FileName))         Params += ";I~S~FileName~" +          FeedItem.FileName;
                                if (FeedItem.FileSize > 0)                            Params += ";I~S~FileSize~" +          FeedItem.FileSize.ToString();

                                if (!string.IsNullOrEmpty(FeedItem.ImageData))        Params += ";I~S~ImageData~"   +       FeedItem.ImageData;
                                if (!string.IsNullOrEmpty(FeedItem.ThumbData))        Params += ";I~S~ThumbData~"   +       FeedItem.ThumbData;
                                if (!string.IsNullOrEmpty(FeedItem.Scroller))         Params += ";I~S~Scroller~"    +       FeedItem.Scroller;
                                if (FeedItem.ImageWidth > 0)                          Params += ";I~S~ImageWidth~"  +       FeedItem.ImageWidth.ToString();
                                if (FeedItem.ImageHeight > 0)                         Params += ";I~S~ImageHeight~" +       FeedItem.ImageHeight.ToString();

                                if (FeedItem.FeedId > 0)                              Params += ";I~S~FeedId~" +            FeedItem.FeedId.ToString();
                                if (FeedItem.FeedItemId > 0)                          Params += ";I~S~FeedItemId~" +        FeedItem.FeedItemId.ToString();
                                if (FeedItem.ParentId > 0)                            Params += ";I~S~ParentId~" +          FeedItem.ParentId.ToString();
                                if (FeedItem.MediaTypeId > 0)                         Params += ";I~S~MediaTypeId~" +       FeedItem.MediaTypeId.ToString();
                                if (FeedItem.FeedTypeId > 0)                          Params += ";I~S~FeedTypeId~" +        FeedItem.FeedTypeId.ToString();
                                if (FeedItem.FeedStateId > 0)                         Params += ";I~S~FeedStateId~" +       FeedItem.FeedStateId.ToString();
                                if (FeedItem.FeedItemTypeId > 0)                      Params += ";I~S~FeedItemTypeId~" +    FeedItem.FeedItemTypeId.ToString();
                                if (FeedItem.FeedItemStateId > 0)                     Params += ";I~S~FeedItemStateId~" +   FeedItem.FeedItemStateId.ToString();

                                Params += ScheduleHelpers.GetParams(FeedItem.Schedule);
                            }

                            diag = "4";
                            helpers.WriteToLog("apiFeedItem:" + Params);
                            DataTable dt = db.GetDataTable("med.api_FeedItem", Params);
                            if (dt.Rows.Count > 0)
                            {
                                cohRes.Status   = dt.Rows[0]["Status"].ToString();
                                cohRes.Response = dt.Rows[0]["Response"].ToString();

                                FeedItem[] FeedItems = new FeedItem[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();

                                    FeedItem = new FeedItem()
                                    {
                                        MediaType       =               dt.Rows[i]["MediaType"].ToString(),
                                        Description     =               dt.Rows[i]["Description"].ToString(),
                                        FeedType        =               dt.Rows[i]["FeedType"].ToString(),
                                        FeedState       =               dt.Rows[i]["FeedState"].ToString(),
                                        FeedItemType    =               dt.Rows[i]["FeedItemType"].ToString(),
                                        FeedItemState   =               dt.Rows[i]["FeedItemState"].ToString(),
                                        Duration        = helpers.ToInt(dt.Rows[i]["Duration"].ToString()),
                                        FileName        =               dt.Rows[i]["FileName"].ToString(),
                                        FileDate        =               dt.Rows[i]["FileDate"].ToString(),
                                        FileSize        = helpers.ToInt(dt.Rows[i]["FileSize"].ToString()),
                                        ImageWidth      = helpers.ToInt(dt.Rows[i]["ImageWidth"].ToString()),
                                        ImageHeight     = helpers.ToInt(dt.Rows[i]["ImageHeight"].ToString()),

                                        FeedId          = helpers.ToInt(dt.Rows[i]["FeedId"].ToString()),
                                        FeedItemId      = helpers.ToInt(dt.Rows[i]["FeedItemPk"].ToString()),
                                        ParentId        = helpers.ToInt(dt.Rows[i]["ParentId"].ToString()),
                                        MediaTypeId     = helpers.ToInt(dt.Rows[i]["MediaTypeId"].ToString()),
                                        FeedTypeId      = helpers.ToInt(dt.Rows[i]["FeedTypeId"].ToString()),
                                        FeedStateId     = helpers.ToInt(dt.Rows[i]["FeedStateId"].ToString()),
                                        FeedItemTypeId  = helpers.ToInt(dt.Rows[i]["FeedItemTypeId"].ToString()),
                                        FeedItemStateId = helpers.ToInt(dt.Rows[i]["FeedItemStateId"].ToString()),
                                    };

                                    if (!string.IsNullOrEmpty(dt.Rows[i]["ImageData"].ToString()))
                                    {
                                        byte[] blob = (byte[])dt.Rows[i]["ImageData"];
                                        if (blob.Length > 0) FeedItem.ImageData = Convert.ToBase64String(blob);
                                    }

                                    if (!string.IsNullOrEmpty(dt.Rows[i]["ThumbData"].ToString()))
                                    {
                                        byte[] blob = (byte[])dt.Rows[i]["ThumbData"];
                                        if (blob.Length > 0) FeedItem.ThumbData = Convert.ToBase64String(blob);
                                    }

                                    if (!string.IsNullOrEmpty(dt.Rows[i]["Scroller"].ToString()))
                                    {
                                        byte[] blob = (byte[])dt.Rows[i]["Scroller"];
                                        if (blob.Length > 0) FeedItem.Scroller = Convert.ToBase64String(blob);
                                    }

                                    if (helpers.ToInt(dt.Rows[i]["ScheduleId"].ToString()) > 0)
                                        FeedItem.Schedule = ScheduleHelpers.LoadFromRow(dt.Rows[i]);

                                    FeedItems[i] = FeedItem;
                                }
                                cohRes.FeedItems = FeedItems;

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
                    helpers.WriteToLog("FeedItem Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cohRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedItemResponse getFeedItems(int ActionType, string Vendor, string Location, FeedItem[] FeedItems)
        {
            return getFeedItem(ActionType, Vendor, Location, FeedItems[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public FeedItemResponse getFeedItem(int ActionType, string Vendor, string Location, FeedItem FeedItem)
        {
            FeedItemRequest request = new FeedItemRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                FeedItem     = FeedItem
            };

            FeedItemResponse response = new FeedItemResponse();

            apiFeedItem api = new apiFeedItem();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (FeedItemResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (FeedItemResponse)bad.Value;
            return response;
        }

    }
}
