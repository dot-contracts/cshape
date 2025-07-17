using Newtonsoft.Json.Linq;

namespace nexus.shared.common
{
    public class OptionRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public Option[] Options      { get; set; } = Array.Empty<Option>();
    }

    public class OptionResponse
    {
        public string       Status       { get; set; } = String.Empty;
        public string       Response     { get; set; } = String.Empty;
        public List<Option> Options      { get; set; } = new List<Option>();
    }

    public class Option
    {
        public string     Value           { get; set; } = string.Empty;
        public int        ValueId         { get; set; }
        public int        ParentId        { get; set; }
        public int        OptionId        { get; set; }
        public string     ParentCode      { get; set; } = string.Empty;
        public string     OptionCode      { get; set; } = string.Empty;
        public string     VarDefault      { get; set; } = string.Empty;
        public string     ValueText       { get; set; } = string.Empty;
        public string     ValueType       { get; set; } = string.Empty;
    
        public bool       Computer        { get; set; }
        public int        ComputerId      { get; set; }
        public bool       ComputerRole    { get; set; }
        public int        ComputerRoleId  { get; set; }
    
        public bool       Worker          { get; set; }
        public int        WorkerId        { get; set; }
        public bool       WorkerRole      { get; set; }
        public int        WorkerRoleId    { get; set; }

        public int        ValueTypeId     { get; set; }
    }
        
 }

