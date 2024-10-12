using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Authentication.Passwords;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Http;
using System.Security.Authentication;
using System.Security.Principal;

namespace KhumaloCraft.Application.Authentication;

[Singleton]
public class FormsAuthenticator : IFormsAuthenticator
{
    private readonly IPrincipalResolver _principalResolver;
    private readonly IHttpContextProvider _httpContextProvider;
    private readonly IHttpCookies _httpCookies;
    private readonly IUserRepository _userRepository;
    //TODO-LP : Add IUserImpersonationRepository
    //private readonly IUserImpersonationRepository _userImpersonationRepository;
    private readonly ICookieAuthenticationManager _authenticationCookieManager;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IUserRolePermissionRepository _userRolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public FormsAuthenticator(
        IPrincipalResolver principalResolver,
        IHttpContextProvider httpContextProvider,
        IHttpCookies httpCookies,
        IUserRepository userRepository,
        ICookieAuthenticationManager authenticationCookieManager,
        IPasswordValidator passwordValidator,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _principalResolver = principalResolver;
        _httpContextProvider = httpContextProvider;
        _httpCookies = httpCookies;
        _userRepository = userRepository;
        _authenticationCookieManager = authenticationCookieManager;
        _passwordValidator = passwordValidator;
        _userRolePermissionRepository = userRolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public bool Authenticated => _principalResolver.IsAuthenticated();

    public void Authenticate(KhumaloCraftPrincipal principal, bool persistCookie)
    {
        _principalResolver.SetPrincipal(principal);

        CreateAndAddAuthenticationCookie(principal, persistCookie);
    }

    private KhumaloCraftPrincipal Authenticate(string username, string password, bool logUserLogin)
    {
        if (!_passwordValidator.TryValidatePassword(username, password, logUserLogin, out var user, out _)
            || !string.Equals(username, user.Username, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new AuthenticationException("Invalid username or password.");
        }

        var principal = KhumaloCraftPrincipal.Create(user, _userRolePermissionRepository, _userRoleRepository);

        _principalResolver.SetPrincipal(principal);

        return principal;
    }

    private void Authenticate(KhumaloCraftPrincipal principal, bool persistCookie, int? impersonatorUserId = null, int? impersonationLogId = null)
    {
        CreateAndAddAuthenticationCookie(principal, persistCookie, impersonatorUserId, impersonationLogId);
    }

    public void AuthenticateWithoutRedirect(string username, string password, bool persistCookie, bool logUserLogin)
    {
        var principal = Authenticate(username, password, logUserLogin);

        Authenticate(principal, persistCookie);
    }

    public void Authenticate(User user, bool persistCookie)
    {
        var principal = KhumaloCraftPrincipal.Authenticate(user, _userRolePermissionRepository, _userRoleRepository, _userRepository);

        Authenticate(principal, persistCookie);
    }

    public void Authenticate(User user, int impersonatorUserId, int impersonationLogId)
    {
        var principal = KhumaloCraftPrincipal.Authenticate(user, _userRolePermissionRepository, _userRoleRepository, _userRepository);

        Authenticate(principal, false, impersonatorUserId, impersonationLogId);
    }

    public void CreateAndAddAuthenticationCookie(
        KhumaloCraftPrincipal principal,
        bool persistCookie,
        int? impersonatorUserId = null,
        int? impersonationLogId = null)
    {
        var authCookieUserData = new AuthenticationCookieUserData()
        {
            UserId = principal.User.Id.Value,
            ImpersonatorUserId = impersonatorUserId,
            UserImpersonationLogId = impersonationLogId
        };

        _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, authCookieUserData);

        _authenticationCookieManager.SignIn(principal, persistCookie);
    }

    public void Signout()
    {
        EndImpersonationFromSignOut();

        _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, null);

        _authenticationCookieManager.SignOut();

        _principalResolver.SetPrincipal(null);

        _principalResolver.ClearPrincipal();

        if (_httpContextProvider.SessionExists)
        {
            _httpContextProvider.ClearSession();
        }

        _authenticationCookieManager.RemoveAuthCookie();

        RemoveOlaCookies();
    }

    public IPrincipal AuthenticateRequestFromCookie(string requestPath, params string[] ignore)
    {
        if (ignore != null)
        {
            foreach (string ignored in ignore)
            {
                if (requestPath.StartsWith(ignored, StringComparison.CurrentCultureIgnoreCase))
                {
                    return null;
                }
            }
        }

        if (_authenticationCookieManager.TryGet(out var principal, out var authCookieUserData))
        {
            _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, authCookieUserData);

            return principal;
        }

        return null;
    }

    public string RemoveOlaCookies()
    {
        const string rememberme = "remember-me";
        const string urss = "urss";
        const string ursid = "ursid";
        const string ctuser = "ctuser";
        const string mvthistory = "mvthistory";
        const string dlId = "dlId";
        const string AWSALB = "AWSALB";

        _httpContextProvider.AllowCookiesInResponse = true;
        _httpCookies.ExpireCookie(rememberme, _httpCookies.Domain, "/");
        _httpCookies.ExpireCookie(urss, _httpCookies.Domain, "/");
        _httpCookies.ExpireCookie(ursid, _httpCookies.Domain, "/");
        _httpCookies.ExpireCookie(ctuser, _httpCookies.Domain, "/");
        _httpCookies.ExpireCookie(mvthistory, _httpCookies.Domain, "/");
        _httpCookies.ExpireCookie(dlId, _httpCookies.Domain, "/");

        return _httpCookies.ExpireCookie(AWSALB, _httpCookies.Domain, "/");
    }

    public AuthenticationCookieUserData GetAuthCookieUserData()
    {
        if (_httpContextProvider.GetItem(AuthenticationCookieUserData.AuthCookieUserDataKey) is AuthenticationCookieUserData authCookieUserData)
        {
            return authCookieUserData;
        }
        throw new Exception("AuthCookie UserData not available");
    }

    public bool TryGetAuthCookieUserData(out AuthenticationCookieUserData authCookieUserData)
    {
        if (_httpContextProvider.GetItem(AuthenticationCookieUserData.AuthCookieUserDataKey) is AuthenticationCookieUserData data)
        {
            authCookieUserData = data;
            return true;
        }

        authCookieUserData = null;
        return false;
    }

    public void StopUserImpersonation()
    {
        var impersonator = _userRepository.Query().Single(u => u.Id == GetAuthCookieUserData().ImpersonatorUserId.Value);

        _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, null);

        Authenticate(impersonator, false);
    }

    private void EndImpersonationFromSignOut()
    {
        if (TryGetAuthCookieUserData(out AuthenticationCookieUserData authCookieUserData) && authCookieUserData.ImpersonatorUserId.HasValue)
        {
            //TODO-L : Add IUserImpersonationRepository
            //_userImpersonationRepository.RecordImpersonationEnd(authCookieUserData.UserImpersonationLogId.Value);
        }
    }
}