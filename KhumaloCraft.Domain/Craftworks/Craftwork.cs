using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Domain.Craftworks;

public sealed class Craftwork
{
    public int? Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Money Price { get; set; }
    public int PrimaryImageReferenceId { get; set; }
    public string ImageUrl { get; set; }
    public CraftworkCategory Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int LastEditedByUserId { get; set; }
    public bool IsNew => !Id.HasValue;

    private List<CraftworkImage> _craftworkImages = [];

    public List<CraftworkImage> CraftworkImages
    {
        get
        {
            return _craftworkImages;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("Cannot set Images to null, use empty list instead");
            }

            _craftworkImages = value;
        }
    }

    public int Quantity { get; set; }

    public Craftwork() { }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public bool TryRemoveQuantity(int quantity)
    {
        if (Quantity >= quantity)
        {
            Quantity -= quantity;

            return true;
        }

        return false;
    }
}