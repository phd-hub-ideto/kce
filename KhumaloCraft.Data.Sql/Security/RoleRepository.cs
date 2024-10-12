using FluentValidation;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Events;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Security.Role;
using KhumaloCraft.Sync;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace KhumaloCraft.Data.Sql.Security;

[Singleton(Contract = typeof(IRoleRepository))]
public class RoleRepository : ValidatingRepositoryBase<Role>, IRoleRepository
{
    private readonly IDomainEventMediator _eventMediator;
    private readonly IPrincipalResolver _principalResolver;

    /* TODO-LP : Uncomment
    public RoleRepository(IEnumerable<IValidator<Role>> validators, IDomainEventMediator eventMediator, IPrincipalResolver principalResolver)
        : base(validators)
    {
        _eventMediator = eventMediator;
        _principalResolver = principalResolver;
    }*/
    public RoleRepository(IDomainEventMediator eventMediator, IPrincipalResolver principalResolver)
        : base([])
    {
        _eventMediator = eventMediator;
        _principalResolver = principalResolver;
    }

    private IQueryable<PermissionIdentifier> RolePermissionsQuery(IDalScope scope) =>
        from permission in scope.KhumaloCraft.Permission
        join permissionSecurityEntityType in scope.KhumaloCraft.PermissionSecurityEntityType on permission.Id equals permissionSecurityEntityType.PermissionId
        join rolePermission in scope.KhumaloCraft.RolePermission on
        new { permissionSecurityEntityType.PermissionId, permissionSecurityEntityType.SecurityEntityTypeId } equals
        new { rolePermission.PermissionId, rolePermission.SecurityEntityTypeId }
        orderby permission.Name ascending
        select new PermissionIdentifier()
        {
            PermissionId = permission.Id,
            Name = permission.Name,
            RoleId = rolePermission.RoleId,
            SecurityEntityTypeId = rolePermission.SecurityEntityTypeId,
        };

    private IQueryable<Role> RoleQuery(IDalScope scope) =>
        from role in scope.KhumaloCraft.Role
        select new Role
        {
            Id = role.Id,
            SecurityEntityTypeId = role.SecurityEntityTypeId,
            Name = role.Name
        };

    private Role FetchSingleRole(IDalScope scope, Expression<Func<Role, bool>> predicate)
    {
        var result = RoleQuery(scope).Where(predicate).Single();

        result.Permissions = RolePermissionsQuery(scope)
            .Where(p => p.RoleId == result.Id && p.SecurityEntityTypeId == result.SecurityEntityTypeId)
                .Select(p => new PermissionIdentifier()
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    RoleId = p.RoleId,
                    SecurityEntityTypeId = result.SecurityEntityTypeId
                }).ToList();

        return result;
    }

    public IQueryable<Role> Query() => QueryContainerFactory.Create(RoleQuery);

    public Role Fetch(int id)
    {
        using var scope = DalScope.Begin();

        return FetchSingleRole(scope, r => r.Id == id);
    }

    public Role FetchByName(string name)
    {
        using var scope = DalScope.Begin();

        return FetchSingleRole(scope, r => r.Name == name);
    }

    public bool TryFetchByName(string name, out Role role)
    {
        using var scope = DalScope.Begin();

        var result = RoleQuery(scope).SingleOrDefault(r => r.Name == name);

        if (result != null)
        {
            result.Permissions = RolePermissionsQuery(scope)
                .Where(p => p.RoleId == result.Id && p.SecurityEntityTypeId == result.SecurityEntityTypeId)
                .Select(p => new PermissionIdentifier()
                {
                    PermissionId = p.PermissionId,
                    Name = p.Name,
                    RoleId = p.RoleId,
                    SecurityEntityTypeId = result.SecurityEntityTypeId
                }).ToList();
        }

        role = result;

        return role != null;
    }

    public void Upsert(Role role)
    {
        if (!TryUpsert(role, out var validationResult))
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    protected override void DoUpsert(Role domainEntity)
    {
        using (var scope = DalScope.Begin())
        {
            DalRole dalRole = null;

            if (!domainEntity.IsNew)
            {
                dalRole = scope.KhumaloCraft.Role.SingleOrDefault(i => i.Id == domainEntity.Id.Value);

                if (dalRole is null)
                {
                    throw new Exception($"Role:Id|SecurityEntityTypeId={domainEntity.Id.Value}|{domainEntity.SecurityEntityTypeId.Value} not found");
                }
            }
            else
            {
                dalRole = new DalRole();
            }

            dalRole.Name = domainEntity.Name;
            dalRole.SecurityEntityTypeId = domainEntity.SecurityEntityTypeId.Value;

            var oldPermissions = domainEntity.IsNew ?
                new List<PermissionIdentifier>() :
                RolePermissionsQuery(scope)
                    .Where(p => p.RoleId == domainEntity.Id && p.SecurityEntityTypeId == domainEntity.SecurityEntityTypeId)
                    .Select(p => new PermissionIdentifier()
                    {
                        PermissionId = p.PermissionId,
                        Name = p.Name,
                        RoleId = p.RoleId,
                        SecurityEntityTypeId = domainEntity.SecurityEntityTypeId
                    }).ToList();

            GetMissingPermissions(oldPermissions, domainEntity.Permissions, out var missingPermissions, out var surplusPermissions);

            if (domainEntity.IsNew || missingPermissions.Any() || surplusPermissions.Any())
            {
                var lastEditedByUserId = _principalResolver.GetRequiredUserId();

                dalRole.LastEditedByUserId = lastEditedByUserId;
                //This is unfortunate - the only reason that this is here is to ensure that the dates are in sync between Role and RolePermission.
                //The entities perform a check for any changes when .Update() is called and do not update if no changes are detected.
                dalRole.UpdatedDate = scope.ServerDate();

                scope.KhumaloCraft.Update(dalRole);

                domainEntity.Id = dalRole.Id;

                if (missingPermissions.Any() || surplusPermissions.Any())
                {
                    SavePermissions(domainEntity.Id.Value, missingPermissions, surplusPermissions, scope, lastEditedByUserId);
                    //TODO-LP : Raise RolePermissionsChangedEvent
                    //_eventMediator.RaiseEvent(new RolePermissionsChangedEvent(domainEntity, lastEditedByUserId, missingPermissions, surplusPermissions));
                }

                scope.Commit();
            }
        }
    }

    private static readonly PermissionIdentifierEqualityComparer _permissionIdentifierEqualityComparer = new();
    private void GetMissingPermissions(
        IEnumerable<PermissionIdentifier> oldPermissions,
        IEnumerable<PermissionIdentifier> permissions,
        out IEnumerable<PermissionIdentifier> missingPermissions,
        out IEnumerable<PermissionIdentifier> surplusPermissions)
    {
        var filteredOldPermissions = from p in oldPermissions where (p.PermissionId.HasValue && p.SecurityEntityTypeId.HasValue) select p;
        var filteredNewPermissions = from p in permissions where (p.PermissionId.HasValue && p.SecurityEntityTypeId.HasValue) select p;

        SimpleCollectionSync.Sync(filteredNewPermissions, filteredOldPermissions, out var _, out surplusPermissions, out missingPermissions, _permissionIdentifierEqualityComparer);
    }

    private void SavePermissions(
        int roleId,
        IEnumerable<PermissionIdentifier> missingPerms,
        IEnumerable<PermissionIdentifier> surplusPerms,
        IDalScope scope,
        int lastEditedByUserId)
    {
        var permissionsToAdd = new List<DalRolePermission>(
            missingPerms.Select(missingPerm =>
            {
                var newPermEntry = new DalRolePermission
                {
                    PermissionId = missingPerm.PermissionId.Value,
                    SecurityEntityTypeId = missingPerm.SecurityEntityTypeId.Value,
                    RoleId = roleId,
                    LastEditedByUserId = lastEditedByUserId
                };

                return newPermEntry;
            })
        );

        scope.KhumaloCraft.RolePermission.AddRange(permissionsToAdd);

        foreach (var surplusPerm in surplusPerms)
        {
            Debug.Assert(surplusPerm.RoleId == roleId);

            var rolePermission = scope.KhumaloCraft.RolePermission.Single(r =>
                r.PermissionId == surplusPerm.PermissionId.Value &&
                r.SecurityEntityTypeId == surplusPerm.SecurityEntityTypeId.Value &&
                r.RoleId == surplusPerm.RoleId.Value);

            rolePermission.LastEditedByUserId = lastEditedByUserId;

            scope.KhumaloCraft.RolePermission.Update(rolePermission);

            scope.KhumaloCraft.RolePermission.Remove(rolePermission);
        }
    }

    private class PermissionIdentifierEqualityComparer : IEqualityComparer<PermissionIdentifier>
    {
        public bool Equals(PermissionIdentifier x, PermissionIdentifier y)
        {
            return x.PermissionId.Value == y.PermissionId.Value;
        }

        public int GetHashCode([DisallowNull] PermissionIdentifier obj)
        {
            return obj.PermissionId.Value.GetHashCode();
        }
    }
}