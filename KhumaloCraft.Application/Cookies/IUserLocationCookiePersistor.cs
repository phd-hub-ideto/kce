namespace KhumaloCraft.Application.Cookies;

public interface IUserLocationCookiePersistor
{
    void Save(UserLocationCookie userLocationCookieData);
    void Remove();

    //TODO-L : Implement GeographicPoint
    //GeographicPoint TryGetGeolocation();
    UserLocationCookie TryGetUserLocationData();
}