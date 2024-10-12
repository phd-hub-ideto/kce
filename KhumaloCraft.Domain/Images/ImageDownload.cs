using System.Net.Http.Headers;

namespace KhumaloCraft.Domain.Images;

public class ImageDownload
{
    public ImageDownload(byte[] data, DateTimeOffset? lastModifiedDate, MediaTypeHeaderValue contentType)
    {
        Data = data;
        LastModifiedDate = lastModifiedDate;
        ContentType = contentType;
    }

    public byte[] Data { get; set; }
    public DateTimeOffset? LastModifiedDate { get; }
    public MediaTypeHeaderValue ContentType { get; }
}
