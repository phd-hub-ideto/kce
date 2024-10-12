using System.Security;

namespace KhumaloCraft.Domain.Security;

public static class SecurityExceptions
{
    public static SecurityException AccessDenied()
    {
        return new SecurityException("Access is denied. Please ensure that the correct credentials are supplied.");
    }

    public static SecurityException AccessDeniedNoCountries()
    {
        return new SecurityException("Access is denied. No permissable countries were found for the user.");
    }

    public static SecurityException NoLoggedInUser()
    {
        return new SecurityException("Access is denied: There is no logged in user.");
    }

    public static SecurityException NotAuthenticated()
    {
        return new SecurityException("Invalid email or password.");
    }

    public static SecurityException AccountNotActivated()
    {
        return new SecurityException("User not active.");
    }

    public static Exception MissingRequiredPermission(Permission permission, SecurityEntityType securityEntityType)
    {
        var message = string.Format(
            "Access is denied: The required permission '{0}' on Entity Type: '{1}' is not granted to the current user.",
            permission.ToString(),
            securityEntityType.ToString());

        return new MissingPermissionSecurityException(message, permission, securityEntityType);
    }

    public static SecurityException CountryAccessRestricted(string countryId)
    {
        return new SecurityException(string.Format("The current user has no access for Country '{0}' in this context.", countryId));
    }
}

[Serializable]
public class MissingPermissionSecurityException : SecurityException
{
    public MissingPermissionSecurityException(string message, Permission permission, SecurityEntityType entityType)
        : base(message)
    {
        Permission = permission;
        EntityType = entityType;
    }

    public Permission Permission { get; private set; }
    public SecurityEntityType EntityType { get; private set; }
}