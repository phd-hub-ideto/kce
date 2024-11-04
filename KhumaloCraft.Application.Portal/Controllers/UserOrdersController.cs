using KhumaloCraft.Application.Portal.Hubs;
using KhumaloCraft.Application.Portal.Models.UserOrders;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class UserOrdersController(
    IOrderService orderService,
    INotificationsService notificationsService) : BaseController
{
    [HttpGet]
    [Route("orders", Name = RouteNames.UserOrders.Orders)]
    public ActionResult Orders()
    {
        var orders = orderService.FetchUserOrders()
                        .OrderByDescending(i => i.Id)
                        .ToList();

        var model = orders.Select(order => UserOrderModel.Create(order, Url)).ToList();

        return View(model);
    }

    [HttpPost]
    [Route("orders/place-order", Name = RouteNames.UserOrders.PlaceOrder)]
    public async Task<ActionResult> PlaceOrder()
    {
        if (orderService.PlaceOrder(out var orderId))
        {
            await notificationsService.SendNotification(NotificationType.OrderPlaced);

            //TODO-LP : Robust Solution? Don't want on the cart screen. Display the message when user is on orders screen.
            await Task.Delay(10000);

            return RedirectToAction("Orders");
        }

        return Json(new { success = false, message = "Failed to place order." });
    }
}