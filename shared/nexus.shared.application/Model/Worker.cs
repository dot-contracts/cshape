using System.Security.Cryptography;

using nexus.common;

namespace nexus.shared.application
{
    public class WorkerRequest
    {
        public int           ActionType  { get; set; } = 0;
        public string        Vendor      { get; set; } = string.Empty;
        public string        Location    { get; set; } = string.Empty;
        public string?       WorkerType  { get; set; } = string.Empty;
        public Worker?       Worker      { get; set; } = new Worker();

        public WorkerRequest(int ActionType, string Vendor, string Location, Worker Worker = null)
        {
            this.ActionType = ActionType;
            this.Vendor = Vendor;
            this.Location = Location;
            this.Worker = (Worker  == null ? new Worker() : Worker);
        }

    }

    public class WorkerResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Worker[] Workers  { get; set; } = Array.Empty<Worker>();
    }

    public class Worker
    {
        public int?      WorkerId       { get; set; } = -1;
        public string?   WorkerType     { get; set; } = string.Empty;
        public string?   WorkerState    { get; set; } = string.Empty;
        public string?   UserName       { get; set; } = string.Empty;
        public string?   Password       { get; set; } = string.Empty;
        public string?   CardNo         { get; set; } = string.Empty;
        public string?   PinNo          { get; set; } = string.Empty;
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
        public int?      WorkerTypeId   { get; set; } = -1;
        public int?      WorkerStateId  { get; set; } = -1;
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
            return (WorkerId > 0
            || !string.IsNullOrEmpty(WorkerType)
            || !string.IsNullOrEmpty(WorkerState)
            || !string.IsNullOrEmpty(Gender)
            || !string.IsNullOrEmpty(Title)
            || !string.IsNullOrEmpty(FirstName)
            || !string.IsNullOrEmpty(OtherName)
            || !string.IsNullOrEmpty(LastName)
            || !string.IsNullOrEmpty(NickName)
            || !string.IsNullOrEmpty(MaritalState)
            || !string.IsNullOrEmpty(Occupation)
            || !string.IsNullOrEmpty(OccupationProp)
            || !string.IsNullOrEmpty(UserName)
            || !string.IsNullOrEmpty(Password)
            || !string.IsNullOrEmpty(CardNo)
            || !string.IsNullOrEmpty(PinNo)


            || helpers.IsDate(BirthDate)
            || UseNickName >= 0
            || NextOfKin >= 0
            || WorkerTypeId >= 0
            || WorkerStateId >= 0
            || WorkerTypeId >= 0
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
            (WorkerId > 0) || WorkerId == 0 || !string.IsNullOrEmpty(CardNo) ||
            (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && helpers.IsDate(BirthDate) && (!string.IsNullOrEmpty(Gender) || GenderId >= 0));
        }
    }
}

