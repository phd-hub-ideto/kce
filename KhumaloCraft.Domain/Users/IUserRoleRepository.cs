using FluentValidation.Results;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Validation;

namespace KhumaloCraft.Domain.Users;

public interface IUserRoleRepository : IValidatingRepository<UserRole>
{
    IQueryable<UserRole> Query();
    void Upsert(UserRole userRole);
    bool TryDelete(int id, out ValidationResult validationResult);
    void Delete(int id);
    void DeleteUserRolesByUserId(int userId);
    bool Exists(SecurityEntityType entityType, int roleId, int userId);
    bool Exists(SecurityEntityType entityType, int roleId);
    bool Exists(SecurityEntityType entityType, int userId, out int? userRoleId);
}