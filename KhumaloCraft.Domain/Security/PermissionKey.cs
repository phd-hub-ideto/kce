namespace KhumaloCraft.Domain.Security;

[Serializable]
public sealed class PermissionKey
{
    public SecurityEntityType GrantedOnSecurityEntityType { get; }

    public Permission Permission { get; }

    public PermissionKey(UserPermission userPermission)
    {
        GrantedOnSecurityEntityType = SecurityEntityType.User;
        Permission = (Permission)userPermission;
    }

    public PermissionKey(AdministratorPermission administratorPermission)
    {
        GrantedOnSecurityEntityType = SecurityEntityType.Administrator;
        Permission = (Permission)administratorPermission;
    }

    public PermissionKey(SecurityEntityType securityEntityType, Permission permission)
    {
        GrantedOnSecurityEntityType = securityEntityType;
        Permission = permission;
    }

    internal static PermissionKey[] CreateFrom(IEnumerable<AdministratorPermission> administratorPermissions)
    {
        return administratorPermissions.Select(permission => new PermissionKey(permission)).ToArray();
    }

    public override string ToString()
    {
        return $"{Permission} - {GrantedOnSecurityEntityType}";
    }
}