using System.Security.Principal;

namespace nexus.shared.promo
{
    public class ActionRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public Action?   Action    { get; set; } = new Action();

        public ActionRequest(int ActionType, string Vendor, string Location, Action Action = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Action    = (Action == null ? new Action() : Action);
        }
    }

    public class ActionResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public Action[] Actions   { get; set; } = Array.Empty<Action>();
    }

    public class Action
    {
        public string?  Promotion       { get; set; } = string.Empty;
        public int?     ActionId        { get; set; } = -1;
        public string?  ActionType      { get; set; } = string.Empty;
        public string?  ActionState     { get; set; } = string.Empty;
        public string?  ActionDefn      { get; set; } = string.Empty;
        public string?  Description     { get; set; } = string.Empty;
        public int?     Prize           { get; set; } = -1;
        public string?  Account         { get; set; } = string.Empty;
        public decimal? Amount          { get; set; } = -1;
        public decimal? PointsRate      { get; set; } = -1;
        public string?  DelayType       { get; set; } = string.Empty;
        public int?     Delay           { get; set; } = -1;
        public string?  ExpireType      { get; set; } = string.Empty;
        public int?     Expiry          { get; set; } = -1;
        public int?     StoryId         { get; set; } = -1;
        public string?  StoryProperty   { get; set; } = string.Empty;
        public string?  OutputType      { get; set; } = string.Empty;
        public string?  OutputTo        { get; set; } = string.Empty;
        public string?  Definition      { get; set; } = string.Empty;
        public int?     ActionTypeId    { get; set; } = -1;
        public int?     ActionStateId   { get; set; } = -1;
        public int?     DelayTypeId     { get; set; } = -1;
        public int?     ExpiryTypeId    { get; set; } = -1;
        public int?     PromotionId     { get; set; } = -1;
        public int?     Sequence        { get; set; } = -1;
        public int?     Sharing         { get; set; } = -1;
        public int?     ShareState      { get; set; } = -1;
        public int?     Operability     { get; set; } = -1;
        public int?     Logging         { get; set; } = -1;
        public int?     ScheduleId      { get; set; } = -1;
        public int?     SystemId        { get; set; } = -1;
        public int?     VenueId         { get; set; } = -1;
        public int?     ParentId        { get; set; } = -1;
        public int?     MediaId         { get; set; } = -1;
        public int?     OwnerId         { get; set; } = -1;
        public int?     LogId           { get; set; } = -1;

        public bool HasData()
        {
            return (ActionId >= 0 || !string.IsNullOrEmpty(ActionType) || !string.IsNullOrEmpty(ActionState) || !string.IsNullOrEmpty(Promotion)
            || PromotionId >= 0 || ActionTypeId >= 0 || ActionStateId >= 0 || ScheduleId >= 0 );
        }
    }
}

