
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiCard : ControllerBase
    {
        [HttpPost("Card")]
        [Authorize]
        public IActionResult Post([FromBody] CardRequest cardReq)
        {
            CardResponse cardRes = new CardResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (cardReq!=null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string Params  =  "I~S~ActionType~" + cardReq.ActionType;
                               Params += ";I~S~Vendor~"     + cardReq.Vendor;
                               Params += ";I~S~Location~"   + cardReq.Location;

                        diag = "1";
                        if (cardReq.Card != null)
                        {
                            Card card = cardReq.Card;

                            bool isValid = false;
                            if (cardReq.ActionType.Equals(1))               isValid= card.HumanId > 0;
                            else if (card.HumanId > 0 || card.CardId > 0)   isValid = true;

                            if (!isValid)
                            {
                                cardRes.Status = "Error";
                                cardRes.Response = "No HumanId or CardId";
                            }
                            else
                            {
                                if (card.HasData())
                                {
                                    diag = "2";
                                    if (!string.IsNullOrEmpty(card.CardType)) Params += ";I~S~CardType~" + card.CardType;
                                    if (!cardReq.ActionType.Equals(0))
                                        if (!string.IsNullOrEmpty(card.CardState)) Params += ";I~S~CardState~" + card.CardState;
                                    if (!string.IsNullOrEmpty(card.CardNo))   Params += ";I~S~CardNo~" + card.CardNo;
                                    if (!string.IsNullOrEmpty(card.MagNo))    Params += ";I~S~MagNo~" + card.MagNo;
                                    if (!string.IsNullOrEmpty(card.PinNo))    Params += ";I~S~PinNo~" + card.PinNo;

                                    diag = "3";
                                    if (helpers.IsDate(card.LastValidated))   Params += ";I~S~LastValidated~" + card.LastValidated.ToString();
                                    if (helpers.IsDate(card.PrintDate))       Params += ";I~S~PrintDate~" + card.PrintDate.ToString();
                                    if (helpers.IsDate(card.LastUsed))        Params += ";I~S~LastUsed~" + card.LastUsed.ToString();

                                    diag = "4";
                                    if (card.CardId > 0)                      Params += ";I~S~CardId~" + card.CardId.ToString();
                                    if (card.HumanId > 0)                     Params += ";I~S~HumanId~" + card.HumanId.ToString();
                                    if (card.CohortId > 0)                    Params += ";I~S~CohortId~" + card.CohortId.ToString();
                                    if (card.Prints > 0)                      Params += ";I~S~Prints~" + card.Prints.ToString();
                                    if (card.LastUsedAt > 0)                  Params += ";I~S~LastUsedAt~" + card.LastUsedAt.ToString();
                                    if (card.LineItemId > 0)                  Params += ";I~S~LineItemId~" + card.LineItemId.ToString();
                                    if (card.CardTypeId > 0)                  Params += ";I~S~CardTypeId~" + card.CardTypeId.ToString();
                                    if (cardReq.ActionType > 0)
                                        if (card.CardStateId > 0)             Params += ";I~S~CardStateId~" + card.CardStateId.ToString();
                                }

                                diag = "5";
                                helpers.WriteToLog("apiCard:" + Params);
                                DataTable dt = db.GetDataTable("loc.api_Card", Params);
                                if (dt.Rows.Count > 0 && dt.Columns.Count>1 )
                                {
                                    cardRes.Status   = dt.Rows[0]["Status"].ToString();
                                    cardRes.Response = dt.Rows[0]["Response"].ToString();

                                    diag = "6";
                                    Card[] Cards = new Card[dt.Rows.Count];
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        diag = "7" + i.ToString();

                                        card = new Card()
                                        {
                                            CardId        = helpers.ToInt (dt.Rows[i]["CardPk"].ToString()),
                                            CardNo        =                dt.Rows[i]["CardNo"].ToString(),
                                            CardType      =                dt.Rows[i]["CardType"].ToString(),
                                            CardState     =                dt.Rows[i]["CardState"].ToString(),
                                            Human         =                dt.Rows[i]["Human"].ToString(),
                                            PinNo         =                dt.Rows[i]["PinNo"].ToString(),
                                            LastValidated = helpers.ToDate(dt.Rows[i]["LastValidated"].ToString()).ToString("dd MMM, yyyy HH:mm:ss"),
                                            PrintDate     = helpers.ToDate(dt.Rows[i]["PrintDate"].ToString()).ToString("dd MMM, yyyy HH:mm:ss"),
                                            LastUsed      = helpers.ToDate(dt.Rows[i]["LastUsed"].ToString()).ToString("dd MMM, yyyy HH:mm:ss"),
                                            Prints        = helpers.ToInt (dt.Rows[i]["Prints"].ToString()),
                                            LastUsedAt    = helpers.ToInt (dt.Rows[i]["LastUsedAt"].ToString()),
                                            HumanId       = helpers.ToInt (dt.Rows[i]["HumanId"].ToString()),
                                            CohortId      = helpers.ToInt (dt.Rows[i]["CohortId"].ToString()),
                                            LineItemId    = helpers.ToInt (dt.Rows[i]["LineItemId"].ToString()),
                                            CardTypeId    = helpers.ToInt (dt.Rows[i]["CardTypeId"].ToString()),
                                            CardStateId   = helpers.ToInt (dt.Rows[i]["CardStateId"].ToString()),
                                        };

                                        diag = "8";

                                        apiAccount apiAccount = new apiAccount();
                                        Account acct = new Account() { HumanId = card.HumanId };
                                        AccountResponse accRes = apiAccount.getAccount(cardReq.ActionType, cardReq.Vendor, cardReq.Location, acct);
                                        if (accRes.Status.Equals("Valid")) card.Account = accRes.Accounts;

                                        diag = "9";

                                        Cards[i] = card;
                                    }
                                    cardRes.Cards = Cards;

                                    if (cardRes.Status.Equals("Valid")) return Ok(cardRes);
                                }
                                else
                                {
                                    cardRes.Status = "Error";
                                    cardRes.Response = "No Data Found, " + db.DataError;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Card Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(cardRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CardResponse getCards(int ActionType, string Vendor, string Location, Card[] Cards)
        {
            return getCard(ActionType, Vendor, Location, Cards[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public CardResponse getCard(int ActionType, string Vendor, string Location, Card Card)
        {
            CardRequest request = new CardRequest(ActionType, Vendor, Location, Card);
            CardResponse response = new CardResponse();

            apiCard apiCard = new apiCard();
            IActionResult action = apiCard.Post(request);
            if (action is OkObjectResult ok) response = (CardResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (CardResponse)bad.Value;
            return response;
        }

    }
}
