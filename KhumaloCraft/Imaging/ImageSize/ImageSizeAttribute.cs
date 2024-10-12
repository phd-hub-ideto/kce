namespace KhumaloCraft.Imaging;

public class ImageSizeAttribute : Attribute
{
    internal ImageSizeAttribute(uint width, uint height)
    {
        ImageSize = new ImageSize(width, height);
    }

    internal ImageSizeAttribute(ImageResizeOption resizeOption, uint width, uint height)
    {
        ImageSize = new ImageSize(resizeOption, width, height);
    }

    public ImageSize ImageSize { get; private set; }
}