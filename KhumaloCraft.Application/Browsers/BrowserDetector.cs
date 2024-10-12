using KhumaloCraft.Http;

namespace KhumaloCraft.Application.Browsers;

public class BrowserDetector : IBrowserDetector
{
    private readonly IHttpRequestProvider _httpRequestProvider;

    public BrowserDetector(IHttpRequestProvider httpRequestProvider)
    {
        _httpRequestProvider = httpRequestProvider ?? throw new ArgumentNullException(nameof(httpRequestProvider));
    }

    public BrowserType GetDetectedInterface()
    {
        if (BrowserHelper.IsSmartphone(_httpRequestProvider.UserAgent))
        {
            return BrowserType.SmartPhone;
        }

        if (BrowserHelper.IsFeaturePhone(_httpRequestProvider.UserAgent))
        {
            return BrowserType.FeaturePhone;
        }

        return BrowserType.Desktop;
    }
}