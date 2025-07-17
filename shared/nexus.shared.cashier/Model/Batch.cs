
namespace nexus.api.cashier.Model
{
    public class BatchRequest
    {
        public int      ActionType   { get; set; } = 0;
        public string   Vendor       { get; set; } = string.Empty;   
        public string   Location     { get; set; } = string.Empty;      
        public Batch?   Batch        { get; set; } = new Batch(); 
    }

    public class BatchResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public string   Response     { get; set; } = String.Empty;
        public Batch[]  Batchs       { get; set; } = Array.Empty<Batch>();
    }

    public class Batch
    {
        public int?      BatchId       { get; set; } = -1;
        public string?   Description { get; set; } = String.Empty;
        public string?   BatchType   { get; set; } = String.Empty;
        public string?   BatchState  { get; set; } = String.Empty;
        public string?   OpenDate    { get; set; } = String.Empty;
        public string?   OpenTime    { get; set; } = String.Empty;
        public string?   CloseDate   { get; set; } = String.Empty;
        public string?   CloseTime   { get; set; } = String.Empty;
        public Lineitem[]?  Lineitems { get; set; } = Array.Empty<Lineitem>();
        public int?      Days       { get; set; } = -1;
        public int?      VenueId    { get; set; } = -1;
        public int?      ParentId   { get; set; } = -1;
        public int?      ComputerId { get; set; } = -1;
        public int?      WorkerId   { get; set; } = -1;
        public int?      PreviousId { get; set; } = -1;
        public int?      MemoId { get; set; } = -1;
        public decimal?  Value1 { get; set; } = -1;
        public decimal?  Value2 { get; set; } = -1;
        public decimal?  Value3 { get; set; } = -1;
        public decimal?  Value4 { get; set; } = -1;
        public decimal?  Value5 { get; set; } = -1;
        public decimal?  Value6 { get; set; } = -1;
        public decimal?  Value7 { get; set; } = -1;
        public decimal?  Value8 { get; set; } = -1;
        public string?   Field1 { get; set; } = string.Empty;
        public string?   Field2 { get; set; } = string.Empty;
        public string?   Field3 { get; set; } = string.Empty;
        public string?   Field4 { get; set; } = string.Empty;
        public string?   Field5 { get; set; } = string.Empty;
        public string?   Field6 { get; set; } = string.Empty;
        public string?   Field7 { get; set; } = string.Empty;
        public string?   Field8 { get; set; } = string.Empty;
        public int?      BatchTypeId  { get; set; } = -1;
        public int?      BatchStateId { get; set; } = -1;


        public bool HasData()
        {
            return (BatchId > 0
            || !string.IsNullOrEmpty(Description)
            || !string.IsNullOrEmpty(BatchType)
            || !string.IsNullOrEmpty(BatchState)

            || !string.IsNullOrEmpty(OpenDate)
            || !string.IsNullOrEmpty(OpenTime)
            || !string.IsNullOrEmpty(CloseDate)
            || !string.IsNullOrEmpty(CloseTime)

            || Days > 0
            || VenueId >= 0
            || ParentId >= 0
            || ComputerId >= 0
            || WorkerId >= 0
            || PreviousId >= 0
            || MemoId >= 0

            || Value1 >= 0
            || Value2 >= 0
            || Value3 >= 0
            || Value4 >= 0
            || Value5 >= 0
            || Value6 >= 0
            || Value7 >= 0
            || Value8 >= 0

            || !string.IsNullOrEmpty(Field1)
            || !string.IsNullOrEmpty(Field2)
            || !string.IsNullOrEmpty(Field3)
            || !string.IsNullOrEmpty(Field4)
            || !string.IsNullOrEmpty(Field5)
            || !string.IsNullOrEmpty(Field6)
            || !string.IsNullOrEmpty(Field7)
            || !string.IsNullOrEmpty(Field8));

        }

    }
}

