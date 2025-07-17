
namespace nexus.common
{
    public class MenuRequest
    {
        public int     WorkerId   { get; set; } = 0;
        public int     ModuleId   { get; set; } = 0;
        public int     DeviceType { get; set; } = 0;
    }

    public class MenuResponse
    {
        public string  Status    { get; set; } = string.Empty;
        public string  Response  { get; set; } = string.Empty;
        public Menu[]  Menus     { get; set; } = Array.Empty<Menu>();
    }

    public class Menu
    {
        public int      MenuId          { get; set; } = -1;
        public int      ItemId          { get; set; } = -1;
        public int      ModuleId        { get; set; } = -1;
        public string   MenuDescription { get; set; } = string.Empty;
        public string   ItemDescription { get; set; } = string.Empty;
        public string   Assembly        { get; set; } = string.Empty;
        public string   RoleType        { get; set; } = string.Empty;
        public string   EndPoint        { get; set; } = String.Empty;
        public string   Property        { get; set; } = String.Empty;
        public bool     HasSubMenu      { get; set; } = false;

        public bool HasData()
        {
            return (MenuId >= 0 || ItemId >= 0 || ModuleId >= 0 || !string.IsNullOrEmpty(MenuDescription) || !string.IsNullOrEmpty(ItemDescription) || !string.IsNullOrEmpty(Assembly) || !string.IsNullOrEmpty(RoleType));
        }

    }
}

