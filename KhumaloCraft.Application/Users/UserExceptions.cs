using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Application.Users;

public static class Exceptions
{
    internal static Exception UserIsNotRegistered(string emailAddress)
    {
        return new UserNotRegisteredException($"Email address {emailAddress} is not registered.");
    }

    public static Exception UserAlreadyRegistered(string emailAddress)
    {
        return new Exception($"User {emailAddress} already exists.");
    }

    internal static RoleException RoleAlreadyExists(string userName, string roleName, SecurityEntityType entityType)
    {
        throw new RoleException($"User '{userName}' already has the role {roleName} for entity {entityType}.");
    }

    internal static Exception TokenIsInvalid()
    {
        throw new Exception("Security token is invalid.");
    }
}