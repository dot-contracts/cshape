
namespace nexus.shared.application
{
    public class UserRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public User?   User       { get; set; } = new User();
    }

    public class UserResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public User[]   Users    { get; set; } = Array.Empty<User>();
    }

    public class User
    {
        public int?     UserId      { get; set; } = -1;
        public string?  Description { get; set; } = string.Empty;
        public string?  UserType    { get; set; } = string.Empty;
        public string?  HumanType   { get; set; } = string.Empty;
        public string?  DeviceType  { get; set; } = string.Empty;
        public string?  Module      { get; set; } = string.Empty;
        public string?  Function    { get; set; } = string.Empty;

        public int?     UserTypeId   { get; set; } = -1;
        public int?     HumanTypeId  { get; set; } = -1;
        public int?     DeviceTypeId { get; set; } = -1;
        public int?     ModuleId     { get; set; } = -1;
        public int?     FunctionId   { get; set; } = -1;

        public bool HasData()
        {
            return (UserId > 0 || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(UserType) || !string.IsNullOrEmpty(HumanType) || !string.IsNullOrEmpty(DeviceType) || !string.IsNullOrEmpty(Module) || !string.IsNullOrEmpty(Function) ||
            UserTypeId >= 0 || HumanTypeId >=0 || DeviceTypeId >= 0 || ModuleId >= 0) || FunctionId >= 0;
        }
    }
}

