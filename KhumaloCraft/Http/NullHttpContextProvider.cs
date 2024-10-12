using System.Collections.Specialized;

namespace KhumaloCraft.Http;

public class NullHttpContextProvider : IHttpContextProvider
{
    public bool CanGetHttpContext => false;

    public string RequestId => null;

    public bool AllowCookiesInResponse { get => false; set => _ = value; }

    public NameValueCollection Headers => new NameValueCollection();

    public string RequestPathAndQuery => null;

    public bool HandlerAvailable => false;

    public bool SessionExists => false;

    public void CheckCacheabilityAndCookies(bool allowCookiesInResponse, bool clearCookiesForJsonResponse)
    {
        //no op
    }

    public void ClearSession()
    {
        throw new NotImplementedException();
    }

    public void ClearSessionValue(string key)
    {
        throw new NotImplementedException();
    }

    public string GetCurrentCookie(string name)
    {
        return null;
    }

    public string GetHttpHeaders(string name)
    {
        return null;
    }

    public object GetItem(object key)
    {
        return null;
    }

    public string GetSessionId()
    {
        return null;
    }

    public object GetSessionValue(string key)
    {
        return null;
    }

    public void SetItem(object key, object value)
    {
        //no op
    }

    public void SetSessionValue(string key, object value)
    {
        //no op
    }

    public string Username => string.Empty;
}
