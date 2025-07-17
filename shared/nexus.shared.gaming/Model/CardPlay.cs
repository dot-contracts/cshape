using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class CardPlayRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public string    CardNo     { get; set; } = string.Empty;
        public string    BadgeNo    { get; set; } = string.Empty;
        public string    HouseNo    { get; set; } = string.Empty;
        public string    OpenDate   { get; set; } = string.Empty;
        public string    CloseDate  { get; set; } = string.Empty;
    }

    public class CardPlayResponse
    {
        public string     Status    { get; set; } = string.Empty;
        public string     Response  { get; set; } = string.Empty;
        public CardPlay[] Results   { get; set; } = Array.Empty<CardPlay>();
    }

    public class CardPlay
    {
        public string?   BadgeNo        { get; set; } = String.Empty;
        public string?   CardNo         { get; set; } = String.Empty;
        public string?   FirstName      { get; set; } = String.Empty;
        public string?   LastName       { get; set; } = String.Empty;
        public string?   FullName       { get; set; } = String.Empty;
        public string?   BirthDate      { get; set; } = string.Empty;
        public string?   MemberType     { get; set; } = string.Empty;
        public string?   MemberState    { get; set; } = string.Empty;
        public string?   FinancialState { get; set; } = string.Empty;
        public string?   Promotion      { get; set; } = string.Empty;
        public string?   TierLevel      { get; set; } = string.Empty;
        public double?   PointsRate     { get; set; } = 0; 
        public string?   LastPlay       { get; set; } = string.Empty;
        public double?   Points         { get; set; } = 0; 
        public double?   Credit         { get; set; } = 0; 
        public string?   HouseNo        { get; set; } = String.Empty;
        public string?   GameName       { get; set; } = String.Empty;
        public string?   OpenDate       { get; set; } = String.Empty;
        public double?   Turnover       { get; set; } = 0; 
        public double?   TotalWin       { get; set; } = 0;
        public double?   Stroke         { get; set; } = 0;
        public string?   Duration       { get; set; } = String.Empty;
        public double?   AvgBet         { get; set; } = 0; 
        public double?   PlayerTO       { get; set; } = 0; 
        public double?   PlayerTW       { get; set; } = 0; 
        public double?   PlayerStroke   { get; set; } = 0; 
        public double?   PlayerAvgBet   { get; set; } = 0; 
        public string?   PlayerDur      { get; set; } = String.Empty;
        public double?   TodayTO        { get; set; } = 0; 
        public double?   TodayTW        { get; set; } = 0; 
        public double?   TodayStroke    { get; set; } = 0; 
        public double?   TodayAvgBet    { get; set; } = 0; 
        public string?   TodayDur       { get; set; } = String.Empty;

        public bool HasData()
        {
            return ( Credit >= 0  );
        }
    }
}

