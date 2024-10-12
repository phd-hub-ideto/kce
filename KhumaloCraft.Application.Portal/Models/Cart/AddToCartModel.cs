using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Cart;

public class AddToCartModel
{
    [Required]
    public int? CraftworkId { get; set; }
}