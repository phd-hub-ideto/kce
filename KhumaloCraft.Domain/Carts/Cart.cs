namespace KhumaloCraft.Domain.Carts;

public class Cart
{
    public int? Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public List<CartItem> CartItems { get; set; } = [];
    public bool IsNew => !Id.HasValue;

    public void Complete()
    {
        IsActive = false;
    }
}