namespace KhumaloCraft.Domain.Images;

public interface IImageUpdater
{
    List<T> UpdateImagesWhereNecessary<T>(List<T> images, List<string> imageUrls) where T : IImage, new();
}