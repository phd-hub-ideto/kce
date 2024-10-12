namespace KhumaloCraft.Domain.Security.Validators;

internal static class PrivilegedPermissions
{
    private static readonly HashSet<Permission> _privilegedPermissions =
    [
        Permission.UpdatePrivilegedPermissions,
        Permission.ManageSettings
    ];

    internal static bool HasPrivilegedPermissions(List<PermissionIdentifier> permissions)
    {
        if (permissions == null)
        {
            return false;
        }

        return permissions.Any(p => p.Permission.HasValue && _privilegedPermissions.Contains(p.Permission.Value));
    }
}
