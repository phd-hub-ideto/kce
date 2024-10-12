using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Domain.Users;

public class UserRole : IEquatable<UserRole>
{
    public UserRole() { }

    public int? Id { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public int RoleId { get; set; }
    public int SecurityEntityTypeId { get; set; }

    public SecurityEntityType SecurityEntityType
    {
        get
        {
            return (SecurityEntityType)SecurityEntityTypeId;
        }
        set
        {
            SecurityEntityTypeId = (int)value;
        }
    }

    public string RoleName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? ActivatedDate { get; set; }
    public string MobileNumber { get; set; }
    public string Username { get; set; }
    public int? LastEditedByUserId { get; set; }
    public bool IsNew => !Id.HasValue;

    public bool Equals(UserRole other)
    {
        return other.RoleId.Equals(RoleId) && other.SecurityEntityTypeId.Equals(SecurityEntityTypeId);
    }
}