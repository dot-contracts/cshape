using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using nexus.common.control;
using nexus.common.control.Themes;
using nexus.shared.common;

namespace nexus.common.control
{
    public partial class NxMenu : NxPanelBase
    {
        public const string MENU_CAPTION_LBL  = "MenuDescription";
        public const string MENU_ITEM_LBL     = "ItemDescription";
        public const string MENU_ASSEMBLY_LBL = "Assembly";
        public const string MENU_ROLETYPE     = "RoleType";

        private Border                             _outerBorder;
        private StackPanel                         Stacker;
        private Dictionary<string, NxMenuGroup>    _menuGroupsByCaption = new();
        private Menu[]                             _menuItemsArray      = Array.Empty<Menu>();

        public event OnMenuItemClickedEventHandler OnMenuItemClicked;
        public delegate void                        OnMenuItemClickedEventHandler(NxMenuItem menuItem, string prompt, string assembly, string entry, string snapIn, int moduleId, int functionId);

        public NxMenu()
        {
            DefaultStyleKey   = typeof(NxMenu);
            ProtectedCursor   = InputSystemCursor.Create(InputSystemCursorShape.Arrow);

            ApplyThemeDefaults();
            NxThemeManager.ThemeChanged += (sender, args) => ApplyThemeDefaults();

            Session.OnUserChanged += Session_OnUserChanged;
        }

        private void ApplyThemeDefaults()
        {
            if (_outerBorder != null)
            {
                _outerBorder.BorderBrush = NxThemeManager.Current.GetThemeBrush("NxMenuBack", Colors.White);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _outerBorder = GetTemplateChild("OuterBorder") as Border;
            Stacker      = GetTemplateChild("Stacker")      as StackPanel;

            if (_menuItemsArray.Length > 0 && Stacker != null)
            {
                CreateMenu(_menuItemsArray);
            }
            else if (Session.CurrentUser?.Menus?.Length > 0)
            {
                CreateMenu(Session.CurrentUser.Menus);
            }
        }

        private void Session_OnUserChanged(object? sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (Session.CurrentUser?.Menus != null && Session.CurrentUser.Menus.Length > 0)
                {
                    CreateMenu(Session.CurrentUser.Menus);
                }
                else
                {
                    Stacker?.Children.Clear();
                    _menuGroupsByCaption.Clear();
                }
            });
        }

        public bool LoadMenuFromResponse(MenuResponse response)
        {
            return response != null ? CreateMenu(response.Menus) : false;
        }

        public bool CreateMenu(Menu[] menuItems)
        {
            if (menuItems == null || menuItems.Length == 0) return false;

            //_menuItemsArray = menuItems.Where(m => m.HasData() && Session.IsInRole(m.RoleType)).ToArray();
            _menuItemsArray = menuItems.ToArray();

            if (Stacker == null) return false;

            Stacker.Children.Clear();
            _menuGroupsByCaption.Clear();

            var    groupedMenus    = _menuItemsArray.GroupBy(m => m.MenuDescription);
            string defaultExpanded = groupedMenus.FirstOrDefault()?.Key ?? string.Empty;

            foreach (var group in groupedMenus)
            {
                var menuGroup = new NxMenuGroup(group.Key);

                menuGroup.MenuGroupStatusEvent += HandleMenuGroupStatus;
                menuGroup.OnMenuItemClicked    += MenuGroup_OnMenuItemClicked;

                Stacker.Children.Add(menuGroup);
                _menuGroupsByCaption[group.Key] = menuGroup;
            }

            DeferMenuPopulation(defaultExpanded);
            return true;
        }

        private void DeferMenuPopulation(string expandedCaption)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var menu in _menuItemsArray)
                {
                   // if (!Session.IsInRole(menu.RoleType)) continue;

                    if (_menuGroupsByCaption.TryGetValue(menu.MenuDescription, out var group))
                    {
                        string assemblyData = $"{menu.Assembly};{menu.EndPoint};{menu.Property};{menu.ModuleId};{menu.ItemId}";
                        bool   show         = group.CaptionText == expandedCaption;

                        group.AddMenuItem(menu.ItemDescription, assemblyData, show);
                    }
                }
            });
        }

        private void MenuGroup_OnMenuItemClicked(NxMenuItem menuItem, string prompt, string assembly, string entry, string snapIn, int moduleId, int functionId)
        {
            OnMenuItemClicked?.Invoke(menuItem, prompt, assembly, entry, snapIn, moduleId, functionId);
        }

        private void HandleMenuGroupStatus(object sender)
        {
            NxMenuGroup clickedGroup = sender as NxMenuGroup;

            foreach (var child in Stacker.Children)
            {
                if (child is NxMenuGroup group && group.CaptionText != clickedGroup.CaptionText)
                {
                    group.RemoveMenuItem();
                }
            }
        }

        public static Menu[] ConvertDataTableToMenus(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(row => new Menu
                        {
                            MenuDescription = row[MENU_CAPTION_LBL].ToString(),
                            ItemDescription = row[MENU_ITEM_LBL].ToString(),
                            RoleType        = row[MENU_ROLETYPE].ToString(),
                            Assembly        = row[MENU_ASSEMBLY_LBL].ToString().Split(';').ElementAtOrDefault(0) ?? "",
                            EndPoint        = row[MENU_ASSEMBLY_LBL].ToString().Split(';').ElementAtOrDefault(1) ?? "",
                            Property        = row[MENU_ASSEMBLY_LBL].ToString().Split(';').ElementAtOrDefault(2) ?? "",
                            ModuleId        = helpers.ToInt(row[MENU_ASSEMBLY_LBL].ToString().Split(';').ElementAtOrDefault(3)),
                            ItemId          = helpers.ToInt(row[MENU_ASSEMBLY_LBL].ToString().Split(';').ElementAtOrDefault(4))
                        })
                        .ToArray();
        }
    }
}
