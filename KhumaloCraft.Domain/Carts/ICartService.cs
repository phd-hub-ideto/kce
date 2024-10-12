namespace KhumaloCraft.Domain.Carts;

public interface ICartService
{
    Cart CreateUserCart();
    Cart Fetch();
    bool AddItemToCart(int craftworkId);
    bool RemoveItemFromCart(int craftworkId);
    bool IncrementCartItem(int craftworkId, out int quantity);
    bool DecrementCartItem(int craftworkId, out int quantity);
    bool TryGetUserCart(out Cart cart);
    List<Cart> GetUserCarts();
}