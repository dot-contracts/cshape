namespace nexus.api.Model.Membership
{ 
    public class FeeRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public Fee?     Fee          { get; set; } = new Fee(); 
    }

    public class FeeResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public int      ResponseCode { get; set; } = 0;
        public string   Response     { get; set; } = String.Empty;
        public Fee[]    Fees         { get; set; } = Array.Empty<Fee>();
    }

    public class Fee
    {
        public int?     FeeId        { get; set; } = -1;
        public int?     ClubId       { get; set; } = -1;
        public string?  Description  { get; set; } = String.Empty;
        public string?  FeeType      { get; set; } = String.Empty;
        public string?  FeeCode      { get; set; } = String.Empty;
        public string?  MemberType   { get; set; } = String.Empty;
        public string?  Club         { get; set; } = String.Empty;
        public decimal? Value        { get; set; } = -1;
        public decimal? MinValue     { get; set; } = -1;
        public int?     GSTInc       { get; set; } = -1;
        public string?  DurationType { get; set; } = String.Empty;
        public int?     Duration     { get; set; } = 0;
        public string?  SPName       { get; set; } = String.Empty;
        public string?  Property     { get; set; } = String.Empty;
        public int?     FeeTypeId    { get; set; } = -1;
        public int?     FeeCodId     { get; set; } = -1;
        public int?     MemberTypeId { get; set; } = -1;


        public bool HasData()
        {
            return (!string.IsNullOrEmpty(Description)
            || !string.IsNullOrEmpty(FeeType)
            || !string.IsNullOrEmpty(FeeCode)
            || !string.IsNullOrEmpty(MemberType)
            || !string.IsNullOrEmpty(DurationType)
            || !string.IsNullOrEmpty(SPName)
            || !string.IsNullOrEmpty(Property)

            || FeeId >= 0
            || ClubId >= 0
            || Value >= 0
            || MinValue >= 0
            || GSTInc >= 0
            || Duration >= 0);
        }

    }
}

