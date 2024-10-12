using KhumaloCraft.Application.Controllers;
using KhumaloCraft.Application.Cookies.PrivacyCookies;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KhumaloCraft.Application.Cookies;

public class CookieController : BaseController
{
    private readonly IHttpCookies _httpCookies;
    private readonly IHttpContextProvider _httpContextProvider;

    public CookieController(IHttpCookies httpCookies, IHttpContextProvider httpContextProvider)
    {
        _httpCookies = httpCookies;
        _httpContextProvider = httpContextProvider;
    }

    [HttpPost]
    [Route("accept-cookie-policy")]
    public virtual void AcceptCookieNotice([FromQuery] bool adConsentGiven)
    {
        var cookieData = new CookieNoticeBannerCookieData
        {
            Date = DateTime.Now,
            AdConsentGiven = adConsentGiven
        };

        _httpCookies.SetCookie(CookieNoticeBannerModel.CookieName, JsonConvert.SerializeObject(cookieData), TimeSpan.FromDays(CookieNoticeBannerModel.DaysCookieIsValid));

        _httpContextProvider.AllowCookiesInResponse = true;
    }
}