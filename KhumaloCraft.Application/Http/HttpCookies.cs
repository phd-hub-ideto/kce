using KhumaloCraft.Http;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace KhumaloCraft.Application.Http;

public class HttpCookies : IHttpCookies
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISettings _settings;

    public HttpCookies(IHttpContextAccessor httpContextAccessor, ISettings settings)
    {
        _httpContextAccessor = httpContextAccessor;
        _settings = settings;
    }

    public bool IsAvailable => _httpContextAccessor.HttpContext != null;

    public string Domain => GetHostOrDomainName(_httpContextAccessor.HttpContext.Request.Host);

    public void ClearCookie(string name)
    {
        RemoveCookie(name);
    }

    public string GetCookie(string name)
    {
        return GetRawCookie(name);
    }

    public string GetRawCookie(string name)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        return httpContext?.Request.Cookies[name];
    }

    public void SetCookie(string name, string value, TimeSpan expiryTime)
    {
        SetCookie(name, value, new HttpCookieOptions(DateTime.Now + expiryTime));
    }

    public void SetCookie(string name, string value, DateTime? expiryDate = null)
    {
        SetCookie(name, value, new HttpCookieOptions(expiryDate));
    }

    public void SetCookie(string name, string value, HttpCookieOptions options)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append(name, value, new CookieOptions()
        {
            Expires = options.ExpiryDate,
            Domain = options.Domain ?? GetHostOrDomainName(_httpContextAccessor.HttpContext.Request.Host),
            SameSite = GetMode(options.SameSiteMode),
            Secure = options.Secure
        });
    }

    public void SetRawCookie(string name, string value, DateTime? expiryDate = null)
    {
        SetCookie(name, value, new HttpCookieOptions(expiryDate));
    }

    public void SetRawCookie(string name, string value, HttpCookieOptions options)
    {
        SetCookie(name, value, options);
    }

    public string ExpireCookie(string name, string domain, string path)
    {
        var cookie = GetCookie(name);

        if (cookie != null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Response.Cookies.Append(name, "", new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-5d),
                Domain = domain,
                Path = path
            });
        }
        return cookie;
    }

    public void RemoveCookie(string name)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.Response.Cookies.Delete(name, new CookieOptions()
        {
            Domain = Domain
        });
    }

    private Microsoft.AspNetCore.Http.SameSiteMode GetMode(KhumaloCraft.Http.SameSiteMode sameSiteMode)
    {
        return sameSiteMode switch
        {
            KhumaloCraft.Http.SameSiteMode.None => Microsoft.AspNetCore.Http.SameSiteMode.None,
            KhumaloCraft.Http.SameSiteMode.Lax => Microsoft.AspNetCore.Http.SameSiteMode.Lax,
            KhumaloCraft.Http.SameSiteMode.Strict => Microsoft.AspNetCore.Http.SameSiteMode.Strict,
            _ => throw new NotSupportedException(sameSiteMode.ToString())
        };
    }

    private string GetDomain(string hostname)
    {
        var parts = hostname.Split('.');

        if (parts.Length <= 1)
        {
            return hostname;
        }

        return string.Join(".", parts, 1, parts.Length - 1).ToLower();
    }

    public string GetHostOrDomainName(HostString hostString)
    {
        var hostname = hostString.Host;

        if (_settings.UseHostNameForCookies)
        {
            return IPAddress.TryParse(hostname, out _) ? new Uri(_settings.PortalBaseUri).Host : hostname;
        }

        return GetDomain(hostname);
    }
}
