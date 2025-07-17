namespace nexus.shared.gaming
{
    public class TodayRequest
    {
        public int       ActionType   { get; set; } = 0;
        public string    Vendor       { get; set; } = string.Empty;
        public string    Location     { get; set; } = string.Empty;

        public int?      EgmID        { get; set; }
        public int?      MemberID     { get; set; }
        public int?      PromotionID  { get; set; }
        public int?      TodayTypeID  { get; set; }
        public int?      DayID        { get; set; }

        public DateTime? FromDate     { get; set; }
        public DateTime? ToDate       { get; set; }
    }

    public class TodayResponse
    {
        public string  Status     { get; set; } = string.Empty;
        public string  Response   { get; set; } = string.Empty;
        public Today[] Results    { get; set; } = Array.Empty<Today>();
    }

   public class Today
    {
        public string     OpenTime       { get; set; } = string.Empty;
        public string     CloseTime      { get; set; } = string.Empty;
        public string     Description    { get; set; } = string.Empty;
        public DateTime   CurrentDate    { get; set; }
        public string     CurrentType    { get; set; } = string.Empty;
        public decimal    TurnOver       { get; set; }
        public decimal    TotalWin       { get; set; }
        public decimal    CashBox        { get; set; }
        public decimal    CanCredit      { get; set; }
        public decimal    MoneyIn        { get; set; }
        public decimal    MoneyOut       { get; set; }
        public decimal    CashIn         { get; set; }
        public decimal    CashOut        { get; set; }
        public decimal    JackPot        { get; set; }
        public int        Stroke         { get; set; }
        public int        Powerup        { get; set; }
        public decimal    Existing       { get; set; }
        public int        Interface      { get; set; }
        public decimal    Clear          { get; set; }
        public decimal    CCDocket       { get; set; }
        public decimal    ShortPay       { get; set; }
        public decimal    Refill         { get; set; }
        public decimal    LinkWin        { get; set; }
        public decimal    LinkCCCE       { get; set; }
        public int        Five           { get; set; }
        public int        Ten            { get; set; }
        public int        Twenty         { get; set; }
        public int        Fifty          { get; set; }
        public int        Hundred        { get; set; }
        public int        FiveHundred    { get; set; }
        public int        Thousand       { get; set; }
        public int        TotalBills     { get; set; }
        public decimal    TotalValue     { get; set; }
        public decimal    Online         { get; set; }
        public decimal    CardIn         { get; set; }
        public int        CardCount      { get; set; }
        public decimal    InPlay         { get; set; }
        public int        PlayCount      { get; set; }
        public int        Fault          { get; set; }
        public int        FaultID        { get; set; }
        public int        FaultCount     { get; set; }
        public int        HotPlay        { get; set; }
        public int        Service        { get; set; }
        public DateTime   ServiceStart   { get; set; }
        public int        Staff          { get; set; }
        public DateTime   StaffStart     { get; set; }
        public int        Cancel         { get; set; }
        public DateTime   CancelStart    { get; set; }
        public decimal    CancelAmount   { get; set; }
        public int        Timeout        { get; set; }
        public DateTime   TimeoutStart   { get; set; }
        public string     Member         { get; set; } = string.Empty;
        public decimal    MemberTO       { get; set; }
        public decimal    MemberPts      { get; set; }
        public string     Promotion      { get; set; } = string.Empty;
        public decimal    PointsRate     { get; set; }
        public decimal    CashlessIn     { get; set; }
        public decimal    CashlessOut    { get; set; }
        public decimal    CashlessPay    { get; set; }
        public decimal    CashlessEGM    { get; set; }
        public int        MemberID       { get; set; }
        public int        DeviceNo       { get; set; }
        public string     ReferenceNo    { get; set; } = string.Empty;
        public int        TodayPK        { get; set; }
        public int        TodayTypeId    { get; set; }
        public int        EgmID          { get; set; }
        public int        PromotionID    { get; set; }
        public int        OwnerID        { get; set; }
        public int        VenueID        { get; set; }
        public int        PeriodId       { get; set; }
        public int        MinuteId       { get; set; }
        public int        DayId          { get; set; }
        public int        WeekId         { get; set; }
        public int        MonthId        { get; set; }
        public int        QuarterId      { get; set; }
        public int        YearId         { get; set; }
        public int        ShiftId        { get; set; }
        public int        IntervalId     { get; set; }
        public DateTime   Inserted       { get; set; }
        public int        Sequence       { get; set; }

        public bool HasData() => true;
    }
}
