using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class PayoutRequest
    {
        public int    ActionType { get; set; } = 0;
        public string Vendor     { get; set; } = string.Empty;
        public string Location   { get; set; } = string.Empty;
        public string OpenDate   { get; set; } = string.Empty;
        public string CloseDate  { get; set; } = string.Empty;
        public string Search     { get; set; } = string.Empty;
        public string Value      { get; set; } = string.Empty;
    }

    public class PayoutResponse
    {
        public string   Status    { get; set; } = string.Empty;
        public string   Response  { get; set; } = string.Empty;
        public Payout[] Results   { get; set; } = Array.Empty<Payout>();
    }

    public class Payout
    {
        public string?  Description { get; set; } = String.Empty;
        public decimal? Value       { get; set; } = 0;

        public bool HasData()
        {
            return (true);
        }
    }
}

