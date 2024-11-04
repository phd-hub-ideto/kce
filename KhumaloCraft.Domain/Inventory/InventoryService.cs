using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Orders;
using System.Transactions;

namespace KhumaloCraft.Domain.Inventory;

[Singleton(Contract = typeof(IInventoryService))]
public class InventoryService(
    ICraftworkRepository craftworkRepository,
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    ICartItemRepository cartItemRepository) : IInventoryService
{
    public async Task UpdateInventory(int orderId)
    {
        var order = orderRepository.Query().Single(o => o.Id == orderId);

        var cart = cartRepository.Query().Single(c => c.Id == order.CartId);

        var cartItems = cartItemRepository.Query().Where(c => c.CartId == cart.Id).ToList();

        using var scope = new TransactionScope();

        foreach (var item in cartItems)
        {
            var craftwork = craftworkRepository.Query().Single(c => c.Id == item.CraftworkId);

            if (craftwork.TryRemoveQuantity(item.Quantity))
            {
                craftworkRepository.UpsertCraftworkQuantity(craftwork.Id.Value, craftwork.Quantity);
            }
        }

        scope.Complete();
    }
}