using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using nexus.common;
using nexus.common.dal;
using nexus.common.cache;
using nexus.shared.gaming;

namespace nexus.api.gaming
{
    [Route("gam")]
    public class apiToday : ControllerBase
    {
        [HttpPost("today")]
        [Authorize]
        public IActionResult Post([FromBody] TodayRequest request)
        {
            TodayResponse response = new()
            {
                Status = "Bad Request",
                Response = "Invalid Request"
            };

            if (request != null)
            {
                try
                {
                    using SQLServer db = new(setting.ConnectionString);
                    string Params  = "I~S~ActionType~" + request.ActionType;
                           Params += ";I~S~Vendor~"    + request.Vendor;
                           Params += ";I~S~Location~"  + request.Location;

                    if (request.EgmID.HasValue)        Params += ";I~I~EgmID~"       + request.EgmID.Value;
                    if (request.MemberID.HasValue)     Params += ";I~I~MemberID~"    + request.MemberID.Value;
                    if (request.PromotionID.HasValue)  Params += ";I~I~PromotionID~" + request.PromotionID.Value;
                    if (request.TodayTypeID.HasValue)  Params += ";I~I~TodayTypeID~" + request.TodayTypeID.Value;
                    if (request.DayID.HasValue)        Params += ";I~I~DayID~"       + request.DayID.Value;

                    if (request.FromDate.HasValue)     Params += ";I~D~FromDate~"    + request.FromDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    if (request.ToDate.HasValue)       Params += ";I~D~ToDate~"      + request.ToDate.Value.  ToString("yyyy-MM-dd HH:mm:ss");

                    helpers.WriteToLog("apiToday: " + Params);

                    DataTable dt = db.GetDataTable("gam.api_today", Params);

                    if (dt.Rows.Count > 0)
                    {
                        response.Status   = "Valid";
                        response.Response = "";

                        Today[] Results = new Today[dt.Rows.Count];
                        Today   Result  = new Today();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (request.ActionType.Equals(0))
                            {
                                Result = new Today()
                                {
                                    OpenTime       =                    dt.Rows[i]["OpenTime"].ToString(),
                                    CloseTime      =                    dt.Rows[i]["CloseTime"].ToString(),
                                    Description    =                    dt.Rows[i]["Description"].ToString(),
                                    CurrentDate    = helpers.ToDate    (dt.Rows[i]["CurrentDate"].ToString()),
                                    CurrentType    =                    dt.Rows[i]["CurrentType"].ToString(),
                                    TurnOver       = helpers.ToDecimal (dt.Rows[i]["TurnOver"].ToString()),
                                    TotalWin       = helpers.ToDecimal (dt.Rows[i]["TotalWin"].ToString()),
                                    CashBox        = helpers.ToDecimal (dt.Rows[i]["CashBox"].ToString()),
                                    CanCredit      = helpers.ToDecimal (dt.Rows[i]["CanCredit"].ToString()),
                                    MoneyIn        = helpers.ToDecimal (dt.Rows[i]["MoneyIn"].ToString()),
                                    MoneyOut       = helpers.ToDecimal (dt.Rows[i]["MoneyOut"].ToString()),
                                    CashIn         = helpers.ToDecimal (dt.Rows[i]["CashIn"].ToString()),
                                    CashOut        = helpers.ToDecimal (dt.Rows[i]["CashOut"].ToString()),
                                    JackPot        = helpers.ToDecimal (dt.Rows[i]["JackPot"].ToString()),
                                    Stroke         = helpers.ToInt     (dt.Rows[i]["Stroke"].ToString()),
                                    Powerup        = helpers.ToInt     (dt.Rows[i]["Powerup"].ToString()),
                                    Existing       = helpers.ToDecimal (dt.Rows[i]["Existing"].ToString()),
                                    Interface      = helpers.ToInt     (dt.Rows[i]["Interface"].ToString()),
                                    Clear          = helpers.ToDecimal (dt.Rows[i]["Clear"].ToString()),
                                    CCDocket       = helpers.ToDecimal (dt.Rows[i]["CCDocket"].ToString()),
                                    ShortPay       = helpers.ToDecimal (dt.Rows[i]["ShortPay"].ToString()),
                                    Refill         = helpers.ToDecimal (dt.Rows[i]["Refill"].ToString()),
                                    LinkWin        = helpers.ToDecimal (dt.Rows[i]["LinkWin"].ToString()),
                                    LinkCCCE       = helpers.ToDecimal (dt.Rows[i]["LinkCCCE"].ToString()),
                                    Five           = helpers.ToInt     (dt.Rows[i]["Five"].ToString()),
                                    Ten            = helpers.ToInt     (dt.Rows[i]["Ten"].ToString()),
                                    Twenty         = helpers.ToInt     (dt.Rows[i]["Twenty"].ToString()),
                                    Fifty          = helpers.ToInt     (dt.Rows[i]["Fifty"].ToString()),
                                    Hundred        = helpers.ToInt     (dt.Rows[i]["Hundred"].ToString()),
                                    FiveHundred    = helpers.ToInt     (dt.Rows[i]["FiveHundred"].ToString()),
                                    Thousand       = helpers.ToInt     (dt.Rows[i]["Thousand"].ToString()),
                                    TotalBills     = helpers.ToInt     (dt.Rows[i]["TotalBills"].ToString()),
                                    TotalValue     = helpers.ToDecimal (dt.Rows[i]["TotalValue"].ToString()),
                                    Online         = helpers.ToDecimal (dt.Rows[i]["Online"].ToString()),
                                    CardIn         = helpers.ToDecimal (dt.Rows[i]["CardIn"].ToString()),
                                    CardCount      = helpers.ToInt     (dt.Rows[i]["CardCount"].ToString()),
                                    InPlay         = helpers.ToDecimal (dt.Rows[i]["InPlay"].ToString()),
                                    PlayCount      = helpers.ToInt     (dt.Rows[i]["PlayCount"].ToString()),
                                    Fault          = helpers.ToInt     (dt.Rows[i]["Fault"].ToString()),
                                    FaultID        = helpers.ToInt     (dt.Rows[i]["FaultID"].ToString()),
                                    FaultCount     = helpers.ToInt     (dt.Rows[i]["FaultCount"].ToString()),
                                    HotPlay        = helpers.ToInt     (dt.Rows[i]["HotPlay"].ToString()),
                                    Service        = helpers.ToInt     (dt.Rows[i]["Service"].ToString()),
                                    ServiceStart   = helpers.ToDate    (dt.Rows[i]["ServiceStart"].ToString()),
                                    Staff          = helpers.ToInt     (dt.Rows[i]["Staff"].ToString()),
                                    StaffStart     = helpers.ToDate    (dt.Rows[i]["StaffStart"].ToString()),
                                    Cancel         = helpers.ToInt     (dt.Rows[i]["Cancel"].ToString()),
                                    CancelStart    = helpers.ToDate    (dt.Rows[i]["CancelStart"].ToString()),
                                    CancelAmount   = helpers.ToDecimal (dt.Rows[i]["CancelAmount"].ToString()),
                                    Timeout        = helpers.ToInt     (dt.Rows[i]["Timeout"].ToString()),
                                    TimeoutStart   = helpers.ToDate    (dt.Rows[i]["TimeoutStart"].ToString()),
                                    Member         =                    dt.Rows[i]["Member"].ToString(),
                                    MemberTO       = helpers.ToDecimal (dt.Rows[i]["MemberTO"].ToString()),
                                    MemberPts      = helpers.ToDecimal (dt.Rows[i]["MemberPts"].ToString()),
                                    Promotion      =                    dt.Rows[i]["Promotion"].ToString(),
                                    PointsRate     = helpers.ToDecimal (dt.Rows[i]["PointsRate"].ToString()),
                                    CashlessIn     = helpers.ToDecimal (dt.Rows[i]["CashlessIn"].ToString()),
                                    CashlessOut    = helpers.ToDecimal (dt.Rows[i]["CashlessOut"].ToString()),
                                    CashlessPay    = helpers.ToDecimal (dt.Rows[i]["CashlessPay"].ToString()),
                                    CashlessEGM    = helpers.ToDecimal (dt.Rows[i]["CashlessEGM"].ToString()),
                                    MemberID       = helpers.ToInt     (dt.Rows[i]["MemberID"].ToString()),
                                    DeviceNo       = helpers.ToInt     (dt.Rows[i]["DeviceNo"].ToString()),
                                    ReferenceNo    =                    dt.Rows[i]["ReferenceNo"].ToString(),
                                    TodayPK        = helpers.ToInt     (dt.Rows[i]["TodayPK"].ToString()),
                                    TodayTypeId    = helpers.ToInt     (dt.Rows[i]["TodayTypeId"].ToString()),
                                    EgmID          = helpers.ToInt     (dt.Rows[i]["EgmID"].ToString()),
                                    PromotionID    = helpers.ToInt     (dt.Rows[i]["PromotionID"].ToString()),
                                    OwnerID        = helpers.ToInt     (dt.Rows[i]["OwnerID"].ToString()),
                                    VenueID        = helpers.ToInt     (dt.Rows[i]["VenueID"].ToString()),
                                    PeriodId       = helpers.ToInt     (dt.Rows[i]["PeriodId"].ToString()),
                                    MinuteId       = helpers.ToInt     (dt.Rows[i]["MinuteId"].ToString()),
                                    DayId          = helpers.ToInt     (dt.Rows[i]["DayId"].ToString()),
                                    WeekId         = helpers.ToInt     (dt.Rows[i]["WeekId"].ToString()),
                                    MonthId        = helpers.ToInt     (dt.Rows[i]["MonthId"].ToString()),
                                    QuarterId      = helpers.ToInt     (dt.Rows[i]["QuarterId"].ToString()),
                                    YearId         = helpers.ToInt     (dt.Rows[i]["YearId"].ToString()),
                                    ShiftId        = helpers.ToInt     (dt.Rows[i]["ShiftId"].ToString()),
                                    IntervalId     = helpers.ToInt     (dt.Rows[i]["IntervalId"].ToString()),
                                    Inserted       = helpers.ToDate    (dt.Rows[i]["Inserted"].ToString()),
                                    Sequence       = helpers.ToInt     (dt.Rows[i]["Sequence"].ToString())
                                };
                            }

                            if (request.ActionType.Equals(1))
                            {
                                Result = new Today()
                                {
                                    OpenTime       =                    dt.Rows[i]["OpenTime"].ToString(),
                                    CloseTime      =                    dt.Rows[i]["CloseTime"].ToString(),
                                    TurnOver       = helpers.ToDecimal (dt.Rows[i]["TurnOver"].ToString()),
                                    TotalWin       = helpers.ToDecimal (dt.Rows[i]["TotalWin"].ToString()),
                                    CashBox        = helpers.ToDecimal (dt.Rows[i]["CashBox"].ToString()),
                                    CanCredit      = helpers.ToDecimal (dt.Rows[i]["CanCredit"].ToString()),
                                    MoneyIn        = helpers.ToDecimal (dt.Rows[i]["MoneyIn"].ToString()),
                                    MoneyOut       = helpers.ToDecimal (dt.Rows[i]["MoneyOut"].ToString()),
                                    CashIn         = helpers.ToDecimal (dt.Rows[i]["CashIn"].ToString()),
                                    CashOut        = helpers.ToDecimal (dt.Rows[i]["CashOut"].ToString()),
                                    Clear          = helpers.ToDecimal (dt.Rows[i]["Clear"].ToString()),
                                    CCDocket       = helpers.ToDecimal (dt.Rows[i]["CCDocket"].ToString()),
                                    ShortPay       = helpers.ToDecimal (dt.Rows[i]["ShortPay"].ToString()),
                                    LinkWin        = helpers.ToDecimal (dt.Rows[i]["LinkWin"].ToString()),
                                    LinkCCCE       = helpers.ToDecimal (dt.Rows[i]["LinkCCCE"].ToString()),
                                    TotalBills     = helpers.ToInt     (dt.Rows[i]["TotalBills"].ToString()),
                                    TotalValue     = helpers.ToDecimal (dt.Rows[i]["TotalValue"].ToString()),
                                    Online         = helpers.ToDecimal (dt.Rows[i]["Online"].ToString()),
                                    CardIn         = helpers.ToDecimal (dt.Rows[i]["CardIn"].ToString()),
                                    CardCount      = helpers.ToInt     (dt.Rows[i]["CardCount"].ToString()),
                                    InPlay         = helpers.ToDecimal (dt.Rows[i]["InPlay"].ToString()),
                                    PlayCount      = helpers.ToInt     (dt.Rows[i]["PlayCount"].ToString()),
                                    Fault          = helpers.ToInt     (dt.Rows[i]["Fault"].ToString()),
                                    FaultID        = helpers.ToInt     (dt.Rows[i]["FaultID"].ToString()),
                                    FaultCount     = helpers.ToInt     (dt.Rows[i]["FaultCount"].ToString()),
                                    HotPlay        = helpers.ToInt     (dt.Rows[i]["HotPlay"].ToString())
                                };
                            }


                            Results[i] = Result;                               
                        }
                        response.Results = Results;

                        if (response.Status.Equals("Valid")) return Ok(response);
                    }
                    else
                    {
                        response.Status   = "Error";
                        response.Response = "No Data Found, " + db.DataError;
                    }                    
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("apiToday Err: " + ex.Message);
                    response.Status = "Exception";
                    response.Response = ex.Message;
                }
            }

            return BadRequest(response);
        }
    }
}
