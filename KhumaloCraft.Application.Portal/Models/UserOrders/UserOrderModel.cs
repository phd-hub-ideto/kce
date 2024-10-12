using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Controllers;
using KhumaloCraft.Domain.Orders;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Models.UserOrders;

public class UserOrderModel
{
    public int Id { get; set; }
    public string CreatedDate { get; set; }
    public string UpdatedDate { get; set; }
    public string TotalAmount { get; set; }
    public string VatRate { get; set; }
    public string OrderStatus { get; set; }
    public string LastEditedBy { get; set; }
    public string ViewUrl { get; set; }

    public static UserOrderModel Create(
        Order order,
        IUrlHelper urlHelper,
        bool isAdmin = false)
    {
        return new UserOrderModel
        {
            Id = order.Id.Value,
            CreatedDate = FormattingHelper.FormatDateTime(order.CreatedDate),
            UpdatedDate = FormattingHelper.FormatDateTime(order.UpdatedDate),
            TotalAmount = order.TotalAmount.ToStringWithDecimals(),
            VatRate = order.VatRate.ToString("P"),
            OrderStatus = order.OrderStatus.GetBestDescription(),
            LastEditedBy = isAdmin ? order.LastEditedBy : null,
            ViewUrl = isAdmin ? urlHelper.Action<ManageOrdersController>(c => c.ViewOrder(order.Id.Value)) : null
        };
    }
}