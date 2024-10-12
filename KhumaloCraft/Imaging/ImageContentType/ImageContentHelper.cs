using KhumaloCraft.Helpers;

namespace KhumaloCraft.Imaging;

public static class ImageContentHelper
{
    public static ImageContentType GetImageContentType(string contentType)
    {
        foreach (var imageContentType in EnumHelper.GetValues<ImageContentType>())
        {
            var imageTypeAttribute = EnumHelper.TryGetAttribute<ImageContentTypeAttribute>(imageContentType);

            if (imageTypeAttribute != default && (imageTypeAttribute.MimeName?.Equals(contentType, StringComparison.InvariantCultureIgnoreCase) ?? false))
            {
                return imageContentType;
            }
        }
        return default;
    }
}