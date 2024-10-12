using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public sealed class ViewCraftworkModel
{
    public string CreatedDate { get; set; }
    public string UpdatedDate { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public bool IsActive { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }

    public static ViewCraftworkModel Create(
        Craftwork craftwork,
        IImageUrlBuilder imageUrlBuilder)
    {
        return new ViewCraftworkModel
        {
            CreatedDate = FormattingHelper.FormatDateTime(craftwork.CreatedDate),
            UpdatedDate = FormattingHelper.FormatDateTime(craftwork.UpdatedDate),
            Title = craftwork.Title,
            Description = craftwork.Description,
            Price = craftwork.Price.ToStringWithDecimals(),
            Category = craftwork.Category.GetBestDescription(),
            IsActive = craftwork.IsActive,
            Quantity = craftwork.Quantity,
            ImageUrl = imageUrlBuilder.GetUrl(craftwork.PrimaryImageReferenceId)
        };
    }
}