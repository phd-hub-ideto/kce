using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Domain;

[Serializable]
public class PermissionIdentifier : IIdentifier
{
    public int? PermissionId { get; set; }

    public Permission? Permission
    {
        get { return (Permission?)PermissionId; }
        set { PermissionId = (int?)value; }
    }

    public int? SecurityEntityTypeId { get; set; }

    public SecurityEntityType? SecurityEntityType
    {
        get => (SecurityEntityType?)SecurityEntityTypeId;
        set
        {
            SecurityEntityTypeId = (int?)value;
        }
    }

    public string Name { get; set; }
    public int? RoleId { get; set; }

    int IIdentifier.Id => PermissionId.Value;
    string IIdentifier.Name => Name;

    public PermissionIdentifier() { }

    public PermissionIdentifier(int roleId, int securityEntityTypeId)
    {
        RoleId = roleId;
        SecurityEntityTypeId = securityEntityTypeId;
    }

    public PermissionIdentifier(SecurityEntityType securityEntityType, Permission permission)
    {
        PermissionId = (int)permission;
        SecurityEntityTypeId = (int)securityEntityType;
    }

    public PermissionIdentifier(AdministratorPermission permission)
    {
        PermissionId = (int)permission;
        SecurityEntityTypeId = (int)Security.SecurityEntityType.Administrator;
    }

    public static implicit operator PermissionIdentifier(AdministratorPermission p)
    {
        return new PermissionIdentifier(p);
    }

    public PermissionIdentifier(UserPermission permission)
    {
        PermissionId = (int)permission;
        SecurityEntityTypeId = (int)Security.SecurityEntityType.User;
    }

    public static implicit operator PermissionIdentifier(UserPermission p)
    {
        return new PermissionIdentifier(p);
    }

    internal PermissionIdentifier(DalPermission rhs)
    {
        PermissionId = rhs.Id;
        Name = rhs.Name;
    }

    public override string ToString()
    {
        return string.Format("{0}", Permission.HasValue ? Permission.ToString() : "null");
    }
}