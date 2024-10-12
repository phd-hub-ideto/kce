namespace KhumaloCraft.Domain.Carts;

public interface ICartItemRepository
{
    IQueryable<CartItem> Query();
    void Upsert(CartItem cartItem);
    void Delete(int cartItemId);
    void DeleteByCartId(int cartId);
}