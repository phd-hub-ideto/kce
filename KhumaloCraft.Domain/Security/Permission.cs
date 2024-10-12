namespace KhumaloCraft.Domain.Security;

public enum Permission
{
    [GrantOn(SecurityEntityType.Administrator)]
    SuperUser = 1,

    [GrantOn(SecurityEntityType.Administrator)]
    UpdatePrivilegedPermissions = 2,

    [GrantOn(SecurityEntityType.Administrator)]
    ManageSettings = 3,

    [GrantOn(SecurityEntityType.Administrator)]
    ManageUsers = 4,

    [GrantOn(SecurityEntityType.Administrator)]
    ManageRoles = 5,

    [GrantOn(SecurityEntityType.Administrator)]
    ViewOrders = 6,

    [GrantOn(SecurityEntityType.Administrator)]
    ProcessOrder = 7,

    [GrantOn(SecurityEntityType.Administrator)]
    ManageProduct = 8,

    [GrantOn(SecurityEntityType.Administrator)]
    [GrantOn(SecurityEntityType.User)]
    ViewOrderHistory = 9
}

public enum AdministratorPermission
{
    SuperUser = Permission.SuperUser,
    UpdatePrivilegedPermissions = Permission.UpdatePrivilegedPermissions,
    ManageSettings = Permission.ManageSettings,
    ManageUsers = Permission.ManageUsers,
    ManageRoles = Permission.ManageRoles,
    ViewOrders = Permission.ViewOrders,
    ProcessOrder = Permission.ProcessOrder,
    ManageProduct = Permission.ManageProduct,
    ViewOrderHistory = Permission.ViewOrderHistory
}

public enum UserPermission
{
    ViewOrderHistory = Permission.ViewOrderHistory
}