namespace KhumaloCraft.Domain.Orders;

public interface IOrderRepository
{
    IQueryable<Order> Query();
    void Upsert(Order order);
}