namespace KhumaloCraft.Domain.Images;

public interface IImageRepository
{
    ImageReference AddImage(ImageData imageData);
    ImageReference[] AddImages(IEnumerable<ImageData> datas);
    int Delete(IEnumerable<ImageReference> referenceIds);
    List<ImageReference> FetchDeletedReferences(int count, DateTime deletedBefore);
    void Reference(IEnumerable<int> referenceIds);
    bool TryGetImageIdByReferenceId(int referenceId, out ImageId imageId);
    bool TryGetImageByImageId(ImageId imageId, out ImageData imageData);
    Task<(bool result, ImageData imageData)> TryGetImageByImageIdAsync(ImageId imageId);
    ImageId FetchOneImageId();
    void Unreference(IEnumerable<int> referenceIds);
    bool IsReferenced(int referenceId);
    ImageReference AddImageReferenceIdToImageId(ImageId imageId);

    (int Width, int Height) GetImageDimensionsById(ImageId imageId);
}