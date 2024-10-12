using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("CraftworkQuantity")]
public class DalCraftworkQuantity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CraftworkId { get; set; }

    [Required]
    public int Count { get; set; }

    public DalCraftwork Craftwork { get; set; }
}