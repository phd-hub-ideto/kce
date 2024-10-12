namespace KhumaloCraft.Domain.Images;

public interface IImageBlobRepository
{
    ImageId AddImage(ImageData imageData);

    bool TryGetImageByImageId(ImageId imageId, out ImageData imageData);

    Task<(bool result, ImageData imageData)> TryGetImageByImageIdAsync(ImageId imageId);

    void DeleteImages(IEnumerable<ImageId> imageIds);

    List<ImageIdAndSize> FetchAllImagesSavedToAzureWithSize();

    void ClaimOrphanedBlob(ImageId imageId, ImageData imageData);
}