using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Structs;
using Microsoft.AspNetCore.SignalR;

namespace KhumaloCraft.Application.Portal.Hubs;

[Singleton(Contract = typeof(INotificationsService))]
public class NotificationsService(
    PushNotificationMessageGenerator messageGenerator,
    IHubContext<NotificationHub> hubContext) : INotificationsService
{
    public async Task SendNotification(
        NotificationType notificationType,
        int? craftworkId = null,
        int? orderId = null,
        Money? oldPrice = null)
    {
        var message = messageGenerator.GenerateNotificationMessage(notificationType, craftworkId, orderId, oldPrice);

        await hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task SendTestNotification(string message)
    {
        await hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }
}