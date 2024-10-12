using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Data.Entities.Entities;

[Table("Craftwork")]
public class DalCraftwork
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int CraftworkCategoryId { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public DateTime UpdatedDate { get; set; }

    [Required]
    public int LastEditedByUserId { get; set; }

    [Required]
    public int PrimaryImageReferenceId { get; set; }

    public DalCraftworkCategory CraftworkCategory { get; set; }
    public DalUser LastEditedByUser { get; set; }
    public DalImageReference ImageReference { get; set; }
}