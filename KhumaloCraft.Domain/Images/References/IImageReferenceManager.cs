namespace KhumaloCraft.Domain.Images;

public interface IImageReferenceManager
{
    int Reference(int imageReferenceId);
    int? PersistReference(ImageHandle imageHandle);

    void UpdateReferenceCollection(IEnumerable<int> oldImageReferences, IEnumerable<int> newImageReferences);

    void Unreference(IEnumerable<int> imageReferenceId);
    void Unreference(ImageHandle imageHandle);
}