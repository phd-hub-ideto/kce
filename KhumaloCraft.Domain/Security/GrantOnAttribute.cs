namespace KhumaloCraft.Domain.Security;

/// <summary>
/// Applied to Permission enumeration-members to indicate that the permission is grantable on a given security-entity-type.
/// </summary>
/// <remarks>
/// Used by the Role-Management U.I. to decide which permissions may be part of a given role.
/// </remarks>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class GrantOnAttribute : Attribute
{
    public SecurityEntityType SecurityEntityType { get; private set; }

    public GrantOnAttribute(SecurityEntityType securityEntityType)
    {
        SecurityEntityType = securityEntityType;
    }

    public override string ToString()
    {
        return string.Format("GrantOnAttribute{{ SecurityEntityType={0} }}", SecurityEntityType);
    }
}