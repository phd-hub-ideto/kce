using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.Application.DurableFunctions;

public class PlaceOrderActivity
{
    [Function(nameof(PlaceOrder))]
    public async Task<OrderResult> PlaceOrder([ActivityTrigger] int userId)
    {
        Console.WriteLine($"Placing order for user: {userId}");

        /*if (orderService.PlaceOrder(out var orderId))
        {
            return new OrderResult
            {
                Success = true,
                OrderId = orderId
            };
        }*/

        return new OrderResult
        {
            Success = false,
            Message = "Unexpected error occurred while placing the order."
        };
    }
}

public class OrderResult
{
    public int OrderId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}