using System.Net;

namespace KhumaloCraft.Http;

public class NullHttpResponseProvider : IHttpResponseProvider
{
    public bool ResponseIsAvailable => false;

    public HttpStatusCode StatusCode => HttpStatusCode.OK; //Not ideal but we don't have a choice.

    public void SetHeader(string name, string value)
    {

    }
    public void SetCookie(string cookieName, string value, DateTime? expiry)
    {

    }
    public bool HeadersWritten()
    {
        return false;
    }
}