namespace KhumaloCraft.Http;

public interface IHttpCookies
{
    bool IsAvailable { get; }
    string Domain { get; }

    string GetCookie(string name);
    void SetCookie(string name, string value, TimeSpan expiryTime);
    void SetCookie(string name, string value, DateTime? expiryDate = null);
    void SetCookie(string name, string value, HttpCookieOptions options);
    void SetRawCookie(string name, string value, DateTime? expiryDate = null);
    void SetRawCookie(string name, string value, HttpCookieOptions options);
    void ClearCookie(string name);
    string GetRawCookie(string name);
    string ExpireCookie(string name, string domain, string path);
    void RemoveCookie(string name);
}