using System.Net;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Http;

public class HttpRequestProvider(IHttpContextAccessor httpContextAccessor) : IHttpRequestProvider
{
    private HttpRequest _httpRequest
    {
        get
        {
            try
            {
                return httpContextAccessor?.HttpContext?.Request;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public Uri UrlReferrer => _httpRequest?.GetTypedHeaders().Referer;

    public string HttpMethod => _httpRequest?.Method;

    public string UserAgent => _httpRequest?.Headers["user-agent"].ToString();

    public Uri RequestUri
    {
        get
        {
            var builder = new UriBuilder();
            builder.Scheme = _httpRequest?.Scheme;
            builder.Host = _httpRequest?.Host.Host;
            builder.Port = _httpRequest?.Host.Port ?? -1;
            builder.Path = _httpRequest?.Path;
            builder.Query = _httpRequest?.QueryString.ToUriComponent();
            return builder.Uri;
        }

    }

    public string RequestUrl => RequestUri.AbsoluteUri;

    public IPAddress UserHostAddress => httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

    //change in behaviour
    public string UserHostName => _httpRequest?.Host.Value; //https://stackoverflow.com/a/50683465/1641076

    public string XForwardedFor => _httpRequest?.Headers["X-Forwarded-For"];

    public string RemoteUser => string.Empty;

    public string RemoteHost => string.Empty;

    public string Browser => string.Empty;

    public string BrowserPlatform => string.Empty;

    public bool IsSecureConnection => _httpRequest.IsHttps;

    public IDictionary<string, object> RouteDataValues => new Dictionary<string, object>();

    public IRequestCookieCollection RequestCookies => _httpRequest.Cookies;

    public bool IsBingBot()
    {
        if (string.IsNullOrEmpty(UserAgent))
            return false;
        return UserAgent.Contains("Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm)");
    }

    public bool IsGoogleBot()
    {
        if (string.IsNullOrEmpty(UserAgent))
            return false;
        return UserAgent.Contains("Googlebot/2.1");
    }

    public bool IsTwitterBot()
    {
        if (string.IsNullOrEmpty(UserAgent))
            return false;
        return UserAgent.Contains("Twitterbot/1.0");
    }

    public bool TryGetCookie(string cookieName, out string cookie)
    {
        if (_httpRequest.Cookies.TryGetValue(cookieName, out cookie))
        {
            return true;
        }
        return false;
    }
    public bool TryGetHeader(string header, out string value)
    {
        value = _httpRequest.Headers[header];
        return value is not null;
    }

}
