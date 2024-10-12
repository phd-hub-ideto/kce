namespace KhumaloCraft.Application.Exceptions;

public class SuburbNotFoundException : ClientException
{
    public SuburbNotFoundException(string message) : base(message) { }
}