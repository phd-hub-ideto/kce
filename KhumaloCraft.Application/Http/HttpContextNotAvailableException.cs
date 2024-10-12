namespace KhumaloCraft.Application.Http;

public class HttpContextNotAvailableException : Exception
{
    public HttpContextNotAvailableException() :
        base("HttpContext is not available.")
    {

    }
}