
namespace nexus.api.cashier.Model
{
    public class LineitemRequest
    {
        public int       ActionType   { get; set; } = 0;
        public string    Vendor       { get; set; } = string.Empty;   
        public string    Location     { get; set; } = string.Empty;      
        public Lineitem? Lineitem     { get; set; } = new Lineitem(); 
    }

    public class LineitemResponse
    {
        public string     Status      { get; set; } = String.Empty;
        public string     Response    { get; set; } = String.Empty;
        public Lineitem[] Lineitems   { get; set; } = Array.Empty<Lineitem>();
    }

    public class Lineitem
    {
        public int?      BatchId        { get; set; } = -1;
        public int?      LineitemId     { get; set; } = -1;
        public string?   LineitemType   { get; set; } = String.Empty;
        public string?   LineitemState  { get; set; } = String.Empty;
        public string?   LineitemDate   { get; set; } = String.Empty;
        public string?   LineitemNo     { get; set; } = String.Empty;
        public string?   Reference      { get; set; } = String.Empty;
        public decimal?  Amount         { get; set; } = -1;
        public string?   Detail         { get; set; } = String.Empty;
        public int?      ComputerId     { get; set; } = -1;
        public int?      WorkerId       { get; set; } = -1;
        public int?      HumanId        { get; set; } = -1;
        public int?      DeviceId       { get; set; } = -1;
        public int?      LocationId     { get; set; } = -1;
        public int?      EntryId        { get; set; } = -1;
        public int?      MediaId        { get; set; } = -1;
        public int?      CRC            { get; set; } = -1;

        public bool HasData()
        {
            return (BatchId > 0
            || LineitemId > 0
            || !string.IsNullOrEmpty(LineitemType)
            || !string.IsNullOrEmpty(LineitemState)
            || !string.IsNullOrEmpty(LineitemDate)
            || !string.IsNullOrEmpty(LineitemNo)
            || !string.IsNullOrEmpty(Reference)
            || Amount > 0
            || !string.IsNullOrEmpty(Detail)

            || ComputerId >= 0
            || WorkerId >= 0
            || HumanId >= 0
            || DeviceId >= 0
            || LocationId > 0
            || EntryId >= 0
            || MediaId >= 0
            || CRC >= 0);
        }
    }
}

