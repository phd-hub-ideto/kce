using System.Text.Json;

namespace KhumaloCraft.Application.Authentication;

public class AuthenticationCookieContainer
{
    public string AuthCookie { get; set; }
    public short Version { get; set; }

    public static AuthenticationCookieContainer Create<TCookie>(TCookie cookieClass, short version)
    {
        return new AuthenticationCookieContainer()
        {
            AuthCookie = JsonSerializer.Serialize(cookieClass),
            Version = version
        };
    }
}