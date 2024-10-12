namespace KhumaloCraft.Application.Models.Shared.Gallery;

public sealed class GalleryImageModel
{
    public string AltTag { get; }
    public string ImageUrl { get; }
    public string ThumbnailImageUrl { get; }
    public string GridImageUrl { get; }
    public string OriginalImageUrl { get; }
    public int ImageId { get; }

    public GalleryImageModel(int imageId, string altTag, string imageUrl, string thumbnailImageUrl, string gridImageUrl,
        string originalImageUrl = null)
    {
        ImageId = imageId;

        AltTag = altTag;
        ImageUrl = imageUrl;
        ThumbnailImageUrl = thumbnailImageUrl;
        GridImageUrl = gridImageUrl;
        OriginalImageUrl = originalImageUrl;
    }
}