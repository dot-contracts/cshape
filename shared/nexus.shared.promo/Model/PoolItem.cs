using System.Security.Principal;

namespace nexus.shared.promo
{
    public class PoolItemRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public PoolItem? PoolItem   { get; set; } = new PoolItem();
    }

    public class PoolItemResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public PoolItem[] PoolItems { get; set; } = Array.Empty<PoolItem>();
    }

    public class PoolItem
    {
        public int?    PoolItemId    { get; set; } = -1;
        public string? PoolItemState { get; set; } = string.Empty;
        public string? Entry         { get; set; } = string.Empty;
        public string? Member        { get; set; } = string.Empty;
        public string? BadgeNo       { get; set; } = string.Empty;
        public string? CardNo        { get; set; } = string.Empty;
        public string? DrawDate      { get; set; } = string.Empty;
        public string? EGM           { get; set; } = string.Empty;
        public int?    Sequence      { get; set; } = -1;
        public int?    Entries       { get; set; } = -1;
        public int?    PoolId        { get; set; } = -1;
        public int?    PromotionId   { get; set; } = -1;
        public int?    TransactionId { get; set; } = -1;
        public int?    MemberId      { get; set; } = -1;
        public int?    EgmId         { get; set; } = -1;
        public int?    PoolItemStateId { get; set; } = -1; 

        public bool HasData()
        {
            return (PoolId >= 0 || !string.IsNullOrEmpty(DrawDate) || !string.IsNullOrEmpty(Entry) || !string.IsNullOrEmpty(BadgeNo) || !string.IsNullOrEmpty(CardNo) 
            || Sequence >= 0 || PoolItemId >= 0 || PoolId >= 0 || PromotionId >= 0 || TransactionId >= 0 || Sequence >= 0 || MemberId >= 0 || PoolItemStateId >= 0);
        }
    }
}

