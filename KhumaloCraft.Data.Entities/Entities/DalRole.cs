using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("Role")]
public class DalRole
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int SecurityEntityTypeId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    public int LastEditedByUserId { get; set; }

    public DalSecurityEntityType SecurityEntityType { get; set; }
    public DalUser LastEditedByUser { get; set; }
}