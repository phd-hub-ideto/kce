using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Application.Monitoring;
using KhumaloCraft.Http;

namespace KhumaloCraft.Application.Session;

public class UniqueUserTracker : IUniqueUserTracker
{
    private readonly IHttpCookies _httpCookies;
    private readonly IRequestStorage _requestStorage;

    public UniqueUserTracker(IHttpCookies httpCookies, IRequestStorage requestStorage)
    {
        _httpCookies = httpCookies;
        _requestStorage = requestStorage;
    }

    public const string IdKey = "Id";
    public const string DateKey = "Date";

    public string GetUniqueUserId()
    {
        EnsureCookie();

        var storage = _requestStorage.Get<UniqueUserIdStorage>();
        return storage.Cookie?.Id;
    }

    public void EnsureCookie()
    {
        if (TrySetUniqueUserId())
        {
            IssueCookie();
        }
    }

    private bool TrySetUniqueUserId()
    {
        if (!_httpCookies.IsAvailable) return false;

        var storage = _requestStorage.Get<UniqueUserIdStorage>();
        if (storage.Cookie != null) return false;

        var rawUserCookie = _httpCookies.GetRawCookie(Cookie.UserId.Name());

        if (string.IsNullOrEmpty(rawUserCookie)
            || !UniqueUserIdCookie.TryParse(rawUserCookie, out var userCookie)
            || DateTime.Now.Subtract(userCookie.Date).Days >= 200)
        {
            storage.Cookie = new UniqueUserIdCookie(new UniqueUserIdGenerator().Create(), DateTime.Now);
            return true;
        }

        storage.Cookie = userCookie;

        return false;
    }

    public void IssueCookie()
    {
        var storage = _requestStorage.Get<UniqueUserIdStorage>();

        _httpCookies.SetRawCookie(Cookie.UserId.Name(), storage.Cookie.ToString(), TwoYearsLater());
    }

    private DateTime TwoYearsLater()
    {
        return DateTime.Now.AddYears(2);
    }

    private class UniqueUserIdStorage
    {
        public UniqueUserIdCookie Cookie { get; set; }
    }
}