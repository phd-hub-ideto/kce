namespace KhumaloCraft.Domain.Images;

public static class ImageContentExtensions
{
    public static ImageData ToImageData(this ImageContent imageContent)
    {
        return new ImageData(imageContent.Data, imageContent.ModifiedDate ?? default, imageContent._contentType);
    }

    public static ImageContent ToImageContent(this ImageData imageData)
    {
        return new ImageContent(
            imageData.Data,
            imageData._contentType,
            imageData.ModifiedDate == default ? null : imageData.ModifiedDate);
    }
}