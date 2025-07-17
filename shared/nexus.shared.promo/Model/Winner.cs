using System.Security.Principal;

namespace nexus.shared.promo
{
    public class WinnerRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public Winner?   Winner     { get; set; } = new Winner();
        
        public WinnerRequest(int ActionType, string Vendor, string Location, Winner Winner = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Winner    = (Winner == null ? new Winner() : Winner);
        }
    }

    public class WinnerResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public Winner[] Winners   { get; set; } = Array.Empty<Winner>();
    }

    public class Winner
    {
        public int?     WinnerId        { get; set; } = -1;
        public string?  WinDate         { get; set; } = string.Empty;
        public string?  WinState        { get; set; } = string.Empty;
        public string?  Entry           { get; set; } = string.Empty;
        public string?  DrawDate        { get; set; } = string.Empty;
        public int?     Sequence        { get; set; } = -1;
        public string?  Member          { get; set; } = string.Empty;
        public string?  BadgeNo         { get; set; } = string.Empty;
        public string?  EGM             { get; set; } = string.Empty;
        public int?     PoolId          { get; set; } = -1;
        public int?     PoolItemId      { get; set; } = -1;
        public int?     PromotionId     { get; set; } = -1;
        public int?     TransactionId   { get; set; } = -1;
        public int?     MemberId        { get; set; } = -1;
        public int?     LocationId      { get; set; } = -1;
        public int?     WinStateId      { get; set; } = -1;

        public bool HasData()
        {
            return (WinnerId >= 0 || !string.IsNullOrEmpty(WinState) || PromotionId >= 0  );
        }
    }
}

