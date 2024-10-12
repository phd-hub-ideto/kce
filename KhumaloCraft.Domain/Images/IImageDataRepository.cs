using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

public interface IImageDataRepository
{
    void Write(ImageId imageId, ImageData image);
    bool TryGet(ImageId imageId, out ImageData image);
    Task<(bool result, ImageData image)> TryGetAsync(ImageId imageId);
    Task DeleteAll(string branch);
    void Delete(ImageId id);
    IEnumerable<ImageIdAndSize> EnumerateAllOrderedByName();
}