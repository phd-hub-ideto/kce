using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Application.Authentication;

public interface IFormsAuthenticator
{
    bool Authenticated { get; }
    void AuthenticateWithoutRedirect(string username, string password, bool persistCookie, bool logUserLogin);
    void Authenticate(User user, int impersonatorUserId, int impersonationLogId);
    void Authenticate(KhumaloCraftPrincipal principal, bool persistCookie);
    void Signout();
    AuthenticationCookieUserData GetAuthCookieUserData();
    bool TryGetAuthCookieUserData(out AuthenticationCookieUserData authCookieUserData);
    void StopUserImpersonation();
}