using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace KhumaloCraft.Application.DurableFunctions;

public class OrderOrchestrator
{
    [Function(nameof(OrderOrchestrator))]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var userId = context.GetInput<int>();

        // Step 1: Place Order for current user
        var orderResult = await context.CallActivityAsync<OrderResult>("PlaceOrder", userId);

        // Step 2: Update inventory
        await context.CallActivityAsync("UpdateInventory", orderResult.OrderId);

        // Step 3: Process payment
        var paymentResult = await context.CallActivityAsync<bool>("ProcessPayment", orderResult.OrderId);

        // Step 4: Send notifications
        //await context.CallActivityAsync("SendOrderNotification", orderResult.OrderId);
    }
}