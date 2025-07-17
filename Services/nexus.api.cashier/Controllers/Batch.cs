
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.cashier;

namespace nexus.api.cashier
{
    [Route("acur")]
    public class apiBatch : ControllerBase
    {
        [HttpPost("Batch")]
        [Authorize]
        public IActionResult Post([FromBody] BatchRequest request)
        {
            BatchResponse BatchRes = new BatchResponse()
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

                    if (request.Batch != null)
                    {
                        Batch Batch = request.Batch;

                        if (Batch.HasData())
                        {
                            if (!string.IsNullOrEmpty(Batch.Description))   Params += ";I~S~Description~" + Batch.Description;
                            if (!string.IsNullOrEmpty(Batch.BatchType))     Params += ";I~S~BatchType~" +   Batch.BatchType;
                            if (!string.IsNullOrEmpty(Batch.BatchState))    Params += ";I~S~BatchState~" +  Batch.BatchState;
                            
                            if (helpers.IsDate(Batch.OpenDate))             Params += ";I~S~OpenDate~" +    Batch.OpenDate;
                            if (helpers.IsDate(Batch.OpenTime))             Params += ";I~S~OpenTime~" +    Batch.OpenTime;
                            if (helpers.IsDate(Batch.CloseDate))            Params += ";I~S~CloseDate~" +   Batch.CloseDate;
                            if (helpers.IsDate(Batch.CloseTime))            Params += ";I~S~CloseTime~" +   Batch.CloseTime;

                            if (Batch.Days > 0)                             Params += ";I~S~Days~" +        Batch.Days.ToString();
                            if (Batch.VenueId > 0)                          Params += ";I~S~VenueId~" +     Batch.VenueId.ToString();
                            if (Batch.ParentId > 0)                         Params += ";I~S~ParentId~" +    Batch.ParentId.ToString();
                            if (Batch.ComputerId > 0)                       Params += ";I~S~ComputerId~" +  Batch.ComputerId.ToString();
                            if (Batch.WorkerId > 0)                         Params += ";I~S~WorkerId~" +    Batch.WorkerId.ToString();
                            if (Batch.PreviousId > 0)                       Params += ";I~S~PreviousId~" +  Batch.PreviousId.ToString();
                            if (Batch.MemoId > 0)                           Params += ";I~S~MemoId~" +      Batch.MemoId.ToString();
                            
                            if (Batch.Value1 > 0)                           Params += ";I~S~Value1~" +      Batch.Value1.ToString();
                            if (Batch.Value2 > 0)                           Params += ";I~S~Value2~" +      Batch.Value2.ToString();
                            if (Batch.Value3 > 0)                           Params += ";I~S~Value3~" +      Batch.Value3.ToString();
                            if (Batch.Value4 > 0)                           Params += ";I~S~Value4~" +      Batch.Value4.ToString();
                            if (Batch.Value5 > 0)                           Params += ";I~S~Value5~" +      Batch.Value5.ToString();
                            if (Batch.Value6 > 0)                           Params += ";I~S~Value6~" +      Batch.Value6.ToString();
                            if (Batch.Value7 > 0)                           Params += ";I~S~Value7~" +      Batch.Value7.ToString();
                            if (Batch.Value8 > 0)                           Params += ";I~S~Value8~" +      Batch.Value8.ToString();

                            if (!string.IsNullOrEmpty(Batch.Field1))        Params += ";I~S~Field1~" +      Batch.Field1;
                            if (!string.IsNullOrEmpty(Batch.Field2))        Params += ";I~S~Field2~" +      Batch.Field2;
                            if (!string.IsNullOrEmpty(Batch.Field3))        Params += ";I~S~Field3~" +      Batch.Field3;
                            if (!string.IsNullOrEmpty(Batch.Field4))        Params += ";I~S~Field4~" +      Batch.Field4;
                            if (!string.IsNullOrEmpty(Batch.Field5))        Params += ";I~S~Field5~" +      Batch.Field5;
                            if (!string.IsNullOrEmpty(Batch.Field6))        Params += ";I~S~Field6~" +      Batch.Field6;
                            if (!string.IsNullOrEmpty(Batch.Field7))        Params += ";I~S~Field7~" +      Batch.Field7;
                            if (!string.IsNullOrEmpty(Batch.Field8))        Params += ";I~S~Field8~" +      Batch.Field8;

                            //        public Lineitem[] Lineitems { get; set; } = Array.Empty<Lineitem>();


                        }

                        helpers.WriteToLog("apiBatch:" + Params);
                        DataTable dt = db.GetDataTable("acur.api_Batch", Params);
                        if (dt.Rows.Count > 0)
                        {
                            BatchRes.Status   = dt.Rows[0]["Status"].ToString();
                            BatchRes.Response = dt.Rows[0]["Response"].ToString();

                            Batch[] Batchs = new Batch[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Batch = new Batch()
                                {
                                    BatchId       = helpers.ToInt (dt.Rows[i]["BatchPk"].ToString()),
                                    //Description  =                dt.Rows[i]["Description"].ToString(),
                                    BatchType    =                dt.Rows[i]["BatchType"].ToString(),
                                    BatchState   =                dt.Rows[i]["BatchState"].ToString(),

                                    OpenDate     =                dt.Rows[i]["OpenDate"].ToString(),
                                    OpenTime     =                dt.Rows[i]["OpenTime"].ToString(),
                                    CloseDate    =                dt.Rows[i]["CloseDate"].ToString(),
                                    CloseTime    =                dt.Rows[i]["CloseTime"].ToString(),

                                    Days         = helpers.ToInt (dt.Rows[i]["Days"].ToString()),
                                    VenueId      = helpers.ToInt (dt.Rows[i]["VenueId"].ToString()),
                                    ParentId     = helpers.ToInt (dt.Rows[i]["ParentId"].ToString()),
                                    ComputerId   = helpers.ToInt (dt.Rows[i]["ComputerId"].ToString()),
                                    WorkerId     = helpers.ToInt (dt.Rows[i]["WorkerId"].ToString()),
                                    PreviousId   = helpers.ToInt (dt.Rows[i]["PreviousId"].ToString()),
                                    MemoId       = helpers.ToInt (dt.Rows[i]["MemoId"].ToString()),

                                    Value1       = helpers.ToDecimal(dt.Rows[i]["Value1"].ToString()),
                                    Value2       = helpers.ToDecimal(dt.Rows[i]["Value2"].ToString()),
                                    Value3       = helpers.ToDecimal(dt.Rows[i]["Value3"].ToString()),
                                    Value4       = helpers.ToDecimal(dt.Rows[i]["Value4"].ToString()),
                                    Value5       = helpers.ToDecimal(dt.Rows[i]["Value5"].ToString()),
                                    Value6       = helpers.ToDecimal(dt.Rows[i]["Value6"].ToString()),
                                    Value7       = helpers.ToDecimal(dt.Rows[i]["Value7"].ToString()),
                                    Value8       = helpers.ToDecimal(dt.Rows[i]["Value8"].ToString()),

                                    Field1       =                   dt.Rows[i]["Field1"].ToString(),
                                    Field2       =                   dt.Rows[i]["Field2"].ToString(),
                                    Field3       =                   dt.Rows[i]["Field3"].ToString(),
                                    Field4       =                   dt.Rows[i]["Field4"].ToString(),
                                    Field5       =                   dt.Rows[i]["Field5"].ToString(),
                                    Field6       =                   dt.Rows[i]["Field6"].ToString(),
                                    Field7       =                   dt.Rows[i]["Field7"].ToString(),
                                    Field8       =                   dt.Rows[i]["Field8"].ToString(),

                                };
                                Batchs[i] = Batch;

                                apiLineitem apiLineitem = new apiLineitem();
                                Lineitem line = new Lineitem() { BatchId = Batch.BatchId };
                                LineitemResponse linRes = apiLineitem.getLineitem(request.ActionType, request.Vendor, request.Location, line);
                                if (linRes.Status.Equals("Valid")) Batch.Lineitems = linRes.Lineitems;
                                else { BatchRes.Status = linRes.Status; BatchRes.Response = linRes.Response; break; }
                            }

                            BatchRes.Batchs = Batchs;

                            if (BatchRes.Status.Equals("Valid")) return Ok(BatchRes);
                        }
                        else
                        {
                            BatchRes.Status   = "Error";
                            BatchRes.Response = "No Data Found, " + db.DataError;
                        }
                    }
                }
            }

            return BadRequest(BatchRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public BatchResponse getBatchs(int ActionType, string Vendor, string Location, Batch[] Batchs)
        {
            return getBatch(ActionType, Vendor, Location, Batchs[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public BatchResponse getBatch(int ActionType, string Vendor, string Location, Batch Batch)
        {
            BatchRequest request = new BatchRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Batch       = Batch
            };

            BatchResponse response = new BatchResponse();

            apiBatch api = new apiBatch();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (BatchResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (BatchResponse)bad.Value;
            return response;
        }
    }
}
