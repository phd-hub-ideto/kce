namespace KhumaloCraft.Domain.Security;

public interface IPermissionService
{
    SecurityEntitySet GetGrantedEntities(Permission permission);
    bool HasAnyAdministrativePermission();
    bool HasAnyPermission(Permission permission);
    bool HasAnyPermission(SecurityEntityType securityEntityType);
    bool HasPermission(AdministratorPermission permission);
    bool HasPermission(UserPermission permission);
    bool IsAdmin();
    void Require(AdministratorPermission permission);
    void Require(UserPermission permission);
    void Require(UserPermission[] permissions);
}