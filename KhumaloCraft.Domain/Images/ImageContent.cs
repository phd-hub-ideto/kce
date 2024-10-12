
using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

[Serializable]
public class ImageContent : ImageUpload
{
    public static readonly ImageContent Missing = new ImageContent([], ImageContentType.Missing, DateTime.MinValue);

    internal ImageContentType _contentType = ImageContentType.Undefined;

    public ImageContentType ContentType
    {
        get
        {
            /*TODO-LP : If needed, fix the content type
            if (ImageContentType.Undefined == _contentType)
            {
                if (null != Data)
                    _contentType = ImagingUtilities.GetImageContentType(Data);
            }*/
            return _contentType;
        }
    }

    public ImageContent()
    {
    }

    public ImageContent(byte[] data, ImageContentType contentType, DateTime? modifiedDate = null)
        : base(data, modifiedDate)
    {
        if (null == data)
            throw new ArgumentNullException(nameof(data));
        _contentType = contentType;
    }

    public ImageContent(byte[] data, DateTime? modifiedDate)
        : this(data, ImageContentType.Undefined, modifiedDate)
    {
    }

    public ImageContent(byte[] data)
        : this(data, ImageContentType.Undefined, null)
    {
    }
}