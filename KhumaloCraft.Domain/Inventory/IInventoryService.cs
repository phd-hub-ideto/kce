namespace KhumaloCraft.Domain.Inventory;

public interface IInventoryService
{
    Task UpdateInventory(int orderId);
}