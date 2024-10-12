using KhumaloCraft.Application.Authentication;
using KhumaloCraft.Domain.Authentication.Passwords;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Application.Users.LoginManager;

public class LoginManager
{
    private readonly IFormsAuthenticator _authenticator;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IUserRolePermissionRepository _userRolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public LoginManager(
        IFormsAuthenticator authenticator,
        IPasswordValidator passwordValidator,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository)
    {
        _authenticator = authenticator;
        _passwordValidator = passwordValidator;
        _userRolePermissionRepository = userRolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    private AuthenticationResult AuthenticateInternal(
        string username,
        string password,
        bool persistCookie,
        bool logUserLogin,
        out KhumaloCraftPrincipal principal)
    {
        if (_passwordValidator.TryValidatePassword(username, password, logUserLogin, out var user, out var userRequiresActivation))
        {
            principal = KhumaloCraftPrincipal.Create(user, _userRolePermissionRepository, _userRoleRepository);

            _authenticator.Authenticate(principal, persistCookie);

            return AuthenticationResult.Authenticated;
        }
        else if (userRequiresActivation)
        {
            principal = null;

            return AuthenticationResult.UserRequiresActivation;
        }

        throw SecurityExceptions.NotAuthenticated();
    }

    public AuthenticationResult Authenticate(
        string username,
        string password,
        bool persistCookie,
        bool logUserLogin,
        out KhumaloCraftPrincipal principal)
    {
        var result = AuthenticateInternal(username, password, persistCookie, logUserLogin, out principal);

        if (result == AuthenticationResult.Authenticated)
        {
            UserContext.UserContext.Current.ClearUserInfoCookies();
        }

        return result;
    }
}