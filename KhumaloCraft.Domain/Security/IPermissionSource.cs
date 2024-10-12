namespace KhumaloCraft.Domain.Security;

public interface IPermissionSource
{
    int UserId { get; }
    IEnumerable<PermissionKey> UserPermissions { get; }
}