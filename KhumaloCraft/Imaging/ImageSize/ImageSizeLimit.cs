namespace KhumaloCraft.Imaging;

[Serializable]
public enum ImageSizeLimit
{
    [ImageSize(1920, 1080)]
    Default,
    [ImageSize(3840, 2160)]
    Double
}

public static class MinimumImageSizes
{
    public static readonly ImageSize PanoramicImage = new ImageSize(1024, 430);
}