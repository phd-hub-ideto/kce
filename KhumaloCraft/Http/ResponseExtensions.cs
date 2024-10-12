using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Http;

public static class ResponseExtensions
{
    public static void SetExpires(this HttpResponse httpResponse, DateTime expiry)
    {
        var headers = httpResponse.GetTypedHeaders();
        headers.Expires = expiry;
    }

    public static void SetLastModified(this HttpResponse httpResponse, DateTime lastModified)
    {
        var headers = httpResponse.GetTypedHeaders();
        headers.LastModified = lastModified;
    }

    public static void SetCachePrivate(this HttpResponse httpResponse)
    {
        var headers = httpResponse.GetTypedHeaders();
        if (headers.CacheControl == null)
        {
            headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Private = true
            };
        }
        else
        {
            headers.CacheControl.Private = true;
        }
    }
}