using FluentValidation.Results;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security.Role;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Domain.Security.Validators.UserRoles;

[Singleton]
public class UserRoleDeleteValidator : IUserRoleDeleteValidator
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;
    private readonly IPrincipalResolver _principalResolver;

    public UserRoleDeleteValidator(IRoleRepository roleRepository, IPermissionService permissionService, IPrincipalResolver principalResolver)
    {
        _roleRepository = roleRepository;
        _permissionService = permissionService;
        _principalResolver = principalResolver;
    }

    public bool CanDelete(UserRole userRole, out ValidationResult validationResult)
    {
        var role = _roleRepository.Fetch(userRole.RoleId);
        if (_permissionService.HasPermission(AdministratorPermission.UpdatePrivilegedPermissions)
            || _principalResolver.GetUserId() == userRole.UserId
            || !PrivilegedPermissions.HasPrivilegedPermissions(role.Permissions))
        {
            validationResult = new ValidationResult();

            return true;
        }

        validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure(nameof(UserRole.RoleId), "Given your current role, you are not permitted to remove users from this role."));

        return false;
    }
}