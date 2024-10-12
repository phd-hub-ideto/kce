using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Domain.Orders;

public class Order
{
    public int? Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int CartId { get; set; }
    public Money TotalAmount { get; set; }
    public double VatRate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string LastEditedBy { get; set; }
    public bool IsNew => !Id.HasValue;
    public bool ViewOnly
    {
        get
        {
            return OrderStatus == OrderStatus.Cancelled ||
                   OrderStatus == OrderStatus.Declined ||
                   OrderStatus == OrderStatus.Completed;
        }
    }
}