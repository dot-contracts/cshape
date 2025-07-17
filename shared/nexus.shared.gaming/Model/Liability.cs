using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class LiabilityRequest
    {
        public int    ActionType { get; set; } = 0;
        public string Vendor     { get; set; } = string.Empty;
        public string Location   { get; set; } = string.Empty;
        public string OpenDate   { get; set; } = string.Empty;
        public string CloseDate  { get; set; } = string.Empty;
        public string Search     { get; set; } = string.Empty;
        public string Value      { get; set; } = string.Empty;
    }

    public class LiabilityResponse
    {
        public string   Status    { get; set; } = string.Empty;
        public string   Response  { get; set; } = string.Empty;
        public string?  CloseDate { get; set; } = String.Empty;
        public decimal? Opening   { get; set; } = 0;
        public decimal? Closing   { get; set; } = 0;
        public decimal? Movement  { get; set; } = 0;

        public bool HasData()
        {
            return (true);
        }
    }
}

