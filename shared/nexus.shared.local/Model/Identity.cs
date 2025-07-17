

//using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;

namespace nexus.shared.local
{
    public class IdentityRequest
    {
        public int      ActionType { get; set; } = 0;
        public string   Vendor     { get; set; } = string.Empty;   
        public string   Location   { get; set; } = string.Empty;      
        public string?  BadgeNo    { get; set; }
        public string?  CardNo     { get; set; }
        public Identity Identity   { get; set; } = new Identity();

        public IdentityRequest(int ActionType, string Vendor, string Location, Identity identity = null, int EntityId = -1)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Identity   = (identity == null ? new Identity() : identity);
            this.Identity.EntityId  = (EntityId >0  ? EntityId  : - 1 );
        }

    }

    public class IdentityResponse
    {
        public Identity[] Identities { get; set; } = Array.Empty<Identity>();
        public string   Status       { get; set; } = String.Empty;
        public int      ResponseCode { get; set; } = 0;
        public string   Response     { get; set; } = String.Empty;

        public IdentityResponse()
        {
            Identities    = new Identity[1];
            Identities[0] = new Identity();
        }

    }

    public class Identity
    {
        public int?      IdentityId         { get; set; } = -1;
        public int?      EntityId           { get; set; } = -1;
        public string?   IdentityType       { get; set; } = String.Empty;
        public string?   IdentityState      { get; set; } = String.Empty;
        public string?   IdentityLocation   { get; set; } = String.Empty;
        public string?   Value              { get; set; } = String.Empty;
        public string?   Property           { get; set; } = String.Empty;
        public string?   Expires            { get; set; } = String.Empty;
        public int?      IdentityTypeId     { get; set; } = -1;
        public int?      IdentityStateId    { get; set; } = -1;
        public int?      IdentityLocationId { get; set; } = -1;
        public int?      ImageId            { get; set; } = -1;
        public int?      NoteId             { get; set; } = -1;

        public bool HasData()
        {
            return (IdentityId >= 0
            || EntityId >= 0
            || !string.IsNullOrEmpty(IdentityType)
            || !string.IsNullOrEmpty(IdentityState)
            || !string.IsNullOrEmpty(IdentityLocation)
            || !string.IsNullOrEmpty(Value)
            || !string.IsNullOrEmpty(Property)
            || !string.IsNullOrEmpty(Expires)
            || IdentityTypeId > 0
            || IdentityStateId > 0
            || IdentityLocationId > 0
            || ImageId > 0
            || NoteId  > 0);
        }

    }
}

