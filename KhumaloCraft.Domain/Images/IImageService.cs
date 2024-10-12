using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

public interface IImageService
{
    ImageContent FetchImage(int imageReferenceId);

    ImageReference CopyImageReference(int imageReferenceId);

    AddImageResult AddImage(ImageUpload imageUpload, SupportedImageContentType? saveContentType);

    AddImageResult[] AddImages(ImageUpload[] newImageUploads, SupportedImageContentType? saveContentType);
}