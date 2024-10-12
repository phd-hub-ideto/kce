using System.Collections.Specialized;

namespace KhumaloCraft.Http;

public interface IHttpContextProvider
{
    bool CanGetHttpContext { get; }
    string RequestId { get; }
    bool AllowCookiesInResponse { get; set; }
    NameValueCollection Headers { get; }
    object GetItem(object key);
    void SetItem(object key, object value);
    string GetHttpHeaders(string name);
    string GetCurrentCookie(string name);
    void SetSessionValue(string key, object value);
    object GetSessionValue(string key);
    void ClearSessionValue(string key);
    bool SessionExists { get; }
    void ClearSession();
    bool HandlerAvailable { get; }
    string Username { get; }
    string GetSessionId();
}