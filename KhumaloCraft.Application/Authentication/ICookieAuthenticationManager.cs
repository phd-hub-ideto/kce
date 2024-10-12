using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Application.Authentication;

public interface ICookieAuthenticationManager
{
    void SignIn(KhumaloCraftPrincipal principal, bool persistCookie);
    void RemoveAuthCookie();
    void SignOut();
    bool TryGet(out KhumaloCraftPrincipal principal, out AuthenticationCookieUserData authCookieUserData);
}