
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common.dal;
using nexus.shared.cashier;


namespace nexus.api.cashier
{
    [Route("acur")]
    public class apiLineitem : ControllerBase
    {
        [HttpPost("Lineitem")]
        [Authorize]
        public IActionResult Post([FromBody] LineitemRequest request)
        {
            LineitemResponse LineitemRes = new LineitemResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request != null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {

                    string Params  =  "I~S~ActionType~" + request.ActionType;
                           Params += ";I~S~Vendor~"     + request.Vendor;
                           Params += ";I~S~Location~"   + request.Location;

                    if (request.Lineitem != null)
                    {
                        Lineitem Lineitem = request.Lineitem;

                        if (Lineitem.HasData())
                        {
                            if (!string.IsNullOrEmpty(Lineitem.LineitemType))     Params += ";I~S~LineitemType~" +  Lineitem.LineitemType;
                            if (!string.IsNullOrEmpty(Lineitem.LineitemState))    Params += ";I~S~LineitemState~" + Lineitem.LineitemState;
                            if (!string.IsNullOrEmpty(Lineitem.LineitemNo))       Params += ";I~S~LineitemNo~" +    Lineitem.LineitemNo;
                            if (!string.IsNullOrEmpty(Lineitem.Reference))        Params += ";I~S~Reference~" +     Lineitem.Reference;
                            if (!string.IsNullOrEmpty(Lineitem.Detail))           Params += ";I~S~Detail~" +        Lineitem.Detail;
                            if (helpers.IsDate(Lineitem.LineitemDate))            Params += ";I~S~LineitemDate~" +  Lineitem.LineitemDate;
                            if (Lineitem.BatchId > 0)                             Params += ";I~S~BatchId~" +       Lineitem.BatchId.ToString();
                            if (Lineitem.LineitemId > 0)                          Params += ";I~S~LineitemId~" +    Lineitem.LineitemId.ToString();
                            if (Lineitem.Amount > 0)                              Params += ";I~S~Amount~" +        Lineitem.Amount.ToString();
                            if (Lineitem.ComputerId > 0)                          Params += ";I~S~ComputerId~" +    Lineitem.ComputerId.ToString();
                            if (Lineitem.WorkerId > 0)                            Params += ";I~S~WorkerId~" +      Lineitem.WorkerId.ToString();
                            if (Lineitem.HumanId > 0)                             Params += ";I~S~HumanId~" +       Lineitem.HumanId.ToString();
                            if (Lineitem.DeviceId > 0)                            Params += ";I~S~DeviceId~" +      Lineitem.DeviceId.ToString();
                            if (Lineitem.LocationId > 0)                          Params += ";I~S~LocationId~" +    Lineitem.LocationId.ToString();
                            if (Lineitem.EntryId > 0)                             Params += ";I~S~EntryId~" +       Lineitem.EntryId.ToString();
                            if (Lineitem.MediaId > 0)                             Params += ";I~S~MediaId~" +       Lineitem.MediaId.ToString();
                            if (Lineitem.CRC > 0)                                 Params += ";I~S~CRC~" +           Lineitem.CRC.ToString();
                        }

                        helpers.WriteToLog("apiLineitem:" + Params);
                        DataTable dt = db.GetDataTable("acur.api_Lineitem", Params);
                        if (dt.Rows.Count > 0)
                        {
                            LineitemRes.Status   = dt.Rows[0]["Status"].ToString();
                            LineitemRes.Response = dt.Rows[0]["Response"].ToString();

                            Lineitem[] Lineitems = new Lineitem[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Lineitem = new Lineitem()
                                {
                                    LineitemType     =                dt.Rows[i]["LineitemType"].ToString(),
                                    LineitemState    =                dt.Rows[i]["LineitemState"].ToString(),
                                    LineitemNo       =                dt.Rows[i]["LineitemNo"].ToString(),
                                    Reference        =                dt.Rows[i]["Reference"].ToString(),
                                    Detail           =                dt.Rows[i]["Detail"].ToString(),

                                    BatchId          = helpers.ToInt (dt.Rows[i]["BatchId"].ToString()),
                                    LineitemId       = helpers.ToInt (dt.Rows[i]["LineitemPk"].ToString()),
                                    ComputerId       = helpers.ToInt (dt.Rows[i]["ComputerId"].ToString()),
                                    WorkerId         = helpers.ToInt (dt.Rows[i]["WorkerId"].ToString()),
                                    HumanId          = helpers.ToInt (dt.Rows[i]["HumanId"].ToString()),
                                    DeviceId         = helpers.ToInt (dt.Rows[i]["DeviceId"].ToString()),
                                    LocationId       = helpers.ToInt (dt.Rows[i]["LocationId"].ToString()),
                                    EntryId          = helpers.ToInt (dt.Rows[i]["EntryId"].ToString()),
                                    MediaId          = helpers.ToInt (dt.Rows[i]["MediaId"].ToString()),
                                    CRC              = helpers.ToInt (dt.Rows[i]["CRC"].ToString()),

                                    Amount           = helpers.ToDecimal (dt.Rows[i]["Amount"].ToString()),
                                    LineitemDate     = helpers.ToDate    (dt.Rows[i]["LineitemDate"].ToString()).ToString("dd MMM, yyyy HH:mm:ss"),
                                };
                                Lineitems[i] = Lineitem;
                            }
                            LineitemRes.Lineitems = Lineitems;

                            if (LineitemRes.Status.Equals("Valid")) return Ok(LineitemRes);
                        }
                        else
                        {
                            LineitemRes.Status   = "Error";
                            LineitemRes.Response = "No Data Found, " + db.DataError;
                        }
                    }
                }
            }

            return BadRequest(LineitemRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public LineitemResponse getLineitems(int ActionType, string Vendor, string Location, Lineitem[] Lineitems)
        {
            return getLineitem(ActionType, Vendor, Location, Lineitems[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public LineitemResponse getLineitem(int ActionType, string Vendor, string Location, Lineitem Lineitem)
        {
            LineitemRequest request = new LineitemRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Lineitem       = Lineitem
            };

            LineitemResponse response = new LineitemResponse();

            apiLineitem api = new apiLineitem();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (LineitemResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (LineitemResponse)bad.Value;
            return response;
        }
    }
}
