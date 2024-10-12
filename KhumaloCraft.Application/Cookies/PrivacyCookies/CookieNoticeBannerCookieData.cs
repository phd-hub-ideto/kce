using KhumaloCraft.Application.Attributes.TypeScript;

namespace KhumaloCraft.Application.Cookies.PrivacyCookies;

[TypeScriptAdditionalModel]
public class CookieNoticeBannerCookieData
{
    public DateTime Date { get; set; }
    public bool? AdConsentGiven { get; set; }
}