
namespace nexus.shared.local
{
    public class ContactRequest
    {
        public int      ActionType { get; set; } = 0;
        public string   Vendor     { get; set; } = string.Empty;   
        public string   Location   { get; set; } = string.Empty;      
        public Contact  Contact    { get; set; } = new Contact();

        public ContactRequest(int ActionType, string Vendor, string Location, Contact contact = null, int EntityId = -1)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;   
            this.Contact    = (contact == null ? new Contact() : contact);
            this.Contact.EntityId = (EntityId > 0 ? EntityId  : - 1 );
        }

    }

    public class ContactResponse
    {
        public string?    Status      { get; set; } = String.Empty;
        public string?    Response    { get; set; } = String.Empty;
        public Contact[] Contacts    { get; set; } = Array.Empty<Contact>();

        public ContactResponse()
        {
            Contacts = new Contact[1];
            Contacts[0] = new Contact();
        }

    }
    public class Contact
    {
        public int?    ContactId       { get; set; } = -1;
        public int?    EntityId        { get; set; } = -1;
        public string? ContactType     { get; set; } = String.Empty;
        public string? ContactState    { get; set; } = String.Empty;
        public string? ContactLocation { get; set; } = String.Empty;
        public string? Detail          { get; set; } = String.Empty;
        public string? Contact1        { get; set; } = String.Empty;
        public string? Contact2        { get; set; } = String.Empty;
        public string? Contact3        { get; set; } = String.Empty;
        public string? StreetName      { get; set; } = String.Empty;
        public int?    StreetId        { get; set; } = -1;
        public string? SuburbName      { get; set; } = String.Empty;
        public int?    SuburbId        { get; set; } = -1;
        public string? PostCode        { get; set; } = String.Empty;
        public string? ProvinceName    { get; set; } = String.Empty;
        public string? CountryName     { get; set; } = String.Empty;
        public int?    Default         { get; set; } = -1;
        public int?    ContactTypeId   { get; set; } = -1;
        public int?    ContactStateId  { get; set; } = -1;

        public bool HasData()
        {
            return (ContactId >= 0 || EntityId >= 0 || !string.IsNullOrEmpty(ContactType) || !string.IsNullOrEmpty(ContactState) || !string.IsNullOrEmpty(ContactLocation) || !string.IsNullOrEmpty(Detail) ||
                    !string.IsNullOrEmpty(Contact1) || !string.IsNullOrEmpty(Contact2) || !string.IsNullOrEmpty(Contact3) 
                    || StreetId >= 0 || !string.IsNullOrEmpty(StreetName)
                    || SuburbId >= 0 || !string.IsNullOrEmpty(SuburbName)
                    || !string.IsNullOrEmpty(PostCode) || !string.IsNullOrEmpty(ProvinceName) || !string.IsNullOrEmpty(CountryName)
                    || Default >= 0 || ContactTypeId>=0 || ContactStateId>=0 );
        }

    }
}

