namespace KhumaloCraft.Domain.Security;

public class SecurityEntitySet
{
    private SecurityEntitySet() { }

    public static SecurityEntitySet Create(Permission permission, IEnumerable<PermissionKey> userPermissions)
    {
        var entities = new SecurityEntitySet();

        entities.Permission = permission;

        entities.HasAdmin = userPermissions.Any(
                                permissionKey => permissionKey.Permission == permission &&
                                permissionKey.GrantedOnSecurityEntityType == SecurityEntityType.Administrator);

        entities.HasUser = userPermissions.Any(
                                permissionKey => permissionKey.Permission == permission &&
                                permissionKey.GrantedOnSecurityEntityType == SecurityEntityType.User);

        return entities;
    }

    public Permission Permission { get; set; }

    public bool HasAdmin { get; set; }
    public bool HasUser { get; set; }

    private static bool Exists(HashSet<int> ids, int? id)
    {
        return id.HasValue && ids?.Contains(id.Value) == true;
    }
}