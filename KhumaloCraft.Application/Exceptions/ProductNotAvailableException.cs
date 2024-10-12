namespace KhumaloCraft.Application.Exceptions;

public class ProductNotAvailableException : ClientException
{
    public ProductNotAvailableException(string message, bool expired = false)
        : base(message)
    {
        HasExpired = expired;
    }

    public bool HasExpired { get; set; }
}