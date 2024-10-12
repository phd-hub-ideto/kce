using KhumaloCraft.Dependencies;
using KhumaloCraft.Imaging;
using SixLabors.ImageSharp;

namespace KhumaloCraft.Domain.Images;

[Singleton]
public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;

    public ImageService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public ImageContent FetchImage(int imageReferenceId)
    {
        if (_imageRepository.TryGetImageIdByReferenceId(imageReferenceId, out var imageId))
        {
            if (_imageRepository.TryGetImageByImageId(imageId, out ImageData imageData))
            {
                return imageData.ToImageContent();
            }
        }

        throw new InvalidOperationException($"Image with referenceid {imageReferenceId} not found.");
    }

    private class ImageInformation
    {
        public ImageData ImageData { get; }
        public ImageSize ImageSize { get; }

        public ImageInformation(ImageData imageData, ImageSize imageSize)
        {
            ImageData = imageData;
            ImageSize = imageSize;
        }
    }

    private ImageInformation GetImageInformation(ImageUpload imageUpload)
    {
        var imageData = new ImageData(imageUpload.Data);

        var imageSize = ImagingUtilities.GetImageSize(imageData.Data);

        return new ImageInformation(imageData, imageSize);
    }

    public AddImageResult AddImage(ImageUpload imageUpload, SupportedImageContentType? saveContentType)
    {
        if (imageUpload == null) throw new ArgumentNullException(nameof(imageUpload));

        var information = GetImageInformation(imageUpload);

        var imageReference = SaveImage(information.ImageData, ImageResizeOption.FitInside, ImageSizeLimit.Default, saveContentType);

        return new AddImageResult(imageReference.ReferenceId, information.ImageSize);
    }

    public AddImageResult[] AddImages(ImageUpload[] newImageUploads, SupportedImageContentType? saveContentType)
    {
        if (newImageUploads == null) throw new ArgumentNullException(nameof(newImageUploads));

        var imageInformations = new ImageInformation[newImageUploads.Length];
        for (int i = 0; i < newImageUploads.Length; i++)
            imageInformations[i] = GetImageInformation(newImageUploads[i]);

        var imageDatas = imageInformations.Select(i => i.ImageData);
        var imageReferences = SaveImages(imageDatas, ImageResizeOption.FitInside, ImageSizeLimit.Default, saveContentType);

        var results = new List<AddImageResult>();

        for (int i = 0; i < imageReferences.Length; i++)
        {
            var information = imageInformations[i];
            var imageReference = imageReferences[i];

            results.Add(new AddImageResult(imageReference.ReferenceId, information.ImageSize));
        }
        return results.ToArray();
    }

    private void EnsureImage(
        ref ImageData imageData,
        ImageResizeOption resizeOption,
        ImageSizeLimit sizeLimit,
        SupportedImageContentType? contentType)
    {
        ImageSize imageSize = sizeLimit.ToImageSize(resizeOption);

        var resizedData = ImagingUtilities.EnsureImage(imageData.Data, imageSize.Width, imageSize.Height, imageSize.ResizeOption.Value);

        var actualContentType = ImagingUtilities.GetImageContentType(resizedData);

        if (resizedData != imageData.Data || actualContentType != imageData.ContentType)
        {
            imageData = new ImageData(resizedData, imageData.ModifiedDate, actualContentType);
        }
    }

    public ImageReference SaveImage(ImageData imageData, ImageResizeOption resizeOption, ImageSizeLimit imageSizeLimit, SupportedImageContentType? saveContentType)
    {
        EnsureImage(ref imageData, resizeOption, imageSizeLimit, saveContentType);

        return _imageRepository.AddImage(imageData);
    }

    public ImageReference[] SaveImages(IEnumerable<ImageData> newImageDatas, ImageResizeOption resizeOption, ImageSizeLimit imageSizeLimit, SupportedImageContentType? contentType)
    {
        var imageDatas = newImageDatas.ToArray();

        for (int i = 0; i < imageDatas.Length; i++)
        {
            EnsureImage(ref imageDatas[i], resizeOption, imageSizeLimit, contentType);
        }

        return _imageRepository.AddImages(imageDatas);
    }

    public ImageReference CopyImageReference(int imageReferenceId)
    {
        if (!_imageRepository.TryGetImageIdByReferenceId(imageReferenceId, out var imageId))
        {
            throw new ArgumentException(message: $"Invalid {nameof(imageReferenceId)}: {imageReferenceId}", paramName: nameof(imageReferenceId));
        }
        return _imageRepository.AddImageReferenceIdToImageId(imageId);
    }
}
