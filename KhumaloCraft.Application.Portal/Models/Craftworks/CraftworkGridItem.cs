using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Controllers;
using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public sealed class CraftworkGridItem
{
    public int Id { get; set; }
    public string CreatedDate { get; set; }
    public string Title { get; set; }
    public string Price { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public string IsActive { get; set; }
    public string UpdatedDate { get; set; }
    public string ViewUrl { get; set; }

    public static CraftworkGridItem Create(Craftwork craftwork, IUrlHelper urlHelper)
    {
        return new CraftworkGridItem
        {
            Id = craftwork.Id.Value,
            CreatedDate = FormattingHelper.FormatDateTime(craftwork.CreatedDate),
            UpdatedDate = FormattingHelper.FormatDateTime(craftwork.UpdatedDate),
            Title = craftwork.Title,
            Price = craftwork.Price.ToStringWithDecimals(),
            Category = craftwork.Category.GetBestDescription(),
            Quantity = craftwork.Quantity,
            IsActive = craftwork.IsActive ? "Yes" : "No",
            ViewUrl = urlHelper.Action<CraftworkController>(c => c.Craftwork(craftwork.Id.Value))
        };
    }
}