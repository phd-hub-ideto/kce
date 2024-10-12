using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("PermissionSecurityEntityType")]
public class DalPermissionSecurityEntityType
{
    [Required]
    public int PermissionId { get; set; }

    [Required]
    public int SecurityEntityTypeId { get; set; }

    public DalPermission Permission { get; set; }
    public DalSecurityEntityType SecurityEntityType { get; set; }
}