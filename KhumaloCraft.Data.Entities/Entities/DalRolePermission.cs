using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("RolePermission")]
public class DalRolePermission
{
    [Required]
    public int RoleId { get; set; }

    [Required]
    public int PermissionId { get; set; }

    [Required]
    public int SecurityEntityTypeId { get; set; }

    [Required]
    public int LastEditedByUserId { get; set; }

    public DalRole Role { get; set; }
    public DalPermission Permission { get; set; }
    public DalSecurityEntityType SecurityEntityType { get; set; }
    public DalUser LastEditedByUser { get; set; }
}