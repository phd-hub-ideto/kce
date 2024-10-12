namespace KhumaloCraft.Application.Exceptions;

public class ClientException : Exception
{
    public static ClientException ParameterIsNull(string name)
    {
        return new ClientException($"Parameter {name} is null.");
    }

    public static ClientException ParameterIsInvalid(string name, object value, string detail)
    {
        return new ClientException($"Value [{value}] for parameter {name} is invalid. {detail}");
    }

    public static ClientException PropertyIsMissing(string objectName, string propertyName)
    {
        return new ClientException($"Property {objectName}.{propertyName} is missing.");
    }

    public static ClientException PropertyIsInvalid(string objectName, string propertyName, object value)
    {
        return new ClientException($"Value [{value}] for property {objectName}.{propertyName} is invalid.");
    }

    public static ClientException PropertyIsInvalid(string objectName, string propertyName, object value, string detail)
    {
        return new ClientException($"Value [{value}] for property {objectName}.{propertyName} is invalid. {detail}");
    }

    public static ClientException EmailDomainPropertyIsInvalid(string objectName, string propertyName, object value)
    {
        return new ClientException($"The email domain '{value}' for property {objectName}.{propertyName} doesn't exist. Please check spelling");
    }

    public ClientException(string message)
        : base(message)
    { }

    public ClientException(string message, params object[] args)
        : base(string.Format(message, args))
    { }

    public ClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}