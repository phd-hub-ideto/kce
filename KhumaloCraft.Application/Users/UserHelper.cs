namespace KhumaloCraft.Application.Users;

public static class UserHelper
{
    public static string GetLoginUrl(ISettings settings)
    {
        return $"{settings.PortalBaseUri}/login";
    }
}