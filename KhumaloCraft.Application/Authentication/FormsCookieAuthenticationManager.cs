using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Authentication;

[Singleton]
public class FormsCookieAuthenticationManager : ICookieAuthenticationManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPrincipalResolver _principalResolver;
    private readonly IHttpContextProvider _httpContextProvider;

    public FormsCookieAuthenticationManager(
        IHttpContextAccessor httpContextAccessor,
        IPrincipalResolver principalResolver,
        IHttpContextProvider httpContextProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _principalResolver = principalResolver;
        _httpContextProvider = httpContextProvider;
    }

    public void RemoveAuthCookie()
    {
    }

    public void SignIn(KhumaloCraftPrincipal principal, bool persistCookie)
    {
        _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties()
            {
                IsPersistent = persistCookie,
            }).Wait();

        _httpContextProvider.AllowCookiesInResponse = true;
    }

    public void SignOut()
    {
        // Clear the existing external cookie
        _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
    }

    public bool TryGet(out KhumaloCraftPrincipal principal, out AuthenticationCookieUserData authCookieUserData)
    {
        authCookieUserData = null;

        return _principalResolver.TryResolveCurrentPrincipal(out principal);
    }
}