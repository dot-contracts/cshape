using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class LookUpRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public string    Search     { get; set; } = string.Empty;
        public string    Value      { get; set; } = string.Empty;
    }

    public class LookUpResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public LookUp[] Results  { get; set; } = Array.Empty<LookUp>();
    }

    public class LookUp
    {
        public string?   Description    { get; set; } = String.Empty;
        public string?   Id             { get; set; } = String.Empty;

        public bool HasData()
        {
            return ( true );
        }
    }
}

