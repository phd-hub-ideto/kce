using System.Net;

namespace KhumaloCraft.Domain.Images;

public class ImageDownloader : IImageDownloader
{
    private static readonly HttpClient _httpClient = new();

    public async Task<(bool, ImageDownload)> TryDownloadIfNotModifiedAsync(string imageUrl, DateTimeOffset? lastModified, long? minContentLength, long? maxContentLength)
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, imageUrl);
            httpRequestMessage.Headers.IfModifiedSince = lastModified;

            using (var response = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return (false, null);
                }
                else
                {
                    response.EnsureSuccessStatusCode();

                    var contentLength = response.Content.Headers.ContentLength;
                    if (contentLength != null && contentLength > maxContentLength)
                    {
                        throw new ImageDownloadException($"Image at url {imageUrl} exceeds maximum length of {maxContentLength} bytes.");
                    }

                    var responseBytes = await Download(imageUrl, maxContentLength, response).ConfigureAwait(false);

                    if (responseBytes.Length < minContentLength)
                    {
                        throw new ImageDownloadException($"Image at url {imageUrl} does not meet the minimum length of {minContentLength} bytes.");
                    }

                    /* TODO-LP : Implement content support checking
                    try
                    {
                        var imageContentType = ImagingUtilities.GetImageContentType(responseBytes);
                        if (!imageContentType.IsSupported())
                        {
                            throw new ImageDownloadContentTypeException($"Image at url {imageUrl} has an unsupported content type {imageContentType}.");
                        }
                    }
                    catch (WICDXException ex)
                    {
                        if (!string.IsNullOrWhiteSpace(ex.Message) && ex.Message.StartsWith("Failed to create the image decoder from the stream"))
                        {
                            throw new ImageDownloadContentTypeException($"Image at url {imageUrl} has an unsupported content type.", ex);
                        }
                        else
                        {
                            throw;
                        }
                    }*/

                    var result = new ImageDownload(
                        responseBytes,
                        response.Headers.Date,
                        response.Content.Headers.ContentType);

                    return (true, result);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            throw new ImageDownloadException($"Error while downloading image '{imageUrl}': {ex.Message}");
        }
        catch (UriFormatException ufe)
        {
            throw new ImageDownloadException($"Error while downloading image '{imageUrl}': {ufe.Message}");
        }
    }

    private static async Task<byte[]> Download(string url, long? maxContentLength, HttpResponseMessage response)
    {
        using var bufferStream = new MemoryStream();

        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
        {
            var buffer = new byte[1024];

            int count;

            while ((count = await responseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                if (bufferStream.Length + count > maxContentLength)
                {
                    throw new ImageDownloadException($"Resource at url {url} exceeds maximum length of {maxContentLength}.");
                }

                bufferStream.Write(buffer, 0, count);
            }
        }

        return bufferStream.ToArray();
    }
}