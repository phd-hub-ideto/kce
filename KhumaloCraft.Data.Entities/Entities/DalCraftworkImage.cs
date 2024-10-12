using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("CraftworkImage")]
public class DalCraftworkImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CraftworkId { get; set; }

    [Required]
    public int ImageReferenceId { get; set; }

    public DalCraftwork Craftwork { get; set; }
    public DalImageReference ImageReference { get; set; }
}