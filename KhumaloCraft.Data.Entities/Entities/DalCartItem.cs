using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("CartItem")]
public class DalCartItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    public int CartId { get; set; }

    [Required]
    public int CraftworkId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public DalCart Cart { get; set; }
    public DalCraftwork Craftwork { get; set; }
}