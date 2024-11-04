using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Inventory;
using KhumaloCraft.Domain.Structs;
using System.Transactions;

namespace KhumaloCraft.Domain.Orders;

[Singleton(Contract = typeof(IOrderService))]
public sealed class OrderService(
    IOrderRepository orderRepository,
    ICartService cartService,
    ICartRepository cartRepository,
    IInventoryService inventoryService,
    ISettings settings) : IOrderService
{
    //TODO-LP : Implement strict validations and custom error messages
    public bool PlaceOrder(out int orderId)
    {
        var cart = cartService.Fetch();

        if (cart.CartItems.Any())
        {
            var vatRate = settings.VatRate;

            var totalBeforeVat = cart.CartItems.Sum(item =>
                                    item.Craftwork.Price.Amount * item.Quantity);

            var totalIncludingVat = totalBeforeVat + totalBeforeVat * vatRate;

            var order = new Order
            {
                CartId = cart.Id.Value,
                TotalAmount = new Money(totalIncludingVat),
                VatRate = (double)vatRate,
                OrderStatus = OrderStatus.Pending //Default order status
            };

            using var scope = new TransactionScope();

            orderRepository.Upsert(order);

            cart.Complete(); //Deactivate / close / complete the cart and we will create a new one for the user

            cartRepository.Upsert(cart);

            inventoryService.UpdateInventory(order.Id.Value);

            cartService.CreateUserCart();

            scope.Complete();

            orderId = order.Id.Value;

            return true;
        }

        orderId = default;

        return false;
    }

    public bool TryUpdateOrder(Order order)
    {
        var currentOrder = orderRepository.Query()
                                .SingleOrDefault(o => o.Id == order.Id);

        if (currentOrder != null)
        {
            var cart = cartRepository.Query()
                            .Single(c => c.Id == currentOrder.CartId);

            if (!cart.IsActive && !currentOrder.ViewOnly)
            {
                currentOrder.OrderStatus = order.OrderStatus;

                orderRepository.Upsert(currentOrder);

                return true;
            }
        }

        return false;
    }

    public IEnumerable<Order> FetchUserOrders()
    {
        var userCarts = cartService.GetUserCarts();

        return orderRepository.Query()
                    .Where(o => userCarts.Select(i => i.Id.Value).Contains(o.CartId))
                    .ToList();
    }

    public IEnumerable<Order> FetchOrders()
    {
        return orderRepository.Query().ToList();
    }

    public bool TryFetchOrder(int orderId, out Order order)
    {
        order = orderRepository.Query()
                    .SingleOrDefault(i => i.Id == orderId);

        return order != null;
    }
}