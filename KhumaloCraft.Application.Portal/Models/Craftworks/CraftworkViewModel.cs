namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public class CraftworkViewModel
{
    public IEnumerable<CraftworkModel> CraftworkModels { get; set; }
    public IEnumerable<CraftworkCartItemModel> CartItems { get; set; }

    public static CraftworkViewModel Create(
        Domain.Carts.Cart cart,
        IEnumerable<CraftworkModel> craftworkModels)
    {
        return new CraftworkViewModel
        {
            CraftworkModels = craftworkModels,
            CartItems = cart.CartItems.Select(CraftworkCartItemModel.Create)
        };
    }
}