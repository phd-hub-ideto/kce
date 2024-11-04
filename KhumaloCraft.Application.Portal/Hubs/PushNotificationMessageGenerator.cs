using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Carts;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Orders;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Structs;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Portal.Hubs;

[Singleton]
public class PushNotificationMessageGenerator(
    IPrincipalResolver principalResolver,
    ICraftworkService craftworkService,
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    ICartRepository cartRepository,
    IImageUrlBuilder imageUrlBuilder)
{
    public string GenerateNotificationMessage(
        NotificationType notificationType,
        int? craftworkId = null,
        int? orderId = null,
        Money? oldPrice = null)
    {
        switch (notificationType)
        {
            case NotificationType.OrderPlaced:
                return GenerateOrderPlacedMessage();
            case NotificationType.ItemPriceIncreased:
                return GenerateItemPriceIncreasedMessage(craftworkId.Value);
            case NotificationType.ItemPriceReduced:
                return GenerateItemPriceReducedMessage(craftworkId.Value, oldPrice.Value);
            case NotificationType.ItemDiscontinued:
                return GenerateItemDiscontinuedMessage(craftworkId.Value);
            case NotificationType.ItemInStock:
                return GenerateItemInStockMessage(craftworkId.Value);
            case NotificationType.ItemOutOfStock:
                return GenerateItemOutOfStockStockMessage(craftworkId.Value);
            case NotificationType.OrderUpdated:
                return GenerateOrderUpdatedMessage(orderId.Value);
            default:
                throw new NotImplementedException($"Not Implemented Notification Type: {notificationType.GetBestDescription()}");
        }
    }

    //TODO-LP : Send the notification to the relevent person and not all users on site
    private string GenerateOrderPlacedMessage()
    {
        var user = principalResolver.ResolveCurrentUser();

        return $"Hi <b>{user.Fullname}</b>,\n" +
            $"Thank you for placing an order.\n" +
            $"Kindly note we are busy processing it and it will be with you soon.";
    }

    private string GenerateItemPriceIncreasedMessage(int craftworkId)
    {
        var craftwork = craftworkService.FetchByCraftworkId(craftworkId);

        return $"Hi,\n" +
            $"Kindly note that price for one of our craftworks has just been increased.\n" +
            $"Title: <b>{craftwork.Title}</b>\n" +
            $"Category: <b>{craftwork.Category.GetBestDescription()}</b>\n" +
            $"New Price: <b>{craftwork.Price.ToStringWithDecimals()}</b>\n" +
            AddImage(craftwork.PrimaryImageReferenceId) +
            $"Thank You!";
    }

    private string GenerateItemPriceReducedMessage(int craftworkId, Money oldPrice)
    {
        var craftwork = craftworkService.FetchByCraftworkId(craftworkId);

        return $"Hi,\n" +
            $"Hurry up! One of our craftworks is on special.\n" +
            $"Was: <strike><b>{oldPrice.ToStringWithDecimals()}</b></strike>\n" +
            $"Now: <b>{craftwork.Price.ToStringWithDecimals()}</b>\n" +
            $"Title: <b>{craftwork.Title}</b>\n" +
            $"Category: <b>{craftwork.Category.GetBestDescription()}</b>\n" +
            AddImage(craftwork.PrimaryImageReferenceId) +
            $"Hurry while stock lasts!!!";
    }

    private string GenerateItemDiscontinuedMessage(int craftworkId)
    {
        var craftwork = craftworkService.FetchByCraftworkId(craftworkId);

        return $"Hi,\n" +
            $"Kindly note that one of our craftworks has just been discontinued.\n" +
            $"Title: <b>{craftwork.Title}</b>\n" +
            $"Category: <b>{craftwork.Category.GetBestDescription()}</b>\n" +
            AddImage(craftwork.PrimaryImageReferenceId) +
            $"We will let you know if it becomes available again.\n\n" +
            $"Thank You!";
    }

    private string GenerateItemInStockMessage(int craftworkId)
    {
        var craftwork = craftworkService.FetchByCraftworkId(craftworkId);

        return $"Hi,\n" +
            $"Kindly note that we have stock for one of our craftworks:\n" +
            $"Title: <b>{craftwork.Title}</b>\n" +
            $"Category: <b>{craftwork.Category.GetBestDescription()}</b>\n" +
            $"Price: <b>{craftwork.Price.ToStringWithDecimals()}</b>\n" +
            AddImage(craftwork.PrimaryImageReferenceId) +
            $"Hurry up and place your order while stock still lasts.\n\n" +
            $"Thank You!";
    }

    private string GenerateItemOutOfStockStockMessage(int craftworkId)
    {
        var craftwork = craftworkService.FetchByCraftworkId(craftworkId);

        return $"Hi,\n" +
            $"Unfortunately we have just ran out of stock for one of our craftworks:\n" +
            $"Title: <b>{craftwork.Title}</b>\n" +
            $"Category: <b>{craftwork.Category.GetBestDescription()}</b>\n" +
            AddImage(craftwork.PrimaryImageReferenceId) +
            $"We will let you know as soon as we have more stock.\n\n" +
            $"Thank You!";
    }

    //TODO-LP : Send the notification to the relevent person and not all users on site
    private string GenerateOrderUpdatedMessage(int orderId)
    {
        var order = orderRepository.Query().Single(o => o.Id == orderId);

        var cart = cartRepository.Query().Single(c => c.Id == order.CartId);

        var user = userRepository.Query().Single(u => u.Id == cart.UserId);

        return $"Hi {user.Fullname},\n" +
            $"Your Order <b>#{orderId}</b> status has just been updated.\n" +
            $"Order Status: <b>{order.OrderStatus.GetBestDescription()}</b>\n" +
            $"Total: <b>{order.TotalAmount.ToStringWithDecimals()}</b>\n\n" +
            $"Thank You!";
    }

    private string AddImage(int referenceId)
    {
        var imageUrl = imageUrlBuilder.GetUrl(referenceId); ;

        return
            $"<div id=\"imagePreview\" class=\"mt-3\">" +
                $"<img src=\"{imageUrl}\" alt=\"Image Preview\" style=\"max-width: 200px;\" />" +
            $"</div>";
    }
}