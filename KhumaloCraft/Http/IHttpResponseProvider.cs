using System.Net;

namespace KhumaloCraft.Http;

public interface IHttpResponseProvider
{
    bool ResponseIsAvailable { get; }
    HttpStatusCode StatusCode { get; }
    void SetHeader(string name, string value);
    void SetCookie(string cookieName, string value, DateTime? expiry);
    bool HeadersWritten();
}