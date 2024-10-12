using KhumaloCraft.Application.Browsers;
using KhumaloCraft.Http;

namespace KhumaloCraft.Application.DisplayModes;

internal class DisplayModeSelector : IDisplayModeSelector
{
    private const string SelectedInterfaceCookieName = "interface";

    public DisplayModeSelector(IBrowserDetector browserDetector, IHttpCookies httpCookies)
    {
        BrowserDetector = browserDetector;
        HttpCookies = httpCookies;
    }

    public IBrowserDetector BrowserDetector { get; }
    public IHttpCookies HttpCookies { get; }

    public DisplayModeType GetSelected()
    {
        return GetExplicitlySelectedDisplayMode() ?? GetDetectedDisplayMode();
    }

    public DisplayModeType GetDetectedDisplayMode()
    {
        var browserType = BrowserDetector.GetDetectedInterface();
        switch (browserType)
        {
            case BrowserType.Desktop:
                return DisplayModeType.Desktop;
            case BrowserType.SmartPhone:
                return DisplayModeType.SmartPhone;
            case BrowserType.FeaturePhone:
                return DisplayModeType.SmartPhone;
            default:
                throw new NotSupportedException($"BrowserType '{browserType}' is not supported.");
        }
    }

    private DisplayModeType? GetExplicitlySelectedDisplayMode()
    {
        var cookie = HttpCookies.GetCookie(SelectedInterfaceCookieName);

        if (!string.IsNullOrWhiteSpace(cookie)
            && Enum.TryParse<DisplayModeType>(cookie, true, out var selectedInterface))
        {
            return selectedInterface;
        }

        return null;
    }

    public void SetSelectedInterface(DisplayModeType displayModeType)
    {
        var explicitDisplayMode = GetExplicitlySelectedDisplayMode();
        if (explicitDisplayMode != null || explicitDisplayMode != displayModeType)
        {
            if (GetDetectedDisplayMode() == displayModeType)
            {
                HttpCookies.ClearCookie(SelectedInterfaceCookieName);
            }
            else
            {
                HttpCookies.SetCookie(SelectedInterfaceCookieName, displayModeType.ToString().ToLowerInvariant());
            }
        }
    }
}
