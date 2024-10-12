using System.Collections.Concurrent;
using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Domain.Images;

[Singleton]
public class ImageReferenceCache : IImageReferenceCache
{
    private readonly ConcurrentDictionary<int, ImageId> _imageIdByImageReferenceId;
    private readonly IImageRepository _imageRepository;

    public ImageReferenceCache(IImageRepository imageRepository)
    {
        _imageIdByImageReferenceId = new ConcurrentDictionary<int, ImageId>();
        _imageRepository = imageRepository;
    }

    public bool TryGetImageId(int imageReferenceId, out ImageId imageId)
    {
        imageId = _imageIdByImageReferenceId.GetOrAdd(imageReferenceId, GetImageId);
        return imageId.Value != default;
    }

    private ImageId GetImageId(int referenceId)
    {
        if (_imageRepository.TryGetImageIdByReferenceId(referenceId, out var imageId))
        {
            return imageId;
        }
        return default;
    }
}