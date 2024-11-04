using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Application.Portal.Hubs;

public interface INotificationsService
{
    Task SendNotification(NotificationType notificationType, int? craftworkId = null, int? orderId = null, Money? oldPrice = null);
    Task SendTestNotification(string message);
}