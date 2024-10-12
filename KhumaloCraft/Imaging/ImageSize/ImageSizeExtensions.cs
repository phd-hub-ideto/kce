namespace KhumaloCraft.Imaging;

public static class ImageSizeExtensions
{
    public static ImageSize ToImageSize(this Enum @enum)
    {
        if (@enum.TryGetAttribute(out ImageSizeAttribute attribute))
            return attribute.ImageSize;

        throw NotSupportedExceptionHelper.New(@enum.GetType().Name, @enum);
    }

    public static ImageSize ToImageSize(this Enum @enum, ImageResizeOption resizeOption)
    {
        if (@enum.TryGetAttribute(out ImageSizeAttribute attribute))
        {
            var imageSize = attribute.ImageSize;
            return new ImageSize(resizeOption, imageSize.Width, imageSize.Height);
        }
        throw NotSupportedExceptionHelper.New(@enum.GetType().Name, @enum);
    }
}