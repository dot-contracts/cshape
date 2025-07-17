using System.Security.Principal;

namespace nexus.shared.promo
{
    public class PoolRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public Pool?     Pool       { get; set; } = new Pool();
        
        public PoolRequest(int ActionType, string Vendor, string Location, Pool pool = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Pool       = (pool == null ? new Pool() : pool);
        }
    }

    public class PoolResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public Pool[]    Pools      { get; set; } = Array.Empty<Pool>();
    }

    public class Pool
    {
        public int?    PoolId        { get; set; } = -1;
        public string? PoolType      { get; set; } = string.Empty;
        public string? PoolState     { get; set; } = string.Empty;
        public string? Promotion     { get; set; } = string.Empty;
        public string? PromotionType { get; set; } = string.Empty;
        public string? Worker        { get; set; } = string.Empty;
        public string? Start         { get; set; } = string.Empty;
        public string? Finish        { get; set; } = string.Empty;
        public string? Ranges        { get; set; } = string.Empty;
        public string? StartRange    { get; set; } = string.Empty;
        public string? FinishRange   { get; set; } = string.Empty;
        public string? Trigger       { get; set; } = string.Empty;
        public string? TriggerDefn   { get; set; } = string.Empty;
        public string? TriggerValue  { get; set; } = string.Empty;
        public string? Action        { get; set; } = string.Empty;
        public string? ActionDefn    { get; set; } = string.Empty;

        public int?    PromotionId   { get; set; } = -1;
        public int?    WorkerId      { get; set; } = -1;
        public int?    PoolTypeId    { get; set; } = -1;
        public int?    PoolStateId   { get; set; } = -1;
        public int?    ScheduleId    { get; set; } = -1;
        public PoolItem[]? PoolItems { get; set; } = Array.Empty<PoolItem>();

        public bool HasData()
        {
            return (PoolId >= 0 || !string.IsNullOrEmpty(PoolType) || !string.IsNullOrEmpty(PoolState) || !string.IsNullOrEmpty(Promotion) || !string.IsNullOrEmpty(PromotionType) || !string.IsNullOrEmpty(Worker)
            || !string.IsNullOrEmpty(Start) || !string.IsNullOrEmpty(Finish) || !string.IsNullOrEmpty(Ranges) || !string.IsNullOrEmpty(StartRange) || !string.IsNullOrEmpty(FinishRange) 
            || PromotionId >= 0 || WorkerId >= 0 || PoolTypeId >= 0 || PoolStateId >= 0 || ScheduleId >= 0 );
        }
    }
}

