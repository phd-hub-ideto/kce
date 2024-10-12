namespace KhumaloCraft.Domain.Images;

[Serializable]
public class ImageDownloadException : Exception
{
    public ImageDownloadException()
    {
    }

    public ImageDownloadException(string message) : base(message)
    {
    }

    public ImageDownloadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}