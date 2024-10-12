using FluentValidation;
using FluentValidation.Results;
using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Security.Validators.UserRoles;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Data.Sql.Users;

[Singleton(Contract = typeof(IUserRoleRepository))]
public class UserRoleRepository : ValidatingRepositoryBase<UserRole>, IUserRoleRepository
{
    private readonly IPrincipalResolver _principalResolver;
    private readonly IUserRoleDeleteValidator _userRoleDeleteValidator;

    public UserRoleRepository(IPrincipalResolver principalResolver, IUserRoleDeleteValidator userRoleDeleteValidator)
        : base([])
    {
        _principalResolver = principalResolver;
        _userRoleDeleteValidator = userRoleDeleteValidator;
    }

    /* TODO-LP : Uncomment
    public UserRoleRepository(IPrincipalResolver principalResolver,
        IEnumerable<IValidator<UserRole>> validators, IUserRoleDeleteValidator userRoleDeleteValidator)
        : base(validators)
    {
        _principalResolver = principalResolver;
        _userRoleDeleteValidator = userRoleDeleteValidator;
    }*/

    public IQueryable<UserRole> Query()
    {
        return QueryContainerFactory.Create(scope =>
            from userRole in scope.KhumaloCraft.UserRole
            join role in scope.KhumaloCraft.Role on userRole.RoleId equals role.Id
            join user in scope.KhumaloCraft.User on userRole.UserId equals user.Id
            select new UserRole
            {
                Id = userRole.Id,
                UserId = userRole.UserId,
                UserFullName = FormattingHelper.AsFullName(user.FirstName, user.LastName),
                RoleId = userRole.RoleId,
                SecurityEntityTypeId = userRole.SecurityEntityTypeId,
                RoleName = role.Name,
                ActivatedDate = user.ActivatedDate,
                MobileNumber = user.MobileNumber,
                Username = user.Username,
                LastEditedByUserId = userRole.LastEditedByUserId
            }
        );
    }

    public void Upsert(UserRole userRole)
    {
        if (!TryUpsert(userRole, out var validationResult))
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    public bool TryDelete(int id, out ValidationResult validationResult)
    {
        using (var scope = DalScope.Begin())
        {
            var failures = new List<ValidationFailure>();

            var userRole = Query().Single(i => i.Id == id);

            if (_userRoleDeleteValidator.CanDelete(userRole, out validationResult))
            {
                var dalUserRole = scope.KhumaloCraft.UserRole.Single(i => i.Id == id);

                var lastEditedUserId = _principalResolver.GetUserId();

                dalUserRole.LastEditedByUserId = lastEditedUserId;

                scope.KhumaloCraft.Update(dalUserRole);

                scope.KhumaloCraft.Remove(dalUserRole);

                scope.Commit();

                return true;
            }

            return validationResult.IsValid;
        }
    }

    public void Delete(int id)
    {
        if (!TryDelete(id, out var validationResult))
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    public void DeleteUserRolesByUserId(int userId)
    {
        using (var scope = DalScope.Begin())
        {
            var userRoles = scope.KhumaloCraft.UserRole.Where(i => i.UserId == userId);

            foreach (var userRole in userRoles)
            {
                Delete(userRole.Id);
            }

            scope.Commit();
        }
    }

    public bool Exists(SecurityEntityType entityType, int roleId, int userId)
    {
        using var scope = DalScope.Begin();

        return scope.KhumaloCraft.UserRole.Any(ur =>
                        ur.UserId == userId &&
                        ur.RoleId == roleId &&
                        ur.SecurityEntityTypeId == (int)entityType);
    }

    public bool Exists(SecurityEntityType entityType, int roleId)
    {
        using var scope = DalScope.Begin();

        return scope.KhumaloCraft.UserRole.Any(ur =>
                    ur.RoleId == roleId &&
                    ur.SecurityEntityTypeId == (int)entityType);
    }

    public bool Exists(SecurityEntityType entityType, int userId, out int? userRoleId)
    {
        using var scope = DalScope.Begin();

        var dalUserRole = scope.KhumaloCraft.UserRole.SingleOrDefault(ur =>
                        ur.UserId == userId &&
                        ur.SecurityEntityTypeId == (int)entityType);

        if (dalUserRole is null)
        {
            userRoleId = null;

            return false;
        }

        userRoleId = dalUserRole.Id;

        return true;
    }

    protected override void DoUpsert(UserRole domainEntity)
    {
        using var scope = DalScope.Begin();

        var dalUserRole = !domainEntity.IsNew ? scope.KhumaloCraft.UserRole.Single(i => i.Id == domainEntity.Id.Value) : new DalUserRole();

        dalUserRole.RoleId = domainEntity.RoleId;
        dalUserRole.UserId = domainEntity.UserId;
        dalUserRole.SecurityEntityTypeId = domainEntity.SecurityEntityTypeId;
        var lastEditedUserId = _principalResolver.GetUserId();
        dalUserRole.LastEditedByUserId = lastEditedUserId;

        scope.KhumaloCraft.UserRole.Update(dalUserRole);

        domainEntity.Id = dalUserRole.Id;

        scope.Commit();
    }
}
