using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class PagingRequest
    {
        public int    ActionType { get; set; } = 0;
        public string Vendor     { get; set; } = string.Empty;
        public string Location   { get; set; } = string.Empty;
        public string OpenDate   { get; set; } = string.Empty;
        public string CloseDate  { get; set; } = string.Empty;
        public string Search     { get; set; } = string.Empty;
        public string Value      { get; set; } = string.Empty;
    }

    public class PagingResponse
    {
        public string   Status    { get; set; } = string.Empty;
        public string   Response  { get; set; } = string.Empty;
        public Paging[] Results   { get; set; } = Array.Empty<Paging>();
    }

    public class Paging
    {
        public string?  EventDate   { get; set; } = String.Empty;
        public string?  Description { get; set; } = String.Empty;
        public string?  Property    { get; set; } = String.Empty;
        public string?  Entity      { get; set; } = String.Empty;
        public string?  Human       { get; set; } = String.Empty;
        public int?     EventId     { get; set; } = 0;
        public int?     EntityId    { get; set; } = 0;
        public int?     HumanId     { get; set; } = 0;

        public bool HasData()
        {
            return (true);
        }
    }
}

