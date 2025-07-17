using System.Security.Principal;

namespace nexus.shared.local
{
    public class AccountRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public Account?  Account    { get; set; } = new Account();
    }

    public class AccountResponse
    {
        public string    Status     { get; set; } = string.Empty;
        public string    Response   { get; set; } = string.Empty;
        public Account[] Accounts   { get; set; } = Array.Empty<Account>();
    }

    public class Account
    {
        public int?     AccountId      { get; set; } = -1;
        public string?  AccountType    { get; set; } = string.Empty;
        public string?  AccountState   { get; set; } = string.Empty;
        public string?  Human          { get; set; } = string.Empty;
        public decimal? GAMPoints      { get; set; } = -1;
        public decimal? GAMBonus       { get; set; } = -1;
        public decimal? NGMPoints      { get; set; } = -1;
        public decimal? NGMBonus       { get; set; } = -1;
        public decimal? PROPoints      { get; set; } = -1;
        public decimal? Points         { get; set; } = -1;
        public decimal? Credit         { get; set; } = -1;
        public decimal? Award          { get; set; } = -1;
        public decimal? Saving         { get; set; } = -1;
        public decimal? Credits        { get; set; } = -1;
        public decimal? Value1         { get; set; } = -1;
        public decimal? Value2         { get; set; } = -1;
        public decimal? Value3         { get; set; } = -1;
        public decimal? Value4         { get; set; } = -1;
        public decimal? Value5         { get; set; } = -1;
        public decimal? Value6         { get; set; } = -1;
        public int?     AccountTypeId  { get; set; } = -1; 
        public int?     AccountStateId { get; set; } = -1;
        public int?     HumanId        { get; set; } = -1;
        public int?     HasCredit      { get; set; } = -1;
        public int?     HasPoints      { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(AccountType) || !string.IsNullOrEmpty(AccountState) || AccountId >= 0 || HumanId >= 0 || GAMPoints >= 0 || NGMPoints >= 0 || PROPoints >= 0
            || Credit >= 0 || Value1 >= 0 || Value2 >= 0 || Value3 >= 0 || Value4 >= 0 || Value5 >= 0 || Value6 >= 0 || HasCredit >= 0 || HasPoints >= 0 || AccountTypeId >= 0 || AccountStateId >= 0);
        }
    }
}

