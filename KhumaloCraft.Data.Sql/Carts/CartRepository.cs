using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Data.Sql.Carts;

[Singleton(Contract = typeof(ICartRepository))]
public sealed class CartRepository(
    IPrincipalResolver principalResolver) : ICartRepository
{
    public IQueryable<Cart> Query()
    {
        return QueryContainerFactory.Create(scope =>
            from cart in scope.KhumaloCraft.Cart
            select new Cart
            {
                Id = cart.Id,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate,
                UserId = cart.UserId,
                IsActive = cart.IsActive
            }
        );
    }

    public void Upsert(Cart cart)
    {
        using var scope = DalScope.Begin();

        DalCart dalCart;

        if (cart.IsNew)
        {
            dalCart = new DalCart
            {
                CreatedDate = scope.ServerDate(),
                UserId = principalResolver.GetRequiredUserId()
            };
        }
        else
        {
            dalCart = scope.KhumaloCraft.Cart.Single(c => c.Id == cart.Id.Value);
        }

        dalCart.IsActive = cart.IsActive;
        dalCart.UpdatedDate = scope.ServerDate();

        scope.KhumaloCraft.Cart.Update(dalCart);

        scope.Commit();

        cart.Id = dalCart.Id;
    }

    public void Delete(int cartId)
    {
        using var scope = DalScope.Begin();

        var dalCart = scope.KhumaloCraft.Cart.Single(c => c.Id == cartId);

        scope.KhumaloCraft.Cart.Remove(dalCart);

        scope.Commit();
    }
}