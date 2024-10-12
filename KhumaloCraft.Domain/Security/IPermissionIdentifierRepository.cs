namespace KhumaloCraft.Domain.Security;

public interface IPermissionIdentifierRepository
{
    List<PermissionIdentifier> FetchBySecurityEntityType(SecurityEntityType securityEntityType);
}