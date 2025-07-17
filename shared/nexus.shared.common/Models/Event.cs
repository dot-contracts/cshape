using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class EventRequest
    {
        public int    ActionType { get; set; } = 0;
        public string Vendor     { get; set; } = string.Empty;
        public string Location   { get; set; } = string.Empty;
        public string OpenDate   { get; set; } = string.Empty;
        public string CloseDate  { get; set; } = string.Empty;
        public string Search     { get; set; } = string.Empty;
        public string Value      { get; set; } = string.Empty;
        public string MaxRows    { get; set; } = "1000";
    }

    public class EventResponse
    {
        public string   Status    { get; set; } = string.Empty;
        public string   Response  { get; set; } = string.Empty;
        public Event[]  Results   { get; set; } = Array.Empty<Event>();
    }

    public class Event
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

