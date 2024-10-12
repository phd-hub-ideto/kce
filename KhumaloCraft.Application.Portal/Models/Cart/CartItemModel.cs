using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Application.Portal.Models.Cart;

public class CartItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Money Price { get; set; }
    public Money Total => new Money(Quantity * Price.Amount);

    public static CartItemModel Create(CartItem cartItem)
    {
        return new CartItemModel
        {
            Id = cartItem.Id.Value,
            Name = cartItem.Craftwork.Title,
            Quantity = cartItem.Quantity,
            Price = cartItem.Craftwork.Price
        };
    }
}