using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Cart;

public class UpdateCartModel
{
    [Required]
    public int? CraftworkId { get; set; }
}