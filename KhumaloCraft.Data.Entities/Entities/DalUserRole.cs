using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("UserRole")]
public class DalUserRole
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int SecurityEntityTypeId { get; set; }

    [Required]
    public int UserId { get; set; }

    public int? LastEditedByUserId { get; set; }

    public DalRole Role { get; set; }
    public DalSecurityEntityType SecurityEntityType { get; set; }
    public DalUser User { get; set; }
    public DalUser LastEditedByUser { get; set; }
}