namespace KhumaloCraft.Domain.Images;

[Serializable]
public class ImageDownloadContentTypeException : Exception
{
    public ImageDownloadContentTypeException()
    {
    }

    public ImageDownloadContentTypeException(string message) : base(message)
    {
    }

    public ImageDownloadContentTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}