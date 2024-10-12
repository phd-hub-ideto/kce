using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Http;

public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest httpRequest)
    {
        return httpRequest.Headers.XRequestedWith == "XMLHttpRequest";
    }
}