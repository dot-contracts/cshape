using Newtonsoft.Json.Linq;

namespace nexus.shared.common
{
    public class StartUpRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public StartUp? StartUp      { get; set; } = new StartUp(); 
    }

    public class StartUpResponse
    {
        public string   Status    { get; set; } = String.Empty;
        public string   Response  { get; set; } = String.Empty;
        public StartUp? StartUp   { get; set; } = new StartUp(); 
    }

    public class StartUp
    {
        public string     Value           { get; set; } = string.Empty;
        public int        ValueId         { get; set; }
        public int        ParentId        { get; set; }
        public int        StartUpId       { get; set; }
        public string     ParentCode      { get; set; } = string.Empty;
        public string     StartUpCode     { get; set; } = string.Empty;
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

