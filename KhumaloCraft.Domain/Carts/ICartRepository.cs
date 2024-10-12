namespace KhumaloCraft.Domain.Carts;

public interface ICartRepository
{
    IQueryable<Cart> Query();
    void Upsert(Cart cart);
    void Delete(int cartId);
}