using KhumaloCraft.Dependencies;
using KhumaloCraft.Http;

namespace KhumaloCraft.Application.Browsers;

[Singleton]
public class BrowserSupportedFeature
{
    private readonly IHttpRequestProvider _httpRequestProvider;

    public BrowserSupportedFeature(IHttpRequestProvider httpRequestProvider)
    {
        _httpRequestProvider = httpRequestProvider;
    }

    private static readonly string[] _browsersWhichSupportOneTap =
    [
        "Chrome",
        "Firefox"
    ];

    public bool SupportsGoogleOneTap()
    {
        var browser = _httpRequestProvider.Browser;
        var browserPlatform = _httpRequestProvider.BrowserPlatform;

        if (string.Equals(browser, "Edge", StringComparison.InvariantCultureIgnoreCase)
            && browserPlatform.Equals("Windows 10"))
        {
            return true;
        }
        else if (_browsersWhichSupportOneTap.Any(b => string.Equals(browser, b, StringComparison.InvariantCultureIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}