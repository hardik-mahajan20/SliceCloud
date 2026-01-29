using SliceCloud.Repository.Constants;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Web.Helpers;

public static class SidebarMenuProvider
{
    #region GetMenu

    /// <summary>
    /// This static class provides the list of the menu items.
    /// </summary>
    /// <returns>A list of sidebar menu items</returns>
    public static List<SidebarMenuItemViewModel> GetMenu()
    {
        return
        [
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.DASHBOARD,
                Controller = SideBarOptionConstants.DASHBOARD,
                Action = SideBarOptionConstants.DASHBOARD,
                Icon = "dashboard_default.svg",
                ActiveIcon = "dashboard_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.USERS,
                Controller = SideBarOptionConstants.USERS,
                Action = SideBarOptionConstants.USERS,
                Icon = "user_default.svg",
                ActiveIcon = "user_active.svg",
                Roles = [RolesConstants.ADMIN]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.ROLE_AND_PERMISSION,
                Controller = SideBarOptionConstants.ROLE_AND_PERMISSION,
                Action = SideBarOptionConstants.ROLE_AND_PERMISSION,
                Icon = "roles_default.svg",
                ActiveIcon = "roles_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.MENU,
                Controller = SideBarOptionConstants.MENU,
                Action = SideBarOptionConstants.MENU,
                Icon = "menu_default.svg",
                ActiveIcon = "menu_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.TABLE_AND_SECTION,
                Controller = SideBarOptionConstants.TABLE_AND_SECTION,
                Action = SideBarOptionConstants.TABLE_AND_SECTION,
                Icon = "table_default.svg",
                ActiveIcon = "table_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.TAX_AND_FEES,
                Controller = SideBarOptionConstants.TAX_AND_FEES,
                Action = SideBarOptionConstants.TAX_AND_FEES,
                Icon = "tax_default.svg",
                ActiveIcon = "tax_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.ORDERS,
                Controller = SideBarOptionConstants.ORDERS,
                Action = SideBarOptionConstants.ORDERS,
                Icon = "orders_default.svg",
                ActiveIcon = "orders_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
            new SidebarMenuItemViewModel
            {
                Text = SideBarOptionConstants.CUSTOMERS,
                Controller = SideBarOptionConstants.CUSTOMERS,
                Action = SideBarOptionConstants.CUSTOMERS,
                Icon = "customer_default.svg",
                ActiveIcon = "customer_active.svg",
                Roles = [RolesConstants.ADMIN,]
            },
        ];
    }

    #endregion
}
