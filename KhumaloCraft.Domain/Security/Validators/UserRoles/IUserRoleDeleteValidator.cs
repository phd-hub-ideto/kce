using FluentValidation.Results;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Domain.Security.Validators.UserRoles;

public interface IUserRoleDeleteValidator
{
    bool CanDelete(UserRole userRole, out ValidationResult validationResult);
}