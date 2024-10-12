using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

public class ImageData
{
    public byte[] Data { get; private set; }

    public DateTime ModifiedDate { get; private set; }

    internal ImageContentType _contentType = ImageContentType.Undefined;

    public ImageContentType ContentType
    {
        get
        {
            if (ImageContentType.Undefined == _contentType)
            {
                if (null != Data)
                    _contentType = ImagingUtilities.GetImageContentType(Data);
            }

            return _contentType;
        }
    }

    public ImageData(byte[] data, DateTime modifiedDate = default, ImageContentType contentType = ImageContentType.Undefined)
    {
        Data = data;
        if (default == modifiedDate)
            ModifiedDate = DateTime.Now;
        else
            ModifiedDate = modifiedDate;
        _contentType = contentType;
    }

    public ImageData New(byte[] data)
    {
        return new ImageData(data, ModifiedDate, ContentType);
    }
}