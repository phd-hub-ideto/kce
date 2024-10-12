using KhumaloCraft.Domain.Craftworks;

namespace KhumaloCraft.Domain.Carts;

public class CartItem
{
    public int? Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int CartId { get; set; }
    public int CraftworkId { get; set; }
    public int Quantity { get; set; }
    public Craftwork Craftwork { get; set; }
    public bool IsNew => !Id.HasValue;

    public void IncrementQuantity()
    {
        Quantity += 1;
    }

    public void DecrementQuantity()
    {
        Quantity -= 1;
    }
}