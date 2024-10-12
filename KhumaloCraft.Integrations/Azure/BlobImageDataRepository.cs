using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Imaging;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Net;

namespace KhumaloCraft.Integrations.Azure;

[Singleton(Contract = typeof(IImageDataRepository))]
public class BlobImageDataRepository : BlobClientBase, IImageDataRepository
{
    private readonly BlobContainerClient _client;
    private const string ImagesContainer = "kc-images";

    public BlobImageDataRepository(ISettings settings)
        : base(settings)
    {

        if (string.IsNullOrEmpty(_settings.AzureAdCertThumbprint))
        {
            _client = ContainerClient();
        }
        else
        {
            _client = ContainerClient(settings.AzureBlobStoragePrimary, ImagesContainer);
        }
    }

    public void Write(ImageId imageId, ImageData image)
    {
        var name = GetBlobName(imageId);

        Write(_client, name, image);
    }

    private void Write(BlobContainerClient client, string name, ImageData image)
    {
        var blobClient = client.GetBlobClient(name);

        var headers = new BlobHttpHeaders()
        {
            ContentType = image.ContentType.ToMimeName(),
        };

        var options = new BlobUploadOptions()
        {
            HttpHeaders = headers,
            Conditions = new BlobRequestConditions
            {
                IfNoneMatch = new ETag("*") //same as overwrite = false
            }
        };

        try
        {
            blobClient.Upload(new BinaryData(image.Data), options);
        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.Conflict)
        {
            // Blob already exists, check for data equality
            if (TryGet(client, name, out var existingData))
            {
                if (ArrayExtensions.AreEqual(image.Data, existingData.Data))
                {
                    return;
                }
            }

            throw;
        }
    }

    public async Task WriteAsync(ImageId imageId, ImageData image)
    {
        var name = GetBlobName(imageId);

        await WriteAsync(_client, name, image).ConfigureAwait(false);
    }

    private async Task WriteAsync(BlobContainerClient client, string name, ImageData image)
    {
        var blobClient = client.GetBlobClient(name);

        var headers = new BlobHttpHeaders()
        {
            ContentType = image.ContentType.ToMimeName(),
        };

        var options = new BlobUploadOptions()
        {
            HttpHeaders = headers,
            Conditions = new BlobRequestConditions
            {
                IfNoneMatch = new ETag("*") //same as overwrite = false
            }
        };

        try
        {
            await blobClient.UploadAsync(new BinaryData(image.Data), options).ConfigureAwait(false);
        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.Conflict)
        {
            // Blob already exists, check for data equality
            var (result, existingData) = await TryGetAsync(client, name).ConfigureAwait(false);
            if (result)
            {
                if (ArrayExtensions.AreEqual(image.Data, existingData.Data))
                {
                    return;
                }
            }

            throw;
        }
    }

    public bool TryGet(ImageId imageId, out ImageData image)
    {
        var name = GetBlobName(imageId);

        return TryGet(_client, name, out image);
    }

    private static bool TryGet(BlobContainerClient client, string name, out ImageData image)
    {
        var blobClient = client.GetBlobClient(name);
        try
        {
            var content = blobClient.DownloadContent();

            var contentType = ImageContentHelper.GetImageContentType(content.Value.Details.ContentType);

            image = new ImageData(
                content.Value.Content.ToArray(),
                content.Value.Details.LastModified.DateTime,
                contentType);

            return true;

        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
        {
            image = null;
            return false;
        }
    }

    public async Task<(bool result, ImageData image)> TryGetAsync(ImageId imageId)
    {
        var name = GetBlobName(imageId);

        return await TryGetAsync(_client, name);
    }

    private static async Task<(bool result, ImageData image)> TryGetAsync(BlobContainerClient client, string name)
    {
        var blobClient = client.GetBlobClient(name);
        try
        {
            var content = await blobClient.DownloadContentAsync();

            var contentType = ImageContentHelper.GetImageContentType(content.Value.Details.ContentType);

            var image = new ImageData(
                content.Value.Content.ToArray(),
                content.Value.Details.LastModified.DateTime,
                contentType);

            return (true, image);

        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
        {
            return (false, null);
        }
    }

    public void Delete(ImageId id)
    {
        string blobName = GetBlobName(id);

        _client
            .GetBlobClient(blobName)
            .DeleteIfExists();
    }

    public async Task DeleteAll(string branch)
    {
        await DeleteAll(_client, branch);
    }

    private async Task DeleteAll(BlobContainerClient client, string branch)
    {
        await foreach (var blob in client.GetBlobsAsync(prefix: branch))
        {
            if (!blob.Deleted)
            {
                var blobClient = client.GetBlobClient(blob.Name);

                await blobClient.DeleteAsync();
            }
        }
    }

    private string GetBlobName(ImageId imageId) => $"{_settings.BranchName.ToLowerInvariant()}/{imageId.Value:D20}";

    private string GetBlobName(ImageId imageId, ImageSizeOption imageSizeOption) => $"{GetBlobName(imageId)}/{imageSizeOption}";

    public IEnumerable<ImageIdAndSize> EnumerateAllOrderedByName()
    {
        return EnumerateAllOrderedByName(_client);
    }

    private IEnumerable<ImageIdAndSize> EnumerateAllOrderedByName(BlobContainerClient client)
    {
        // Include the slash to elliminate branches with similar names and one zero to elliminate any blobs in the old format for dev.
        string prefix = _settings.BranchName.ToLowerInvariant() + "/0";
        var pageable = client.GetBlobs(prefix: prefix);

        foreach (var page in pageable.AsPages())
        {
            foreach (var blob in page.Values)
            {
                var name = blob.Name.Substring(blob.Name.LastIndexOf('/') + 1);

                yield return new ImageIdAndSize(new ImageId(int.Parse(name)), (int)(blob.Properties.ContentLength ?? 0));
            }
        }
    }
}