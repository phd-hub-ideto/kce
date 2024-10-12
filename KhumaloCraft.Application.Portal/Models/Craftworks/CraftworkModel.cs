using KhumaloCraft.Domain.Craftworks;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public sealed class CraftworkModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public string ImageUrl { get; set; }
    public CraftworkCategory Category { get; set; }
    public int Quantity { get; set; }
}