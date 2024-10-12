using KhumaloCraft.Data.Entities;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Data.Sql.Security;

[Singleton(Contract = typeof(IPermissionIdentifierRepository))]
public class PermissionIdentifierRepository : IPermissionIdentifierRepository
{
    public List<PermissionIdentifier> FetchBySecurityEntityType(SecurityEntityType securityEntityType)
    {
        using var scope = DalScope.Begin();

        return (from permission in scope.KhumaloCraft.Permission
                join permissionSecurityEntityType in scope.KhumaloCraft.PermissionSecurityEntityType on permission.Id equals permissionSecurityEntityType.PermissionId
                where permissionSecurityEntityType.SecurityEntityTypeId == (int)securityEntityType
                select new PermissionIdentifier(permission)).ToList();
    }
}