using KhumaloCraft.Domain.Authentication;
using KhumaloCraft.Domain.Authentication.AccessTokens;
using KhumaloCraft.Domain.Authentication.Passwords;
using KhumaloCraft.Domain.Users;
using System.Security.Principal;

namespace KhumaloCraft.Domain.Security;

public class KhumaloCraftPrincipal : GenericPrincipal, IPermissionSource
{
    private readonly IUserRolePermissionRepository _userRolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    protected KhumaloCraftPrincipal(User user, IUserRolePermissionRepository userRolePermissionRepository, IUserRoleRepository userRoleRepository)
        : base(new KhumaloCraftIdentity(user.Username), [])
    {
        Username = user.Username;
        Permissions = new PermissionManager(this);
        User = user;
        _userRolePermissionRepository = userRolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public PermissionManager Permissions { get; private set; }

    public User User { get; }

    public int UserId => User.Id.Value;

    private IEnumerable<PermissionKey> _userPermissions;

    public IEnumerable<PermissionKey> UserPermissions => _userPermissions ??= _userRolePermissionRepository.FetchByUserId(UserId);

    int IPermissionSource.UserId => User.Id.Value;

    public string Username { get; private set; }

    public static bool TryAuthenticate(
        IPasswordValidator passwordValidator,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        string username, string password, out KhumaloCraftPrincipal principal)
    {
        var user = passwordValidator.ValidatePassword(username, password);

        if (user?.IsActivated == true)
        {
            principal = Authenticate(
                user,
                userRolePermissionRepository,
                userRoleRepository,
                userRepository);

            return true;
        }

        principal = null;

        return false;
    }

    public static KhumaloCraftPrincipal Authenticate(
        User user,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository)
    {
        if (!user.IsActivated)
        {
            throw SecurityExceptions.AccountNotActivated();
        }

        if (user.LastLoginDate != DateTime.Today)
        {
            userRepository.UpdateLastLoginDate(user.Id.Value);
        }

        var principal = new KhumaloCraftPrincipal(user, userRolePermissionRepository, userRoleRepository);

        PrincipalResolver.Instance.SetPrincipal(principal);

        return principal;
    }

    public static KhumaloCraftPrincipal Create(User user, IUserRolePermissionRepository userRolePermissionRepository, IUserRoleRepository userRoleRepository)
    {
        var principal = new KhumaloCraftPrincipal(user, userRolePermissionRepository, userRoleRepository);
        return principal;
    }

    public static KhumaloCraftPrincipal Authenticate(
        IPasswordValidator passwordValidator,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        string username, string password)
    {
        if (!TryAuthenticate(
            passwordValidator,
            userRolePermissionRepository,
            userRoleRepository,
            userRepository,
            username, password,
            out var principal))
        {
            throw SecurityExceptions.NotAuthenticated();
        }

        return principal;
    }

    public static string CreateAccessToken(User user)
    {
        return AccessToken.Generate(user.PasswordHash);
    }

    public bool IsSuperUser()
    {
        return Permissions.HasPermission(AdministratorPermission.SuperUser);
    }

    public static bool HasAnyAdministrativePermission()
    {
        return PrincipalResolver.Instance.TryResolveCurrentPrincipal(out var principal)
            && principal.Permissions.HasAnyAdministrativePermission();
    }

    public void RequirePermission(UserPermission permission)
    {
        this.Permissions.Require(permission);
    }

    public bool HasPermission(AdministratorPermission permission)
    {
        return Permissions.HasPermission(permission);
    }

    public bool HasPermission(UserPermission permission)
    {
        return Permissions.HasPermission(permission);
    }
}