namespace KhumaloCraft.Domain.Images;

public interface IImageReferenceCache
{
    bool TryGetImageId(int imageReferenceId, out ImageId imageId);
}
