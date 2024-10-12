using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using Newtonsoft.Json;
using System.Web;

namespace KhumaloCraft.Application.Users.UserPreferences;

[Singleton]
public class UserPreferencesService
{
    private readonly IHttpCookies _httpCookies;
    private static readonly string _cookieName = EnumHelper.GetDescription(Cookie.UserPreferences);

    public UserPreferencesService(IHttpCookies httpCookies)
    {
        _httpCookies = httpCookies;
    }

    public bool GetShowGoogleDebugAds()
    {
        var cookie = _httpCookies.GetCookie(_cookieName);

        if (!string.IsNullOrEmpty(cookie))
        {
            var userPreferences = JsonConvert.DeserializeObject<UserPreferencesCookie>(HttpUtility.UrlDecode(cookie), new JsonSerializerSettings()
            {
                Error = (sender, e) => e.ErrorContext.Handled = true
            });

            if (userPreferences != null)
            {
                return userPreferences.ShowDebugGoogleAds;
            }
        }

        return false;
    }

    public void SetShowGoogleDebugAds(bool showGoogleDebugAds)
    {
        var cookieData = new UserPreferencesCookie()
        {
            ShowDebugGoogleAds = showGoogleDebugAds
        };

        _httpCookies.SetCookie(_cookieName, JsonConvert.SerializeObject(cookieData), TimeSpan.FromDays(30));
    }
}