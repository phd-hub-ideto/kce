using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace KhumaloCraft.Http;

public static class RequestExtensions
{
    public static Uri GetUri(this HttpRequest httpRequest)
    {
        return new Uri(httpRequest.GetEncodedUrl());
    }
}