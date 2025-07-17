namespace nexus.api.Model.Membership
{
    public class ClubRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public Club?    Club         { get; set; } = new Club(); 
    }

    public class ClubResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public string   Response     { get; set; } = String.Empty;
        public Club[]   Clubs        { get; set; } = Array.Empty<Club>();
    }

    public class Club
    {
        public int?    ClubId       { get; set; } = -1;
        public string? Description  { get; set; } = String.Empty;
        public string? ClubType     { get; set; } = String.Empty;
        public string? ClubCode     { get; set; } = String.Empty;
        public string? Gender       { get; set; } = String.Empty;
        public string? Parent       { get; set; } = String.Empty;
        public int?    AutoRenew    { get; set; } = -1;
        public int?    Prorata      { get; set; } = -1;
        public int?    UniqueFees   { get; set; } = -1;
        public string? RollType     { get; set; } = String.Empty;
        public int?    Grace        { get; set; } = -1;
        public int?    Lapse        { get; set; } = -1;
        public int?    UnFinancial  { get; set; } = -1;
        public int?    UnFinCnt     { get; set; } = -1;
        public int?    RenewalMonth { get; set; } = -1;
        public int?    DaysBeforeExpire { get; set; } = -1;
        public string? ProcessDays  { get; set; } = String.Empty;
        public int?    CohortId     { get; set; } = -1;
        public int?    ClubTypeId   { get; set; } = -1;
        public int?    GenderId     { get; set; } = -1;
        public int?    ParentId     { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(Description)
            || !string.IsNullOrEmpty(ClubType)
            || !string.IsNullOrEmpty(ClubCode)
            || !string.IsNullOrEmpty(Gender)
            || !string.IsNullOrEmpty(RollType)
            || !string.IsNullOrEmpty(ProcessDays)

            || ClubId >= 0
            || AutoRenew >= 0
            || Prorata >= 0
            || UniqueFees >= 0
            || Grace >= 0
            || Lapse >= 0
            || UnFinancial >= 0
            || UnFinCnt >= 0
            || RenewalMonth >= 0
            || CohortId >= 0
            || DaysBeforeExpire >= 0);
        }

    }
}

