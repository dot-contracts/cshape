

using nexus.common;
using System.Security.Cryptography.Xml;

namespace nexus.shared.local
{
    public class CardRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public Card?   Card       { get; set; } = new Card();

        public CardRequest()
        {
        }

        public CardRequest(int ActionType, string Vendor, string Location, Card card = null, int HumanId = -1)
        {
            this.ActionType = ActionType;
            this.Vendor = Vendor;
            this.Location = Location;
            this.Card = (card == null ? new Card() : card);
            if (this.Card!=null)
                this.Card.HumanId = (HumanId > 0 ? HumanId : -1);
        }
    }

    public class CardResponse
    {
        public string  Status    { get; set; } = string.Empty;
        public string  Response  { get; set; } = string.Empty;
        public Card[]  Cards     { get; set; } = Array.Empty<Card>();

        public CardResponse()
        {
            Cards           = new Card[1];
            Card card       = new Card();
            card.Account    = new Account[1];
            card.Account[0] = new Account();
            Cards[0] = card;
        }
    }

    public class Card
    {
        public int?       CardId        { get; set; } = -1;
        public string?    CardNo        { get; set; } = string.Empty;
        public string?    CardType      { get; set; } = string.Empty;
        public string?    CardState     { get; set; } = string.Empty;
        public string?    Human         { get; set; } = string.Empty;
        public string?    MagNo         { get; set; } = string.Empty;
        public string?    PinNo         { get; set; } = string.Empty;
        public string?    LastValidated { get; set; } = string.Empty;
        public string?    PrintDate     { get; set; } = string.Empty;
        public int?       Prints        { get; set; } = -1;
        public string?    LastUsed      { get; set; } = string.Empty;
        public int?       LastUsedAt    { get; set; } = -1;
        public Account[]? Account       { get; set; } = Array.Empty<Account>();
        public int?       HumanId       { get; set; } = -1;
        public int?       CohortId      { get; set; } = -1;
        public int?       LineItemId    { get; set; } = -1;
        public int?       CardTypeId    { get; set; } = -1;
        public int?       CardStateId   { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(CardType) || !string.IsNullOrEmpty(CardState) || !string.IsNullOrEmpty(CardNo) || !string.IsNullOrEmpty(MagNo) || !string.IsNullOrEmpty(PinNo) || helpers.IsDate(LastValidated) ||
            helpers.IsDate(PrintDate) || helpers.IsDate(LastUsed) || CardId >= 0 || HumanId >= 0 || CohortId >= 0 || Prints >= 0 || LastUsedAt >= 0 || LineItemId >= 0 || CardTypeId >= 0 || CardStateId >= 0);
        }

    }
}

