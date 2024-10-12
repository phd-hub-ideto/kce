using System.Net.Mime;

namespace KhumaloCraft.Imaging;

public static class ImageContentTypeMimeNames
{
    public const string Missing = "image/missing"; // The ImageId or ImageReferenceId refers to an image that does not exist.

    public const string Png = "image/png";
    public const string Xpng = "image/x-png";
    public const string Bmp = "image/bmp";
    public const string Pjpeg = "image/pjpeg";

    public const string Gif = MediaTypeNames.Image.Gif;
    public const string Jpeg = MediaTypeNames.Image.Jpeg;
    public const string Tiff = MediaTypeNames.Image.Tiff;
}

public static class ImageContentTypeFileExtensions
{
    public const string Png = ".png";
    public const string Xpng = ".png";
    public const string Bmp = ".bmp";

    public const string Gif = ".gif";
    public const string Jpeg = ".jpg";
    public const string Tiff = ".tif";
}

public class ImageContentTypeAttribute : Attribute
{
    public string MimeName { get; }
    public string FileExtension { get; }
    internal ImageContentTypeAttribute(string mimeName, string fileExtension)
    {
        MimeName = mimeName;
        FileExtension = fileExtension;
    }
}

[Serializable]
public enum ImageContentType
{
    [ImageContentType(null, null)]
    Undefined = 0,
    [ImageContentType(ImageContentTypeMimeNames.Jpeg, ImageContentTypeFileExtensions.Jpeg)]
    Jpeg = 1,
    [ImageContentType(ImageContentTypeMimeNames.Gif, ImageContentTypeFileExtensions.Gif)]
    Gif = 2,
    [ImageContentType(ImageContentTypeMimeNames.Png, ImageContentTypeFileExtensions.Png)]
    Png = 3,
    [ImageContentType(ImageContentTypeMimeNames.Tiff, ImageContentTypeFileExtensions.Tiff)]
    Tiff = 4,
    [ImageContentType(ImageContentTypeMimeNames.Bmp, ImageContentTypeFileExtensions.Bmp)]
    Bmp = 5,
    [ImageContentType(ImageContentTypeMimeNames.Missing, null)]
    Missing = 6
}

[Serializable]
public enum SupportedImageContentType
{
    [ImageContentType(ImageContentTypeMimeNames.Jpeg, ImageContentTypeFileExtensions.Jpeg)]
    Jpeg = ImageContentType.Jpeg,
    [ImageContentType(ImageContentTypeMimeNames.Gif, ImageContentTypeFileExtensions.Gif)]
    Gif = ImageContentType.Gif,
    [ImageContentType(ImageContentTypeMimeNames.Png, ImageContentTypeFileExtensions.Png)]
    Png = ImageContentType.Png
}
