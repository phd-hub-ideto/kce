using KhumaloCraft.Domain.Craftworks;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public sealed class CreateCraftworkModel
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public CraftworkCategory Category { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [Display(Name = "Image")]
    public int? ImageReferenceId { get; set; }

    [Required]
    public int? Quantity { get; set; }
}