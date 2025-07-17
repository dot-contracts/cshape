

namespace nexus.shared.local
{
    public class AccountUpdateRequest
    {
        public int?     ActionType { get; set; }
        public string?  Vendor     { get; set; }
        public string?  Location   { get; set; }
        public string?  BadgeNo    { get; set; }
        public string?  CardNo     { get; set; }
        public string?  PinNo      { get; set; }
        public string?  Account    { get; set; }
        public decimal? Amount     { get; set; }
    }

    public class AccountUpdateResponse
    {
        public string?   Status       { get; set; } = String.Empty;
        public string?   Response     { get; set; } = String.Empty;
        public string?   BadgeNo      { get; set; } = String.Empty;
        public string?   CardNo       { get; set; } = String.Empty;
        public string?   FirstName    { get; set; } = String.Empty;
        public string?   LastName     { get; set; } = String.Empty;
        public string?   CardType     { get; set; } = String.Empty;
        public string?   BirthDate    { get; set; } = String.Empty;
        public string?   FinancialTo  { get; set; } = String.Empty;
        public decimal?  GAMPoints    { get; set; } = decimal.MinValue;
        public decimal?  GAMBonus     { get; set; } = decimal.MinValue;
        public decimal?  NGMPoints    { get; set; } = decimal.MinValue;
        public decimal?  NGMBonus     { get; set; } = decimal.MinValue;
        public decimal?  PROPoints    { get; set; } = decimal.MinValue;
        public decimal?  Points       { get; set; } = decimal.MinValue;
        public decimal?  Credit       { get; set; } = decimal.MinValue;
        public decimal? Award        { get; set; } = decimal.MinValue;
        public decimal?  Saving       { get; set; } = decimal.MinValue;
        public decimal?  Credits      { get; set; } = decimal.MinValue;
        public string?   Promotion    { get; set; } = String.Empty;
        public decimal?  PointsRate   { get; set; } = decimal.MinValue;
        public int?      TierLevel    { get; set; } = 0;
    }

}
