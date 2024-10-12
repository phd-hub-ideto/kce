using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;

namespace KhumaloCraft.Application.ImageHelpers;

public class ImageUrlBuilder : IImageUrlBuilder
{
    private readonly IImageReferenceCache _imageReferenceCache;
    private readonly ISettings _settings;
    private static Uri _imageServerUriForImageId;

    public ImageUrlBuilder(IImageReferenceCache imageReferenceCache, ISettings settings)
    {
        _imageReferenceCache = imageReferenceCache;
        _settings = settings;
    }

    public string GetUrl(int? imageReferenceId, ImageSizeOption imageSize = ImageSizeOption.Original)
    {
        if (imageReferenceId != null)
        {
            return GetUrl(imageReferenceId.Value, imageSize);
        }

        return null;
    }

    public string GetUrl(int imageReferenceId, ImageSizeOption imageSize = ImageSizeOption.Original)
    {
        if (_imageReferenceCache.TryGetImageId(imageReferenceId, out var imageId))
        {
            return GetUrl(imageId, imageSize, _settings);
        }
        else
        {
            var uriBuilder = new UriBuilder(_settings.PortalBaseUri);
            return uriBuilder.Uri.AbsoluteUri;
        }
    }

    public string GetUrl(ImageId? imageId, ImageSizeOption imageSize = ImageSizeOption.Original)
    {
        if (imageId != null)
        {
            return GetUrl(imageId.Value, imageSize, _settings);
        }
        return null;
    }

    private static string GetUrl(ImageId imageId, ImageSizeOption imageSize, ISettings settings)
    {
        var uriBuilder = new UriBuilder(ImageServerUri(settings));

        uriBuilder.Path += imageId;

        if (imageSize != ImageSizeOption.Original)
        {
            uriBuilder.Path += "/" + imageSize.ToString();
        }
        return uriBuilder.Uri.AbsoluteUri;
    }

    private static Uri ImageServerUri(ISettings settings)
    {

        if (_imageServerUriForImageId == null)
        {
            var uriBuilder = new UriBuilder(settings.ImageServerBaseUri);

            _imageServerUriForImageId = uriBuilder.Uri;
        }

        return _imageServerUriForImageId;
    }

    public string GetUrl(ImageReference imageReference, ImageSizeOption imageSize = ImageSizeOption.Original)
    {
        return GetUrl(imageReference?.ImageId, imageSize);
    }
}