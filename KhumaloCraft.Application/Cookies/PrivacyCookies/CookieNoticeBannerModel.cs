using KhumaloCraft.Application.Attributes.TypeScript;
using KhumaloCraft.Helpers;
using Newtonsoft.Json;

namespace KhumaloCraft.Application.Cookies.PrivacyCookies;

[TypeScriptAdditionalModel]
public class CookieNoticeBannerModel
{
    //KhumaloCraft accepted cookie policy -> KCACP
    public static string CookieName => Cookie.AcceptedCookiePolicy.GetBestDescription();

    public const int DaysCookieIsValid = 365;

    public string CookiePolicyUrl { get; }

    public bool ShouldDisplay { get; }

    public bool AdConsentGiven { get; }

    internal CookieNoticeBannerModel(string cookiePolicyUrl, bool shouldDisplay, bool adConsentGiven)
    {
        CookiePolicyUrl = cookiePolicyUrl;
        ShouldDisplay = shouldDisplay;
        AdConsentGiven = adConsentGiven;
    }

    public static CookieNoticeBannerModel Create(string cookiePolicyUrl, string cookie, bool isInformationPage = false)
    {
        if (string.IsNullOrEmpty(cookie))
        {
            return new CookieNoticeBannerModel(cookiePolicyUrl, !isInformationPage, false);
        }

        bool success = true;

        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        var cookieData = JsonConvert.DeserializeObject<CookieNoticeBannerCookieData>(cookie, settings);

        var showBanner = !(isInformationPage || (success && cookieData.AdConsentGiven.HasValue));

        return new CookieNoticeBannerModel(cookiePolicyUrl, showBanner, cookieData.AdConsentGiven == true);
    }
}