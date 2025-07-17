
using System.Data;
using System.Diagnostics.Tracing;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiAccount : ControllerBase
    {
        [HttpPost("Account")]
        [Authorize]
        public IActionResult Post([FromBody] AccountRequest request)
        {

            // var currentUser = HttpContext.User;

            AccountResponse acctRes = new AccountResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                        if (request.Account != null)
                        {
                            Account account = request.Account;

                            bool isValid = false;
                            if (request.ActionType.Equals(1)) isValid = account.HumanId > 0;
                            else if (account.HumanId > 0 || account.AccountId > 0) isValid = true;

                            //if (!isValid)
                            //{
                            //    acctRes.Status   = "Error";
                            //    acctRes.Response = "No HumanId or AccountId";
                           // }
                            //else 
                            { 
                                if (account.HasData())
                                {
                                    diag = "1";
                                    if (!string.IsNullOrEmpty(account.AccountType))   Params += ";I~S~AccountType~" +  account.AccountType;
                                    if (!string.IsNullOrEmpty(account.AccountState))  Params += ";I~S~AccountState~" + account.AccountState;

                                    if (account.AccountId > 0)   Params += ";I~S~AccountId~" +  account.AccountId.ToString();
                                    if (account.HumanId > 0)     Params += ";I~S~HumanId~" +    account.HumanId.ToString();
                                    if (account.GAMPoints > 0)   Params += ";I~S~GAMPoints~" +  account.GAMPoints.ToString();
                                    if (account.GAMBonus > 0)    Params += ";I~S~GAMBonus~" +   account.GAMBonus.ToString();
                                    if (account.NGMPoints > 0)   Params += ";I~S~NGMPoints~" +  account.NGMPoints.ToString();
                                    if (account.NGMBonus > 0)    Params += ";I~S~NGMBonus~" +   account.NGMBonus.ToString();
                                    if (account.PROPoints > 0)   Params += ";I~S~PROPoints~" +  account.PROPoints.ToString();
                                    diag = "2";

                                    if (account.Credit > 0)      Params += ";I~S~Credit~" +     account.Credit.ToString();
                                    if (account.Award > 0)       Params += ";I~S~Award~" +      account.Award.ToString();
                                    if (account.Saving> 0)       Params += ";I~S~Saving~" +     account.Saving.ToString();

                                    if (account.Value1 > 0)      Params += ";I~S~Value1~" +     account.Value1.ToString();
                                    if (account.Value2 > 0)      Params += ";I~S~Value2~" +     account.Value2.ToString();
                                    if (account.Value3 > 0)      Params += ";I~S~Value3~" +     account.Value3.ToString();
                                    if (account.Value4 > 0)      Params += ";I~S~Value4~" +     account.Value4.ToString();
                                    if (account.Value5 > 0)      Params += ";I~S~Value5~" +     account.Value5.ToString();
                                    if (account.Value6 > 0)      Params += ";I~S~Value6~" +     account.Value6.ToString();
                                    if (account.HasCredit > 0)   Params += ";I~S~HasCredit~" +  account.HasCredit.ToString();
                                    if (account.HasPoints > 0)   Params += ";I~S~HasPoints~" +  account.HasPoints.ToString();
                                    if (account.AccountTypeId > 0)  Params += ";I~S~AccountTypeId~" +  account.AccountTypeId.ToString();
                                    if (account.AccountStateId > 0) Params += ";I~S~AccountStateId~" + account.AccountStateId.ToString();
                                    diag = "4";
                                }

                                diag = "5";
                                helpers.WriteToLog("apiAccount:" + Params);
                                DataTable dt = db.GetDataTable("loc.api_Account", Params);
                                if (dt.Rows.Count > 0)
                                {
                                    diag = "6";
                                    acctRes.Status   = dt.Rows[0]["Status"].ToString();
                                    acctRes.Response = dt.Rows[0]["Response"].ToString();

                                    Account[] Accounts = new Account[dt.Rows.Count];
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        diag = "7" + i.ToString();
                                        Account Account = new Account()
                                        {
                                            AccountId        = helpers.ToInt(dt.Rows[i]["AccountPk"].ToString()),
                                            AccountType      =               dt.Rows[i]["AccountType"].ToString(),
                                            AccountState     =               dt.Rows[i]["AccountState"].ToString(),
                                            Human            =               dt.Rows[i]["Human"].ToString(),

                                            GAMPoints        = helpers.ToDecimal(dt.Rows[i]["GAMPoints"].ToString()),
                                            GAMBonus         = helpers.ToDecimal(dt.Rows[i]["GAMBonus"].ToString()),
                                            NGMPoints        = helpers.ToDecimal(dt.Rows[i]["NGMPoints"].ToString()),
                                            NGMBonus         = helpers.ToDecimal(dt.Rows[i]["NGMBonus"].ToString()),
                                            PROPoints        = helpers.ToDecimal(dt.Rows[i]["PROPoints"].ToString()),
                                            Points           = helpers.ToDecimal(dt.Rows[i]["Points"].ToString()),
                                            Credit           = helpers.ToDecimal(dt.Rows[i]["Credit"].ToString()),
                                            Saving           = helpers.ToDecimal(dt.Rows[i]["Saving"].ToString()),
                                            Award            = helpers.ToDecimal(dt.Rows[i]["Award"].ToString()),
                                            Credits          = helpers.ToDecimal(dt.Rows[i]["Credits"].ToString()),
                                            Value1           = helpers.ToDecimal(dt.Rows[i]["Value1"].ToString()),
                                            Value2           = helpers.ToDecimal(dt.Rows[i]["Value2"].ToString()),
                                            Value3           = helpers.ToDecimal(dt.Rows[i]["Value3"].ToString()),
                                            Value4           = helpers.ToDecimal(dt.Rows[i]["Value4"].ToString()),
                                            Value5           = helpers.ToDecimal(dt.Rows[i]["Value5"].ToString()),
                                            Value6           = helpers.ToDecimal(dt.Rows[i]["Value6"].ToString()),
                                            AccountTypeId    = helpers.ToInt(    dt.Rows[i]["AccountTypeId"].ToString()),
                                            AccountStateId   = helpers.ToInt(    dt.Rows[i]["AccountStateId"].ToString()),
                                            HumanId          = helpers.ToInt(    dt.Rows[i]["HumanId"].ToString()),
                                            HasCredit        = helpers.ToInt(    dt.Rows[i]["HasCredit"].ToString()),
                                            HasPoints        = helpers.ToInt(    dt.Rows[i]["HasPoints"].ToString())
                                        };
                                        Accounts[i] = Account;

                                    }
                                    acctRes.Accounts = Accounts;

                                    if (acctRes.Status.Equals("Valid")) return Ok(acctRes);
                                }
                                else
                                {
                                    acctRes.Status   = "Error";
                                    acctRes.Response = "No Data Found, " + db.DataError;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) 
                {
                    helpers.WriteToLog("Account Err:" + diag + ";" + ex.Message); 
                }
            }

            return BadRequest (acctRes);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public AccountResponse getAccounts(int ActionType, string Vendor, string Location, Account[] Accounts)
        {
            return getAccount(ActionType, Vendor, Location, Accounts[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public AccountResponse getAccount(int ActionType, string Vendor, string Location, Account Account)
        {
            AccountRequest request = new AccountRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Account    = Account
            };

            AccountResponse response = new AccountResponse();

            apiAccount apiAccount = new apiAccount();
            IActionResult action = apiAccount.Post(request);
            if (action is OkObjectResult ok)          response = (AccountResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (AccountResponse)bad.Value;
            return response;
        }
    }
}
