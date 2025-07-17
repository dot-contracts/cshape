using System.Drawing;

namespace nexus.shared.local
{
    public class ClassRequest
    {
        public int     ActionType   { get; set; } = 0;
        public string  Vendor       { get; set; } = string.Empty;
        public string  Location     { get; set; } = string.Empty;
        //public int     ActionTypeId { get; set; } = 0;
        public Class?  Class        { get; set; } = new Class();
    }

    public class ClassResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Class[]  Classes  { get; set; } = Array.Empty<Class>();
    }

    public class Class
    {
        public string?  Parent        { get; set; } = string.Empty;
        public string?  Description   { get; set; } = string.Empty;
        public string?  ClassType     { get; set; } = string.Empty;
        public string?  ClassState    { get; set; } = string.Empty;
        public int?     ClassId       { get; set; } = -1;
        public int?     ClassTypeId   { get; set; } = -1;
        public int?     ClassStateId  { get; set; } = -1;
        public int?     ParentId      { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(ClassType) || !string.IsNullOrEmpty(ClassState) || ClassId > 0 || ClassTypeId>0 || ClassStateId>0 || ParentId > 0 ||
                    !string.IsNullOrEmpty(Description));
        }
    }
}

