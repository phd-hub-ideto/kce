namespace KhumaloCraft.Imaging;

public static class ImageContentTypeExtensions
{
    public static string ToMimeName(this ImageContentType type)
    {
        if (type.TryGetAttribute(out ImageContentTypeAttribute attribute))
            return attribute.MimeName;
        throw NotSupportedExceptionHelper.New(typeof(ImageContentType), type);
    }

    private static readonly HashSet<ImageContentType> _validContentTypes = new HashSet<ImageContentType>(
        Enum.GetValues(typeof(SupportedImageContentType)).Cast<ImageContentType>()
    );
    public static bool IsSupported(this ImageContentType type)
    {
        return _validContentTypes.Contains(type);
    }

    public static ImageContentType ToImageContentType(this SupportedImageContentType type)
    {
        switch (type)
        {
            case SupportedImageContentType.Jpeg: return ImageContentType.Jpeg;
            case SupportedImageContentType.Png: return ImageContentType.Png;
            case SupportedImageContentType.Gif: return ImageContentType.Gif;
        }
        throw NotSupportedExceptionHelper.New(typeof(SupportedImageContentType), type);
    }
}