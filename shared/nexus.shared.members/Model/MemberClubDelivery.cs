using nexus.api.Model.Local;

namespace nexus.api.Model.Membership
{
    public class MemberClubDeliveryRequest
    {
        public int         ActionType { get; set; } = -1;
        public string      Vendor     { get; set; } = string.Empty;   
        public string      Location   { get; set; } = string.Empty;      
        public MemberClubDelivery? MemberClubDelivery { get; set; } = new MemberClubDelivery();

        public MemberClubDeliveryRequest() { }

        public MemberClubDeliveryRequest(int ActionType, string Vendor, string Location, MemberClubDelivery mcd = null, int MemberId = -1)
        {
            this.ActionType = ActionType;
            this.Vendor = Vendor;
            this.Location = Location;
            this.MemberClubDelivery = (mcd == null ? new MemberClubDelivery() : mcd);
            this.MemberClubDelivery.MemberId = (MemberId > 0 ? MemberId : -1);
        }
    }

    public class MemberClubDeliveryResponse
    {
        public string       Status              { get; set; } = String.Empty;
        public string       Response            { get; set; } = String.Empty;
        public MemberClubDelivery[] MemberClubDeliveries  { get; set; } = Array.Empty<MemberClubDelivery>();

        public MemberClubDeliveryResponse()
        {
            MemberClubDeliveries = new MemberClubDelivery[1];
            MemberClubDeliveries[0] = new MemberClubDelivery();
        }

    }

    public class MemberClubDelivery
    {
        public int?      MemberClubDeliveryId     { get; set; } = -1;
        public int?      MemberId         { get; set; } = -1;
        public int?      ClubId           { get; set; } = -1;
        public string?   DeliveryType     { get; set; } = String.Empty;
        public int?      EDM              { get; set; } = -1;
        public int?      SMS              { get; set; } = -1;
        public int?      POST             { get; set; } = -1;
        public int?      DeliveryTypeId   { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(DeliveryType)

            || MemberId >= 0
            || ClubId >= 0
            || EDM >= 0
            || SMS >= 0
            || POST >= 0
            || DeliveryTypeId >= 0);
        }
    }
}

