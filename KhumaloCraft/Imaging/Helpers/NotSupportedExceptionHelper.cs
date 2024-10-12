namespace KhumaloCraft.Imaging;

public class NotSupportedExceptionHelper
{
    public static Exception New(string what, object value)
    {
        return new NotSupportedException($"{what} of {value} is not supported.");
    }

    public static Exception New(Type type, object value)
    {
        return New(type.Name, value);
    }
}