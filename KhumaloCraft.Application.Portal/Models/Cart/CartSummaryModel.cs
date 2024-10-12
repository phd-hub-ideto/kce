using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Application.Portal.Models.Cart;

public class CartSummaryModel
{
    public int Id { get; set; }
    public List<CartItemModel> Items { get; set; }
    public decimal VatRate { get; set; }
    public Money TotalBeforeVat => new Money(Items.Sum(item => item.Total.Amount));
    public Money Vat => new Money(TotalBeforeVat.Amount * VatRate);
    public Money Total => new Money(TotalBeforeVat.Amount + Vat.Amount);

    public static CartSummaryModel Create(
        Domain.Carts.Cart cart,
        ISettings settings)
    {
        return new CartSummaryModel
        {
            Id = cart.Id.Value,
            Items = CreateCartItems(cart.CartItems),
            VatRate = settings.VatRate
        };
    }

    private static List<CartItemModel> CreateCartItems(
        List<CartItem> cartItems)
    {
        return cartItems
                    .OrderBy(item => item.Id)
                    .Select(CartItemModel.Create)
                    .ToList();
    }
}