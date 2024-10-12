using KhumaloCraft.Dependencies;
using KhumaloCraft.Http;
using Newtonsoft.Json;
using System.Web;

namespace KhumaloCraft.Application.Cookies;

[Singleton(Contract = typeof(IUserLocationCookiePersistor))]
public class UserLocationCookiePersistor : IUserLocationCookiePersistor
{
    private readonly IHttpCookies _httpCookies;
    private readonly IHttpContextProvider _httpContextProvider;

    private string _cookieName;
    private const int AgeInDays = 365;

    public UserLocationCookiePersistor(IHttpCookies httpCookies, IHttpContextProvider httpContextProvider)
    {
        _httpCookies = httpCookies;
        _cookieName = Cookie.UserLocation.Name();
        _httpContextProvider = httpContextProvider;
    }
    public void Save(UserLocationCookie userLocationCookieData)
    {
        _httpContextProvider.AllowCookiesInResponse = true;

        _httpCookies.SetCookie(_cookieName, JsonConvert.SerializeObject(userLocationCookieData), TimeSpan.FromDays(AgeInDays));
    }

    public void Remove()
    {
        _httpContextProvider.AllowCookiesInResponse = true;

        _httpCookies.ClearCookie(_cookieName);
    }

    private void SaveCookie(double latitude, double longitude)
    {
        _httpContextProvider.AllowCookiesInResponse = true;

        //TODO-L : Implement GeographicPoint
        /* 
        if (_locationMappingRepository.TryMapSuburbIdForLocation(new GeographicPoint(latitude, longitude), out var suburbId))
        {
            var suburb = _locationCache.FetchSuburb(suburbId);
            var cookieData = new UserLocationCookie()
            {
                CityName = suburb.CityName,
                Latitude = latitude,
                Longitude = longitude,
                SuburbId = suburb.Id,
                SuburbName = suburb.Name
            };
            _httpCookies.SetCookie(_cookieName, JsonConvert.SerializeObject(cookieData), TimeSpan.FromDays(AgeInDays));
        }*/
    }

    //TODO-L : Implement GeographicPoint
    /*
    public GeographicPoint TryGetGeolocation()
    {
        var userLocation = TryGetUserLocationData();
        if (userLocation != null)
        {
            return new GeographicPoint(userLocation.Latitude, userLocation.Longitude);
        }

        return null;
    }*/

    public UserLocationCookie TryGetUserLocationData()
    {
        var cookie = _httpCookies.GetCookie(_cookieName);

        if (!string.IsNullOrEmpty(cookie))
        {

            var userLocation = JsonConvert.DeserializeObject<UserLocationCookie>(HttpUtility.UrlDecode(cookie), new JsonSerializerSettings()
            {
                Error = (sender, e) => e.ErrorContext.Handled = true
            });

            if (userLocation != null)
            {
                return userLocation;
            }
        }
        return null;
    }
}