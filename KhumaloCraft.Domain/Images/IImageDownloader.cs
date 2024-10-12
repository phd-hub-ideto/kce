namespace KhumaloCraft.Domain.Images;

public interface IImageDownloader
{
    public Task<(bool, ImageDownload)> TryDownloadIfNotModifiedAsync(string imageUrl, DateTimeOffset? lastModified, long? minContentLength, long? maxContentLength);
}