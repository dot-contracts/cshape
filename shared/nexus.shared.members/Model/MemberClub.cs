using nexus.api.Model.Local;

namespace nexus.api.Model.Membership
{
    public class MemberClubRequest
    {
        public int         ActionType { get; set; } = -1;
        public string      Vendor     { get; set; } = string.Empty;   
        public string      Location   { get; set; } = string.Empty;      
        public MemberClub? MemberClub { get; set; } = new MemberClub();

        public MemberClubRequest()
        {
        }
        public MemberClubRequest(int ActionType, string Vendor, string Location, MemberClub memClub = null, int MemberId = -1, int ClubId = -1)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.MemberClub = (memClub == null ? new MemberClub() : memClub);
            this.MemberClub.MemberId = (MemberId > 0 ? MemberId  : -1);
            this.MemberClub.ClubId   = (ClubId   > 0 ? ClubId    : -1);
        }
    }

    public class MemberClubResponse
    {
        public string       Status              { get; set; } = String.Empty;
        public string       Response            { get; set; } = String.Empty;
        public MemberClub[] MemberClubs         { get; set; } = Array.Empty<MemberClub>();

        public MemberClubResponse()
        {
            MemberClubs = new MemberClub[1];
            MemberClubs[0] = new MemberClub();
        }

    }

    public class MemberClub
    {
        public int?     MemberClubId     { get; set; } = -1;
        public string?  Member           { get; set; } = String.Empty;
        public string?  Club             { get; set; } = String.Empty;
        public string?  BadgeNo          { get; set; } = String.Empty;
        public string?  MemberType       { get; set; } = String.Empty;
        public string?  MemberState      { get; set; } = String.Empty;
        public string?  FinancialState   { get; set; } = String.Empty;
        public string?  FinancialTo      { get; set; } = String.Empty;
        public bool?    PrimaryClub      { get; set; } = false;
        public string?  Joined           { get; set; } = String.Empty;
        public string?  Approved         { get; set; } = String.Empty;
        public string?  LastPayment      { get; set; } = String.Empty;
        public int?     MemberId         { get; set; } = -1;
        public int?     ClubId           { get; set; } = -1;
        public int?     FeeId            { get; set; } = -1;
        public int?     PromotionId      { get; set; } = -1;
        public int?     BatchId          { get; set; } = -1;
        public int?     Voting           { get; set; } = -1;
        public int?     RenewMode        { get; set; } = -1;
        public int?     MemberTypeId     { get; set; } = -1;
        public int?     MemberStateId    { get; set; } = -1;
        public int?     FinancialStateId { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(Member)
            || !string.IsNullOrEmpty(Club)
            || !string.IsNullOrEmpty(MemberType)
            || !string.IsNullOrEmpty(MemberState)
            || !string.IsNullOrEmpty(FinancialState)
            || !string.IsNullOrEmpty(FinancialTo)
            || !string.IsNullOrEmpty(Joined)
            || !string.IsNullOrEmpty(Approved)
            || !string.IsNullOrEmpty(LastPayment)

            || MemberId >= 0
            || ClubId >= 0
            || FeeId >= 0
            || PromotionId >= 0
            || BatchId >= 0
            || Voting >= 0
            || RenewMode >= 0
            || MemberTypeId >= 0
            || MemberStateId >= 0
            || FinancialStateId >= 0);
        }
    }
}

