using Microsoft.Azure.Functions.Worker;

namespace KhumaloCraft.Application.DurableFunctions;

public class UpdateInventoryActivity
{
    [Function(nameof(UpdateInventory))]
    public async Task UpdateInventory([ActivityTrigger] int userId)
    {
        Console.WriteLine($"Updating inventory...");

        //await inventoryService.UpdateInventory(userId);
    }
}