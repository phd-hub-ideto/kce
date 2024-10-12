namespace KhumaloCraft.Domain.Security;

public class PermissionManager : IPermissionService
{
    private readonly IPermissionSource _permissionSource;

    internal PermissionManager(IPermissionSource permissionSource)
    {
        _permissionSource = permissionSource;
    }

    protected IEnumerable<PermissionKey> UserPermissions => _permissionSource.UserPermissions;

    public static PermissionManager Current => new PermissionManager(new CurrentPermissionScopePermissionSource());

    /// <summary>
    /// Returns true if the user has any administrative-level permissions.
    /// </summary>
    public bool HasAnyAdministrativePermission()
    {
        return UserPermissions.Any(item => item.GrantedOnSecurityEntityType == SecurityEntityType.Administrator);
    }

    /// <summary>
    /// GetGranted Entities will return all the current user's granted entities
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    public SecurityEntitySet GetGrantedEntities(Permission permission)
    {
        return SecurityEntitySet.Create(permission, UserPermissions);
    }

    public bool HasAnyPermission(Permission permission)
    {
        return UserPermissions.Any(item => item.Permission == permission);
    }

    public bool HasAnyPermission(SecurityEntityType securityEntityType)
    {
        return UserPermissions.Any(item => item.GrantedOnSecurityEntityType == securityEntityType);
    }

    /// <summary>
    /// Returns true if the user has the specified permission on the specified entity.
    /// </summary>
    public bool HasPermission(SecurityEntityType securityEntityType, Permission permission)
    {
        if (UserPermissions.Any(permissionKey =>
            permissionKey.GrantedOnSecurityEntityType == securityEntityType &&
            permissionKey.Permission == permission))
        {
            // Exact match
            return true;
        }

        // Has admin for that permission
        var entitySet = GetGrantedEntities(permission);

        if (entitySet.HasAdmin)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the current user has the specified admin permission.
    /// </summary>
    public bool HasPermission(AdministratorPermission permission)
    {
        return HasPermission(SecurityEntityType.Administrator, (Permission)permission);
    }

    /// <summary>
    /// Returns true if the current user has the specified user-level permission.
    /// </summary>
    public bool HasPermission(UserPermission permission)
    {
        return HasPermission(SecurityEntityType.User, (Permission)permission);
    }

    private void Require(SecurityEntityType securityEntityType, Permission permission)
    {
        if (!HasPermission(securityEntityType, permission))
        {
            throw SecurityExceptions.MissingRequiredPermission(permission, securityEntityType);
        }
    }

    /// <summary>
    /// Checks whether the user has the specified admin-permission and throws an exception if it is not found.
    /// </summary>
    public void Require(AdministratorPermission permission)
    {
        Require(SecurityEntityType.Administrator, (Permission)permission);
    }

    /// <summary>
    /// Checks whether the user has the specified (current-)user-level-permission and throws an exception if it is not found.
    /// </summary>
    public void Require(UserPermission permission)
    {
        Require(SecurityEntityType.User, (Permission)permission);
    }

    /// <summary>
    /// Checks whether the user has the specified (current-)user-level-permission for all countries and throws an exception if it is not found.
    /// </summary>
    public void Require(UserPermission[] permissions)
    {
        foreach (var permission in permissions)
        {
            Require(SecurityEntityType.User, (Permission)permission);
        }
    }

    public bool IsAdmin()
    {
        return UserPermissions.Any(permissionKey => permissionKey.GrantedOnSecurityEntityType == SecurityEntityType.Administrator);
    }
}