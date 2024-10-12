using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Domain.Carts;

[Singleton(Contract = typeof(ICartService))]
public sealed class CartService(
    IPrincipalResolver principalResolver,
    ICartRepository cartRepository,
    ICartItemRepository cartItemRepository,
    ICraftworkRepository craftworkRepository) : ICartService
{
    //TODO-LP : Implement strict validations and custom error messages

    public Cart CreateUserCart()
    {
        var cart = new Cart
        {
            IsActive = true
        };

        cartRepository.Upsert(cart);

        return cart;
    }

    public Cart Fetch()
    {
        //TODO-LP : Performance improvemt - Load everything related to a cart at once
        // Cart, CartItem, Craftwork
        var userId = principalResolver.GetRequiredUserId();

        var cart = cartRepository.Query()
                        .SingleOrDefault(x => x.UserId == userId && x.IsActive);

        if (cart == null)
        {
            //Create a cart for the user. Everyone gets a cart

            cart = CreateUserCart();
        }

        var cartItems = cartItemRepository.Query()
                            .Where(c => c.CartId == cart.Id)
                            .ToList();

        if (cartItems.Any())
        {
            var craftworkIds = cartItems.Select(i => i.CraftworkId).ToList();

            var craftworksLookup = craftworkRepository.Query()
                                        .Where(c => craftworkIds.Contains(c.Id.Value))
                                        .ToLookup(k => k.Id.Value);

            foreach (var cartItem in cartItems)
            {
                cartItem.Craftwork = craftworksLookup[cartItem.CraftworkId].Single();
            }

            cart.CartItems.AddRange(cartItems);
        }

        return cart;
    }

    public bool AddItemToCart(int craftworkId)
    {
        var cart = GetUserCart();

        var cartItem = GetCartItem(cart.Id.Value, craftworkId);

        if (cartItem != null)
        {
            //You Can't Add Item to the cart multiple times
            return false;
        }

        cartItem = new CartItem
        {
            CartId = cart.Id.Value,
            CraftworkId = craftworkId,
            Quantity = 1 //Default to 1 item, until incremented
        };

        cartItemRepository.Upsert(cartItem);

        return true;
    }

    public bool RemoveItemFromCart(int craftworkId)
    {
        var cart = GetUserCart();

        var cartItem = GetCartItem(cart.Id.Value, craftworkId);

        if (cartItem == null)
        {
            //Item Not In The
            return false;
        }

        cartItemRepository.Delete(cartItem.Id.Value);

        return true;
    }

    public bool IncrementCartItem(int craftworkId, out int quantity)
    {
        var cart = GetUserCart();

        var cartItem = GetCartItem(cart.Id.Value, craftworkId);

        if (cartItem == null)
        {
            //Item Not In The
            quantity = default;

            return false;
        }

        cartItem.IncrementQuantity();

        cartItemRepository.Upsert(cartItem);

        quantity = cartItem.Quantity;

        return true;
    }

    public bool DecrementCartItem(int craftworkId, out int quantity)
    {
        var cart = GetUserCart();

        var cartItem = GetCartItem(cart.Id.Value, craftworkId);

        if (cartItem == null)
        {
            //Item Not In The
            quantity = default;

            return false;
        }

        cartItem.DecrementQuantity();

        if (cartItem.Quantity <= 0)
        {
            cartItemRepository.Delete(cartItem.Id.Value);
        }
        else
        {
            cartItemRepository.Upsert(cartItem);
        }

        quantity = cartItem.Quantity;

        return true;
    }

    public bool TryGetUserCart(out Cart cart)
    {
        cart = cartRepository.Query()
                .SingleOrDefault(c =>
                    c.UserId == principalResolver.GetRequiredUserId() &&
                    c.IsActive);

        return cart != null;
    }

    public List<Cart> GetUserCarts()
    {
        return cartRepository.Query()
                        .Where(c =>
                            c.UserId == principalResolver.GetRequiredUserId())
                        .ToList();
    }

    private Cart GetUserCart()
    {
        var cart = cartRepository.Query()
                .Single(c =>
                    c.UserId == principalResolver.GetRequiredUserId() &&
                    c.IsActive);

        return cart;
    }

    private CartItem GetCartItem(int cartId, int craftworkId)
    {
        var cartItem = cartItemRepository.Query()
                        .SingleOrDefault(c =>
                            c.CartId == cartId &&
                            c.CraftworkId == craftworkId);

        return cartItem;
    }
}