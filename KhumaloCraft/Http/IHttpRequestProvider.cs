using System.Net;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Http;

public interface IHttpRequestProvider
{
    Uri UrlReferrer { get; }
    string HttpMethod { get; }
    string UserAgent { get; }
    Uri RequestUri { get; }
    string RequestUrl { get; }
    IPAddress UserHostAddress { get; }
    string UserHostName { get; }
    string XForwardedFor { get; }
    string RemoteUser { get; }
    string RemoteHost { get; }
    string Browser { get; }
    string BrowserPlatform { get; }
    bool IsSecureConnection { get; }
    IDictionary<string, object> RouteDataValues { get; }
    IRequestCookieCollection RequestCookies { get; }
    bool IsBingBot();
    bool IsGoogleBot();
    bool IsTwitterBot();
    bool TryGetCookie(string cookieName, out string cookie);
    bool TryGetHeader(string header, out string value);
}