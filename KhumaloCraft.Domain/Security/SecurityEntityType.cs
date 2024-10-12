namespace KhumaloCraft.Domain.Security;

public enum SecurityEntityType
{
    /// <summary>
    /// Administrator-Level Permissions are System-Wide
    /// </summary>
    Administrator = 1,

    /// <summary>
    /// User-Level Permissions apply to a user's own account.
    /// </summary>
    User = 2
}