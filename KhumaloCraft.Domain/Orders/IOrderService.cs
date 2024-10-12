namespace KhumaloCraft.Domain.Orders;

public interface IOrderService
{
    bool PlaceOrder(out int orderId);
    bool TryUpdateOrder(Order order);
    IEnumerable<Order> FetchUserOrders();
    IEnumerable<Order> FetchOrders();
    bool TryFetchOrder(int orderId, out Order order);
}