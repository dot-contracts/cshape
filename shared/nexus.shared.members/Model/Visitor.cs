using nexus.api.Model.Local;

namespace nexus.api.Model.Membership
{
    public class VisitorRequest
    {
        public int           ActionType   { get; set; } = 0;
        public string        Vendor       { get; set; } = string.Empty;   
        public string        Location     { get; set; } = string.Empty;
        public Visitor       Visitor      { get; set; } = new Visitor();
    }

    public class VisitorResponse
    {
        public string     Status       { get; set; } = String.Empty;
        public string     Response     { get; set; } = String.Empty;
        public Visitor    Visitor      { get; set; } = new Visitor();
    }

    public class Visitor
    {
        public int?      VisitorId       { get; set; } = -1;
        public string?   VisitorType     { get; set; } = string.Empty;
        public string?   VisitorState    { get; set; } = string.Empty;
        public string?   VisitorNo       { get; set; } = string.Empty;
        public int?      VisitorTypeId   { get; set; } = -1;
        public int?      VisitorStateId  { get; set; } = -1;
        public string?   CardNo          { get; set; } = string.Empty;

        public bool HasData()
        {
            return (true    );
        }

    }

}
