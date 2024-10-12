using KhumaloCraft.Domain.Carts;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public class CraftworkCartItemModel
{
    public int CraftworkId { get; set; }
    public int Quantity { get; set; }

    public static CraftworkCartItemModel Create(CartItem cartItem)
    {
        return new CraftworkCartItemModel
        {
            CraftworkId = cartItem.CraftworkId,
            Quantity = cartItem.Quantity,
        };
    }
}