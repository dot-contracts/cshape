using System.Security.Principal;

namespace nexus.shared.promo
{
    public class TransactionRequest
    {
        public int           ActionType  { get; set; } = 0;
        public string        Vendor      { get; set; } = string.Empty;
        public string        Location    { get; set; } = string.Empty;
        public string        PromotionId { get; set; } = string.Empty;
        public Transaction?  Transaction { get; set; } = new Transaction();
    }

    public class TransactionResponse
    {
        public string        Status        { get; set; } = string.Empty;
        public string        Response      { get; set; } = string.Empty;
        public Transaction[] Transactions  { get; set; } = Array.Empty<Transaction>();
    }

    public class Transaction
    {
        public int?     TransactionId      { get; set; } = -1;
        public string?  TransactionState   { get; set; } = string.Empty;
        public string?  Promotion          { get; set; } = string.Empty;
        public string?  PromotionType      { get; set; } = string.Empty;
        public string?  Trigger            { get; set; } = string.Empty;
        public string?  Action             { get; set; } = string.Empty;
        public string?  Device             { get; set; } = string.Empty;
        public string?  Worker             { get; set; } = string.Empty;
        public string?  Member             { get; set; } = string.Empty;
        public decimal? TriggerValue       { get; set; } = -1;
        public int?     TriggerCount       { get; set; } = -1;
        public decimal? MinValue           { get; set; } = -1;
        public decimal? MaxValue           { get; set; } = -1;
        public string?  TriggerDefn        { get; set; } = string.Empty;
        public string?  AdjustmentType     { get; set; } = string.Empty;
        public decimal? Adjustment         { get; set; } = -1;
        public string?  Expires            { get; set; } = string.Empty;
        public string?  Taken              { get; set; } = string.Empty;
        public string?  Starts             { get; set; } = string.Empty;
        public string?  Finishes           { get; set; } = string.Empty;
        public string?  ActionDefn         { get; set; } = string.Empty;
        public int?     Prize              { get; set; } = -1;
        public string?  OutputTo           { get; set; } = string.Empty;
        public string?  Account            { get; set; } = string.Empty;
        public decimal? Amount             { get; set; } = -1;
        public decimal? PointsRate         { get; set; } = -1;
        public string?  ExpiryType         { get; set; } = string.Empty;
        public int?     Expiry             { get; set; } = -1;
        public string?  StoryProperty      { get; set; } = string.Empty;
        public int?     StoryId            { get; set; } = -1;
        public int?     ActionTypeId       { get; set; } = -1; 
        public int?     ActionStateId      { get; set; } = -1; 
        public int?     ExpiryTypeId       { get; set; } = -1;
        public int?     TransactionStateId { get; set; } = -1;
        public int?     AdjustmentTypeId   { get; set; } = -1;
        public int?     TriggerId          { get; set; } = -1;
        public int?     ActionId           { get; set; } = -1;
        public int?     PromotionId        { get; set; } = -1;
        public int?     ParentId           { get; set; } = -1;
        public int?     DeviceId           { get; set; } = -1;
        public int?     WorkerId           { get; set; } = -1;
        public int?     MemberId           { get; set; } = -1;
        public string?  Inserted           { get; set; } = string.Empty;
        public string?  Modified           { get; set; } = string.Empty;
        public int?     MinuteId           { get; set; } = -1;
        public int?     DayId              { get; set; } = -1;
        public int?     WeekId             { get; set; } = -1;
        public int?     MonthId            { get; set; } = -1;
        public int?     QuarterId          { get; set; } = -1;
        public int?     YearId             { get; set; } = -1;
        public int?     ShiftId            { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(TransactionState) || !string.IsNullOrEmpty(Promotion) || !string.IsNullOrEmpty(PromotionType) || !string.IsNullOrEmpty(Trigger) || !string.IsNullOrEmpty(Action) || !string.IsNullOrEmpty(Device) || !string.IsNullOrEmpty(Worker) || !string.IsNullOrEmpty(Member) ||
                    !string.IsNullOrEmpty(TriggerDefn) || !string.IsNullOrEmpty(AdjustmentType) || !string.IsNullOrEmpty(Expires) || !string.IsNullOrEmpty(Taken) || !string.IsNullOrEmpty(Starts) || !string.IsNullOrEmpty(Finishes) || !string.IsNullOrEmpty(ActionDefn) || !string.IsNullOrEmpty(OutputTo) || !string.IsNullOrEmpty(Account) || !string.IsNullOrEmpty(ExpiryType) || !string.IsNullOrEmpty(StoryProperty) ||
                    !string.IsNullOrEmpty(Inserted) || !string.IsNullOrEmpty(Modified)  ||
                    TransactionId >= 0 || TriggerValue >= 0 || TriggerCount >= 0 || MinValue >= 0 || MaxValue >= 0 || Adjustment >= 0 || Prize >= 0 || Amount >= 0 || PointsRate >= 0 || Expiry >= 0 || 
                    StoryId >= 0 || ActionTypeId >= 0 || ActionStateId >= 0 || ExpiryTypeId >= 0 || TransactionStateId >= 0 || AdjustmentTypeId >= 0 || TriggerId >= 0 || ActionId >= 0 || PromotionId >= 0 || ParentId >= 0 || DeviceId >= 0 || WorkerId >= 0 || MemberId >= 0 || MinuteId >= 0 || DayId >= 0 || WeekId >= 0 || MonthId >= 0 || QuarterId >= 0 || YearId >= 0 || ShiftId >= 0);
        }
    }
}

