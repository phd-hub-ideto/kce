using KhumaloCraft.Data.Entities;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Data.Sql.Users;

public sealed class UserRolePermissionRepository : IUserRolePermissionRepository
{
    public List<PermissionKey> FetchByUserId(int userId)
    {
        using var scope = DalScope.Begin();

        return [.. (from userRole in scope.KhumaloCraft.UserRole
                join rolePermission in scope.KhumaloCraft.RolePermission on userRole.RoleId equals rolePermission.RoleId
                where userRole.UserId == userId
                select new PermissionKey((SecurityEntityType)userRole.SecurityEntityTypeId, (Permission)rolePermission.PermissionId)
                )];
    }
}