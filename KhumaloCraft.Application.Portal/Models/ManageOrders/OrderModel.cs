using KhumaloCraft.Domain.Orders;
using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Application.Portal.Models.ManageOrders;

public class OrderModel
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int CartId { get; set; }
    public Money TotalAmount { get; set; }
    public string VatRate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string LastEditedBy { get; set; }
    public bool ViewOnly { get; set; }

    public static OrderModel Create(Order order, bool canProcessOrder)
    {
        return new OrderModel
        {
            Id = order.Id.Value,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            CartId = order.CartId,
            TotalAmount = order.TotalAmount,
            VatRate = order.VatRate.ToString("P"),
            OrderStatus = order.OrderStatus,
            LastEditedBy = order.LastEditedBy,
            ViewOnly = order.ViewOnly || !canProcessOrder
        };
    }
}