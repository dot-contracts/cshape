using nexus.api.Model.Local;

namespace nexus.api.Model.Membership
{
    public class MemberRequest
    {
        public int           ActionType   { get; set; } = 0;
        public string        Vendor       { get; set; } = string.Empty;   
        public string        Location     { get; set; } = string.Empty;
        public Member        Member       { get; set; } = new Member();
    }

    public class MemberResponse
    {
        public string?     Status       { get; set; } = String.Empty;
        public string?     Response     { get; set; } = String.Empty;
        public Member      Member       { get; set; } = new Member();
    }

    public class Member
    {
        public Patron                      Patron             { get; set; } = new Patron();
        public CardResponse?               Card               { get; set; } = new CardResponse();
        public MemberClubResponse?         MemberClub         { get; set; } = new MemberClubResponse();
        public MemberClubDeliveryResponse? MemberClubDelivery { get; set; } = new MemberClubDeliveryResponse();
        public ContactResponse?            Contact            { get; set; } = new ContactResponse();
        public ClassEntityResponse?        Interest           { get; set; } = new ClassEntityResponse();
        public bool HasData()
        {
            return (true    );
        }

    }

}

