using System.Drawing;

namespace nexus.shared.local
{
    public class ClassEntityRequest
    {
        public int         ActionType  { get; set; } = 0;
        public string      Vendor      { get; set; } = string.Empty;
        public string      Location    { get; set; } = string.Empty;
        public ClassEntity ClassEntity { get; set; } = new ClassEntity();

        public ClassEntityRequest(int ActionType, string Vendor, string Location, ClassEntity classEntity = null, int EntityId = -1)
        {
            this.ActionType  = ActionType;
            this.Vendor      = Vendor;
            this.Location    = Location;
            this.ClassEntity = (classEntity == null ? new ClassEntity() : classEntity);
            this.ClassEntity.EntityId = (EntityId > 0 ? EntityId : -1);
        }
    }

    public class ClassEntityResponse
    {
        public string         Status   { get; set; } = string.Empty;
        public string         Response { get; set; } = string.Empty;
        public ClassEntity[]  Classes  { get; set; } = Array.Empty<ClassEntity>();

        public ClassEntityResponse()
        {
            Classes = new ClassEntity[1];
            ClassEntity clas = new ClassEntity();
            Classes[0] = clas;
        }

    }

    public class ClassEntity
    {
        public string?  Parent        { get; set; } = string.Empty;
        public string?  Class         { get; set; } = string.Empty;
        public string?  Entity        { get; set; } = string.Empty;
        public string?  ClassType     { get; set; } = string.Empty;
        public string?  ClassState    { get; set; } = string.Empty;
        public int?     ClassId       { get; set; } = -1;
        public int?     EntityId      { get; set; } = -1;
        public int?     ParentId      { get; set; } = -1;

        public bool HasData()
        {
            return (!string.IsNullOrEmpty(Class) || !string.IsNullOrEmpty(Entity) || !string.IsNullOrEmpty(ClassType) || !string.IsNullOrEmpty(ClassState) || ClassId > 0 || EntityId > 0 || ParentId > 0 );
        }
    }
}

