using KhumaloCraft.Dependencies;
using KhumaloCraft.Imaging;

namespace KhumaloCraft.Domain.Images;

[Singleton]
public class FallbackImageService
{
    private readonly ISettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;

    public FallbackImageService(ISettings settings, IHttpClientFactory httpClientFactory)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
    }

    public bool FallbackEnabled => !string.IsNullOrEmpty(_settings.ImageServerFallbackServer);

    public bool TryGetFallbackImage(ImageId imageId, out ImageData imageData)
    {
        if (FallbackEnabled)
        {
            var uriBuilder = new UriBuilder(_settings.ImageServerFallbackServer)
            {
                Path = imageId.ToString()
            };

            var client = _httpClientFactory.CreateClient();

            var fallbackResponse = client.GetAsync(uriBuilder.Uri).GetAwaiter().GetResult();

            if (fallbackResponse.IsSuccessStatusCode)
            {
                using (var stream = fallbackResponse.Content.ReadAsStream())
                {
                    var headers = fallbackResponse.Content.Headers;
                    var contentType = ImageContentHelper.GetImageContentType(headers.ContentType.MediaType);
                    var modifiedDate = headers.LastModified < DateTime.Now ? headers.LastModified.Value.LocalDateTime : DateTime.Now;

                    imageData = new ImageData(stream.ReadAllBytes(), modifiedDate, contentType);
                    return true;
                }
            }
            else if (fallbackResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                imageData = default;
                return false;
            }
            else
            {
                fallbackResponse.EnsureSuccessStatusCode();
            }
        }

        imageData = default;

        return false;
    }
}
