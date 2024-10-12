using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Orders;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Structs;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Data.Sql.Orders;

[Singleton(Contract = typeof(IOrderRepository))]
public sealed class OrderRepository(
    IPrincipalResolver principalResolver) : IOrderRepository
{
    public IQueryable<Order> Query()
    {
        return QueryContainerFactory.Create(scope =>
            from order in scope.KhumaloCraft.Order
            select new Order
            {
                Id = order.Id,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                CartId = order.CartId,
                TotalAmount = new Money(order.TotalAmount),
                VatRate = order.VatRate,
                OrderStatus = (OrderStatus)order.OrderStatusId,
                LastEditedBy = FormattingHelper.AsFullName(order.LastEditedByUser.FirstName, order.LastEditedByUser.LastName)
            }
        );
    }

    public void Upsert(Order order)
    {
        using var scope = DalScope.Begin();

        DalOrder dalOrder;

        if (order.IsNew)
        {
            dalOrder = new DalOrder
            {
                CreatedDate = scope.ServerDate(),
                CartId = order.CartId,
                TotalAmount = order.TotalAmount.Amount,
                VatRate = order.VatRate
            };
        }
        else
        {
            dalOrder = scope.KhumaloCraft.Order.Single(i => i.Id == order.Id.Value);
        }

        dalOrder.OrderStatusId = (int)order.OrderStatus;
        dalOrder.UpdatedDate = scope.ServerDate();
        dalOrder.LastEditedByUserId = principalResolver.GetRequiredUserId();

        scope.KhumaloCraft.Order.Update(dalOrder);

        scope.Commit();

        order.Id = dalOrder.Id;
    }
}