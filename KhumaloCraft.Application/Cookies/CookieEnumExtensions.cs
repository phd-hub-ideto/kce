using KhumaloCraft.Helpers;
using System.ComponentModel;

namespace KhumaloCraft.Application.Cookies;

public static class CookieEnumExtensions
{
    public static string Name(this Cookie cookie)
    {
        return cookie.GetBestDescription();
    }
}