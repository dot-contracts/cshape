using System.Drawing;

namespace nexus.api.local.Model
{
    public class RoleRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public Role?   Role       { get; set; } = new Role();
    }

    public class RoleResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Role[]   Roles    { get; set; } = Array.Empty<Role>();
    }

    public class Role
    {
        public int?     RoleId      { get; set; } = -1;
        public string?  Description { get; set; } = string.Empty;
        public string?  RoleType    { get; set; } = string.Empty;
        public string?  HumanType   { get; set; } = string.Empty;
        public string?  DeviceType  { get; set; } = string.Empty;
        public string?  Module      { get; set; } = string.Empty;
        public string?  Function    { get; set; } = string.Empty;

        public int?     RoleTypeId   { get; set; } = -1;
        public int?     HumanTypeId  { get; set; } = -1;
        public int?     DeviceTypeId { get; set; } = -1;
        public int?     ModuleId     { get; set; } = -1;
        public int?     FunctionId   { get; set; } = -1;

        public bool HasData()
        {
            return (RoleId > 0 || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(RoleType) || !string.IsNullOrEmpty(HumanType) || !string.IsNullOrEmpty(DeviceType) || !string.IsNullOrEmpty(Module) || !string.IsNullOrEmpty(Function) ||
            RoleTypeId >= 0 || HumanTypeId >=0 || DeviceTypeId >= 0 || ModuleId >= 0) || FunctionId >= 0;
        }
    }
}

