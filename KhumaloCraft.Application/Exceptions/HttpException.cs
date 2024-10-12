namespace KhumaloCraft.Application.Exceptions;

// https://stackoverflow.com/a/35829637
public class HttpException : Exception
{
    private readonly int _httpStatusCode;

    public HttpException(int httpStatusCode)
    {
        _httpStatusCode = httpStatusCode;
    }

    public HttpException(System.Net.HttpStatusCode httpStatusCode)
    {
        _httpStatusCode = (int)httpStatusCode;
    }

    public HttpException(int httpStatusCode, string message) : base(message)
    {
        _httpStatusCode = httpStatusCode;
    }

    public HttpException(System.Net.HttpStatusCode httpStatusCode, string message) : base(message)
    {
        _httpStatusCode = (int)httpStatusCode;
    }

    public HttpException(int httpStatusCode, string message, Exception inner) : base(message, inner)
    {
        _httpStatusCode = httpStatusCode;
    }

    public HttpException(System.Net.HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner)
    {
        _httpStatusCode = (int)httpStatusCode;
    }

    public int StatusCode { get { return _httpStatusCode; } }
}