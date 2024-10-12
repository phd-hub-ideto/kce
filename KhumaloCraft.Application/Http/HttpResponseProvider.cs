using System.Net;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Http;

public class HttpResponseProvider(
    IHttpContextAccessor httpContextAccessor) : IHttpResponseProvider
{
    private HttpResponse _response
    {
        get
        {
            try
            {
                return httpContextAccessor?.HttpContext?.Response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public bool ResponseIsAvailable => _response != null;
    public HttpStatusCode StatusCode => (HttpStatusCode)_response.StatusCode;

    public void SetHeader(string name, string value)
    {
        _response.Headers[name] = value;
    }
    public void SetCookie(string cookieName, string value, DateTime? expiry)
    {
        if (ResponseIsAvailable)
        {
            CookieOptions options = default;
            if (expiry.HasValue)
            {
                options = new CookieOptions()
                {
                    Expires = expiry.Value,
                };
            }
            _response.Cookies.Append(cookieName, value, options);
        }
    }

    // https://stackoverflow.com/questions/55119289/response-headerswritten-in-asp-net-core-2
    public bool HeadersWritten()
    {
        if (ResponseIsAvailable)
        {
            return _response.HasStarted;
        }
        return false;
    }
}