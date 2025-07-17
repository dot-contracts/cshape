using System.Security.Cryptography;

using nexus.common;

namespace nexus.shared.local
{
    public class PatronRequest
    {
        public int           ActionType  { get; set; } = 0;
        public string        Vendor      { get; set; } = string.Empty;
        public string        Location    { get; set; } = string.Empty;
        public string?       PatronType  { get; set; } = string.Empty;
        public Patron?       Patron      { get; set; } = new Patron();

        public PatronRequest(int ActionType, string Vendor, string Location, Patron patron = null)
        {
            this.ActionType = ActionType;
            this.Vendor = Vendor;
            this.Location = Location;
            this.Patron = (patron  == null ? new Patron() : patron);
        }

    }

    public class PatronResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Patron[] Patrons { get; set; } = Array.Empty<Patron>();
    }

    public class Patron
    {
        public int?      MemberId       { get; set; } = -1;
        public string?   MemberType     { get; set; } = string.Empty;
        public string?   MemberState    { get; set; } = string.Empty;
        public string?   BadgeNo        { get; set; } = string.Empty;
        public string?   CardNo         { get; set; } = string.Empty;
        public string?   Gender         { get; set; } = string.Empty;
        public string?   Title          { get; set; } = string.Empty;
        public string?   FirstName      { get; set; } = string.Empty;
        public string?   OtherName      { get; set; } = string.Empty;
        public string?   LastName       { get; set; } = string.Empty;
        public string?   NickName       { get; set; } = string.Empty;
        public int?      UseNickName    { get; set; } = -1;
        public string?   BirthDate      { get; set; } = string.Empty;
        public int?      NextOfKin      { get; set; } = -1;
        public string?   MaritalState   { get; set; } = string.Empty;
        public int?      PatronTypeId   { get; set; } = -1;
        public int?      PatronStateId  { get; set; } = -1;
        public int?      MemberTypeId   { get; set; } = -1;
        public int?      MemberStateId  { get; set; } = -1;
        public int?      GenderId       { get; set; } = -1;
        public int?      TitleId        { get; set; } = -1;
        public int?      MaritalStateId { get; set; } = -1;
        public string?   Occupation     { get; set; } = String.Empty;
        public string?   OccupationProp { get; set; } = String.Empty;
        public int?      AddressId      { get; set; } = 0;
        public int?      PhoneId        { get; set; } = 0;
        public int?      EMailId        { get; set; } = 0;
        public int?      NoticeId       { get; set; } = 0;
        public int?      MessageId      { get; set; } = 0;
        public int?      CredentialId   { get; set; } = 0;
        public int?      PromotionId    { get; set; } = 0;
        public double?   PointsRate     { get; set; } = 0;
        public string?   PromoExpiry    { get; set; } = string.Empty;
        public int?      ImportId       { get; set; } = 0;

        public bool HasData()
        {
            return (MemberId > 0
            || !string.IsNullOrEmpty(MemberType)
            || !string.IsNullOrEmpty(MemberState)
            || !string.IsNullOrEmpty(BadgeNo)
            || !string.IsNullOrEmpty(CardNo)
            || !string.IsNullOrEmpty(Gender)
            || !string.IsNullOrEmpty(Title)
            || !string.IsNullOrEmpty(FirstName)
            || !string.IsNullOrEmpty(OtherName)
            || !string.IsNullOrEmpty(LastName)
            || !string.IsNullOrEmpty(NickName)
            || !string.IsNullOrEmpty(MaritalState)
            || !string.IsNullOrEmpty(Occupation)
            || !string.IsNullOrEmpty(OccupationProp)
            || helpers.IsDate(BirthDate)
            || UseNickName >= 0
            || NextOfKin >= 0
            || PatronTypeId >= 0
            || PatronStateId >= 0
            || MemberTypeId >= 0
            || GenderId >= 0
            || TitleId >= 0
            || MaritalStateId >= 0
            || NoticeId >= 0
            || MessageId >= 0
            || CredentialId >= 0
            || PromotionId >= 0
            || PointsRate >= 0
            || helpers.IsDate(PromoExpiry))
            || ImportId >= 0;
        }

        public bool ValidSearch()
        {
            return
            (MemberId > 0) || !string.IsNullOrEmpty(BadgeNo) || !string.IsNullOrEmpty(CardNo) ||
            (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && helpers.IsDate(BirthDate) && (!string.IsNullOrEmpty(Gender) || GenderId >= 0));
        }
    }
}

