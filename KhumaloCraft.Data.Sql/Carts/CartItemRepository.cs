using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Carts;

namespace KhumaloCraft.Data.Sql.Carts;

[Singleton(Contract = typeof(ICartItemRepository))]
public sealed class CartItemRepository : ICartItemRepository
{
    public IQueryable<CartItem> Query()
    {
        return QueryContainerFactory.Create(scope =>
               from cartItem in scope.KhumaloCraft.CartItem
               select new CartItem
               {
                   Id = cartItem.Id,
                   CreatedDate = cartItem.CreatedDate,
                   UpdatedDate = cartItem.UpdatedDate,
                   CartId = cartItem.CartId,
                   CraftworkId = cartItem.CraftworkId,
                   Quantity = cartItem.Quantity
               }
         );
    }

    public void Upsert(CartItem cartItem)
    {
        using var scope = DalScope.Begin();

        DalCartItem dalCartItem;

        if (cartItem.IsNew)
        {
            dalCartItem = new DalCartItem
            {
                CreatedDate = scope.ServerDate(),
                CartId = cartItem.CartId,
                CraftworkId = cartItem.CraftworkId
            };
        }
        else
        {
            dalCartItem = scope.KhumaloCraft.CartItem.Single(c => c.Id == cartItem.Id.Value);
        }

        dalCartItem.UpdatedDate = scope.ServerDate();
        dalCartItem.Quantity = cartItem.Quantity;

        scope.KhumaloCraft.CartItem.Update(dalCartItem);

        scope.Commit();

        cartItem.Id = dalCartItem.Id;
    }

    public void Delete(int cartItemId)
    {
        using var scope = DalScope.Begin();

        var dalCartItem = scope.KhumaloCraft.CartItem.Single(c => c.Id == cartItemId);

        scope.KhumaloCraft.CartItem.Remove(dalCartItem);

        scope.Commit();
    }

    public void DeleteByCartId(int cartId)
    {
        using var scope = DalScope.Begin();

        scope.KhumaloCraft.CartItem.RemoveAll(c => c.CartId == cartId);

        scope.Commit();
    }
}