using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Domain.Users;

public interface IUserRolePermissionRepository
{
    List<PermissionKey> FetchByUserId(int userId);
}