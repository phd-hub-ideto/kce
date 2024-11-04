using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.Application.DurableFunctions;

public class ProcessPaymentActivity
{
    [Function(nameof(ProcessPayment))]
    public async Task<bool> ProcessPayment([ActivityTrigger] int orderId)
    {
        //Console.WriteLine($"Processing payment for order: {orderId}");

        // Implement payment processing logic here
        //return await _paymentService.ProcessPayment(orderId);

        return true;
    }
}