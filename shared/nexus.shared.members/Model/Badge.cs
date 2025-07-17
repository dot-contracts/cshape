using Newtonsoft.Json.Linq;
using nexus.api.Model.Membership;
using System.Reflection;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace nexus.api.Model.Local 
{

    public class BadgeRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public int?    CohortId   { get; set; } = -1;
        public Badge?  Badge      { get; set; } = new Badge();
    }

    public class BadgeResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public string   Response     { get; set; } = String.Empty;
        public Badge[]  Badges       { get; set; } = Array.Empty<Badge>();
    }

    public class Badge
    {
        public string?   BadgeNo       { get; set; } = String.Empty;
        public string?   Member        { get; set; } = String.Empty;
        public string?   BadgeState    { get; set; } = String.Empty;
        public int?      BadgeStateId  { get; set; } = -1;
        public int?      MemberId      { get; set; } = -1;
        public int?      CohortId      { get; set; } = -1;
        public string?   Modified      { get; set; } = string.Empty;


        public bool HasData()
        {
            return (!string.IsNullOrEmpty(BadgeNo)
            || !string.IsNullOrEmpty(BadgeState)
            || !string.IsNullOrEmpty(Member)

            || MemberId >= 0
            || BadgeStateId >= 0);
        }

    }

}
