namespace KhumaloCraft.Domain.Security;

public class CurrentPermissionScopePermissionSource : IPermissionSource
{
    public int UserId => PermissionScope.Current.UserId;

    public IEnumerable<PermissionKey> UserPermissions => PermissionScope.Current.UserPermissions;
}