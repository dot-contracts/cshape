namespace nexus.api.Model.Membership
{
    public class PINRequest
    {
        public int?     ActionType { get; set; }
        public string?  Vendor     { get; set; }
        public string?  Location   { get; set; }
        public string?  BadgeNo    { get; set; }
        public string?  CardNo     { get; set; }
        public int?     MemberId   { get; set; }
        public string?  PinNo      { get; set; }
        public string?  NewPinNo   { get; set; }
    }


    public class PINResponse
    {
        public string   Status   { get; set; } = String.Empty;
        public string   Response { get; set; } = String.Empty;
    }

}
