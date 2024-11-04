using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Hubs;
using KhumaloCraft.Application.Portal.Models.ManageOrders;
using KhumaloCraft.Application.Portal.Models.UserOrders;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Application.Routing.Routes;
using KhumaloCraft.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class ManageOrdersController(
    IOrderService orderService,
    IKCRouteBuilder kCRouteBuilder,
    INotificationsService notificationsService) : BaseController
{
    [HttpGet]
    [Route("admin/orders", Name = RouteNames.ManageOrders.ViewOrders)]
    public ActionResult ViewOrders()
    {
        if (!HasAccess(Domain.Security.AdministratorPermission.ViewOrders))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var orders = orderService.FetchOrders()
                        .OrderByDescending(i => i.Id)
                        .ToList();

        var model = orders.Select(order => UserOrderModel.Create(order, Url, true)).ToList();

        return View(model);
    }

    [HttpGet]
    [Route("admin/order/{id:int}", Name = RouteNames.ManageOrders.ViewOrder)]
    public ActionResult ViewOrder(int id)
    {
        var canProcessOrder = HasAccess(Domain.Security.AdministratorPermission.ProcessOrder);

        if (!HasAccess(Domain.Security.AdministratorPermission.ViewOrders) &&
            !canProcessOrder)
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (!orderService.TryFetchOrder(id, out var order))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var model = OrderModel.Create(order, canProcessOrder);

        return View(model);
    }

    [HttpPost]
    [Route("admin/order/update", Name = RouteNames.ManageOrders.UpdateOrder)]
    public async Task<ActionResult> UpdateOrder([FromForm] OrderModel model)
    {
        if (!HasAccess(Domain.Security.AdministratorPermission.ProcessOrder))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var order = new Order
        {
            Id = model.Id,
            CartId = model.CartId,
            OrderStatus = model.OrderStatus
        };

        if (orderService.TryUpdateOrder(order))
        {
            await notificationsService.SendNotification(NotificationType.OrderUpdated, orderId: order.Id.Value);

            return Redirect(Url.Action<ManageOrdersController>(c => c.ViewOrder(order.Id.Value)));
        }

        return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
    }
}