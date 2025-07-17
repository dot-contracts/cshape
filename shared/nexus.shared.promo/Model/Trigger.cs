using System.Security.Principal;

namespace nexus.shared.promo
{
    public class TriggerRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public Trigger?  Trigger    { get; set; } = new Trigger();
        
        public TriggerRequest(int ActionType, string Vendor, string Location, Trigger Trigger = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Trigger    = (Trigger == null ? new Trigger() : Trigger);
        }
    }

    public class TriggerResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public Trigger[] Triggers   { get; set; } = Array.Empty<Trigger>();
    }

    public class Trigger
    {
        public int?     TriggerId        { get; set; } = -1;
        public string?  Promotion        { get; set; } = string.Empty;
        public string?  PromotionType    { get; set; } = string.Empty;
        public string?  PromotionState   { get; set; } = string.Empty;
        public string?  TriggerType      { get; set; } = string.Empty;
        public string?  TriggerState     { get; set; } = string.Empty;
        public string?  TriggerDefn      { get; set; } = string.Empty;
        public string?  Description      { get; set; } = string.Empty;
        public decimal? MinValue         { get; set; } = -1;
        public decimal? MaxValue         { get; set; } = -1;
        public int?     TriggerTypeId    { get; set; } = -1;
        public int?     TriggerStateId   { get; set; } = -1;
        public int?     PromotionId      { get; set; } = -1;
        public int?     Sequence         { get; set; } = -1;
        public int?     Sharing          { get; set; } = -1;
        public int?     ShareState       { get; set; } = -1;
        public int?     Operability      { get; set; } = -1;
        public int?     Logging          { get; set; } = -1;
        public int?     ScheduleId       { get; set; } = -1;
        public int?     SystemId         { get; set; } = -1;
        public int?     VenueId          { get; set; } = -1;
        public int?     ParentId         { get; set; } = -1;
        public int?     MediaId          { get; set; } = -1;
        public int?     OwnerId          { get; set; } = -1;
        public int?     LogId            { get; set; } = -1;

        public bool HasData()
        {
            return (TriggerId >= 0 || !string.IsNullOrEmpty(TriggerType) || !string.IsNullOrEmpty(TriggerState) || !string.IsNullOrEmpty(Promotion) || !string.IsNullOrEmpty(PromotionType) 
            || PromotionId >= 0 || TriggerTypeId >= 0 || TriggerStateId >= 0 || ScheduleId >= 0 );
        }
    }
}

