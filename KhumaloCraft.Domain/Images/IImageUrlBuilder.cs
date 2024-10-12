using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

public interface IImageUrlBuilder
{
    string GetUrl(int? imageReferenceId, ImageSizeOption imageSize = ImageSizeOption.Original);
    string GetUrl(ImageReference imageReference, ImageSizeOption imageSize = ImageSizeOption.Original);
    string GetUrl(int imageReferenceId, ImageSizeOption imageSize = ImageSizeOption.Original);
    string GetUrl(ImageId? imageId, ImageSizeOption imageSize = ImageSizeOption.Original);
}