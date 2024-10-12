using KhumaloCraft.Dependencies;
using KhumaloCraft.Urls;

namespace KhumaloCraft.Application.Routing.Portal;

[Singleton]
public class ExternalUrlBuilder(ISiteUriHelper siteUriHelper)
{
    internal string BuildHomeUrl()
    {
        return siteUriHelper.GetSiteUri().AbsoluteUri;
    }

    public string BuildTermsAndConditionsUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.TermsAndConditions}";
    }

    public string BuildPrivacyPolicyUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.PrivacyPolicy}";
    }

    public string BuildPrivacyPolicyWithUtmTags(string source, string medium, string campaign)
    {
        return $"{BuildHomeUrl()}{ExternalUrls.PrivacyPolicy}/?utm_source={source}&utm_medium={medium}&utm_campign={campaign}";
    }

    public string BuildCookiePolicyUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.CookiePolicy}";
    }

    public string BuildLoginUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.Login}";
    }

    public string BuildAccountActivationUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.AccountActivation}";
    }

    public string BuildViewOrdersUrl()
    {
        return $"{BuildHomeUrl()}{ExternalUrls.ViewOrders}";
    }
}