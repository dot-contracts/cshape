namespace nexus.shared.common
{
    public class EnumCodeRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public Enumcode? EnumCode    { get; set; } = new Enumcode(); 
    }

    public class EnumCodeResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public string   Response     { get; set; } = String.Empty;
        public Enumcode[] EnumCodes  { get; set; } = Array.Empty<Enumcode>();
    }

    public class Enumcode
    {
        public int?    EnumPk        { get; set; } = -1;
        public int?    ValuePk       { get; set; } = -1;
        public int?    ValueType     { get; set; } = -1;
        public string? EnumCode      { get; set; } = String.Empty;
        public string? EnumPath      { get; set; } = String.Empty;
        public string? EnumDesc      { get; set; } = String.Empty;
        public string? ValueCode     { get; set; } = String.Empty;
        public string? ValueDesc     { get; set; } = String.Empty;
        public string? Description   { get; set; } = String.Empty;
        public string? EnumType      { get; set; } = String.Empty;
        public string? EnumState     { get; set; } = String.Empty;
        public int?    Group         { get; set; } = -1;
        public string? Value         { get; set; } = String.Empty;
        public int?    DenoteId      { get; set; } = -1;
        public int?    ParentId      { get; set; } = -1;
        public int?    Sequence      { get; set; } = -1;
    }
 }

