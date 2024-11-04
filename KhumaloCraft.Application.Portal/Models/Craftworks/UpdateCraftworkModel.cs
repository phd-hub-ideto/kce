using KhumaloCraft.Domain.Craftworks;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Helpers;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Craftworks;

public sealed class UpdateCraftworkModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public CraftworkCategory Category { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [Display(Name = "Image")]
    public int? ImageReferenceId { get; set; }

    [Required]
    public int? Quantity { get; set; }
    public string CreatedDate { get; set; }
    public string UpdatedDate { get; set; }
    public string ImageUrl { get; set; }

    public static UpdateCraftworkModel Create(
        Craftwork craftwork,
        IImageUrlBuilder imageUrlBuilder)
    {
        var model = new UpdateCraftworkModel
        {
            Id = craftwork.Id.Value,
            Title = craftwork.Title,
            Price = craftwork.Price.Amount,
            Description = craftwork.Description,
            Category = craftwork.Category,
            IsActive = craftwork.IsActive,
            ImageReferenceId = craftwork.PrimaryImageReferenceId,
            Quantity = craftwork.Quantity,
            CreatedDate = FormattingHelper.FormatDateTime(craftwork.CreatedDate),
            UpdatedDate = FormattingHelper.FormatDateTime(craftwork.UpdatedDate)
        };

        model.SetImageUrl(imageUrlBuilder);

        return model;
    }

    public void SetImageUrl(IImageUrlBuilder imageUrlBuilder)
    {
        ImageUrl = imageUrlBuilder.GetUrl(ImageReferenceId);
    }
}