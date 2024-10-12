using System.Net;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Http;

public class NullHttpRequestProvider : IHttpRequestProvider
{
    public Uri UrlReferrer => null;

    public string HttpMethod => null;

    public string UserAgent => null;

    public Uri RequestUri => null;

    public string RequestUrl => null;

    public IPAddress UserHostAddress => null;

    public string UserHostName => null;

    public string XForwardedFor => null;

    public string RemoteUser => null;

    public string RemoteHost => null;

    public string Browser => null;

    public string BrowserPlatform => null;

    public bool IsSecureConnection => false;

    public IDictionary<string, object> RouteDataValues => new Dictionary<string, object>(0);

    public IRequestCookieCollection RequestCookies => null;

    public bool IsBingBot()
    {
        return false;
    }

    public bool IsGoogleBot()
    {
        return false;
    }

    public bool IsTwitterBot()
    {
        return false;
    }

    public bool TryGetCookie(string cookieName, out string cookie)
    {
        cookie = default;
        return false;
    }

    public bool TryGetHeader(string header, out string value)
    {
        value = default;
        return false;
    }
}