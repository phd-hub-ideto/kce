using KhumaloCraft.Application.Validation;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Domain.Users.Access;
using System.Security;
using System.Transactions;

namespace KhumaloCraft.Application.Users;

[Singleton(Contract = typeof(IUserAccessService))]
public class UserAccessService : IUserAccessService
{
    private readonly LoginManager.LoginManager _loginManager;
    private readonly IUserRepository _userRepository;
    private readonly FullUriBuilder _fullUriBuilder;
    private readonly ISettings _settings;
    private readonly IResetPasswordHasher _resetPasswordHasher;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserAccessService(
        IUserRepository userRepository,
        LoginManager.LoginManager loginManager,
        FullUriBuilder fullUriBuilder,
        ISettings settings,
        IResetPasswordHasher resetPasswordHasher,
        IUserRoleRepository userRoleRepository)
    {
        _loginManager = loginManager;
        _userRepository = userRepository;
        _fullUriBuilder = fullUriBuilder;
        _settings = settings;
        _resetPasswordHasher = resetPasswordHasher;
        _userRoleRepository = userRoleRepository;
    }

    public LoginResult Login(LoginParameters loginParameters)
    {
        if (_userRepository.ExistsByUsername(loginParameters.Username) && !string.IsNullOrWhiteSpace(loginParameters.Password))
        {
            return Authenticate(loginParameters);
        }

        return new LoginResult { Success = false, Error = LoginError.AccountDoesNotExist };
    }

    public Domain.Users.Access.RegistrationResult Register(RegistrationParameters registrationParameters)
    {
        if (_userRepository.ExistsByUsername(registrationParameters.Username))
        {
            return new Domain.Users.Access.RegistrationResult
            {
                Success = false,
                Errors = [RegistrationError.UserAlreadyExists]
            };
        }

        using var scope = new TransactionScope();

        var userAccountManager = new UserAccountManager(
            registrationParameters.Username,
            loginUrl: UserHelper.GetLoginUrl(_settings));

        var token = userAccountManager.GetResetPasswordToken();

        RegistrationResult result;

        if (registrationParameters.NoPasswordRegistration)
        {
            result = userAccountManager.RegisterWithNoPassword(
                _resetPasswordHasher,
                buildAccountActivationUri: (username, resetAccountToken, returnUrl) => _fullUriBuilder.BuildAccountActivationUriForRegistrationWithNoPassword(username, resetAccountToken, returnUrl, registrationParameters.PassThroughParameters),
                firstname: registrationParameters.FirstName,
                lastname: registrationParameters.LastName,
                mobile: registrationParameters.MobileNumber,
                returnUrl: registrationParameters.PassThroughParameters.ReturnUrl);
        }
        else
        {
            result = userAccountManager.Register(
                fullUriBuilder: _fullUriBuilder,
                password: registrationParameters.Password,
                enforcePasswordStrength: true,
                firstname: registrationParameters.FirstName,
                lastname: registrationParameters.LastName,
                mobile: registrationParameters.MobileNumber,
                emailActivate: true,
                passThroughParameters: registrationParameters.PassThroughParameters);
        }

        if (result.IsSuccess)
        {
            var user = _userRepository.Query().Single(u => u.Id == result.UserId);

            user.FirstName = registrationParameters.FirstName;
            user.LastName = registrationParameters.LastName;
            user.MobileNumber = registrationParameters.MobileNumber;

            _userRepository.Upsert(user);

            //Assign KC User Role
            var userRole = new UserRole
            {
                UserId = user.Id.Value,
                RoleId = (int)Role.KCUser,
                SecurityEntityType = SecurityEntityType.User
            };

            _userRoleRepository.Upsert(userRole);

            scope.Complete();

            return new Domain.Users.Access.RegistrationResult
            {
                UserId = user.Id,
                Success = true
            };
        }

        return new Domain.Users.Access.RegistrationResult
        {
            Success = false,
            Errors = [Map(result.Status)]
        };
    }

    public bool SetPassword(
        string password,
        string username,
        string token,
        out string error)
    {
        var userAccountManager = new UserAccountManager(username);

        if (!userAccountManager.IsRegistered())
        {
            error = "Unknown username specified.";

            return false;
        }

        if (!userAccountManager.IsActivated())
        {
            error = "Account not activated.";

            return false;
        }

        if (!userAccountManager.IsValidResetPasswordToken(token, _resetPasswordHasher))
        {
            error = "Security token has expired, or is invalid.";

            return false;
        }

        var passwordComplexity = PasswordValidator.IsPasswordComplex(password);

        if (passwordComplexity.Status != PasswordComplexityInValidationReason.None)
        {
            error = passwordComplexity.Message;

            return false;
        }

        using (var scope = new TransactionScope())
        {
            userAccountManager.TryChangePassword(password, true);

            scope.Complete();
        }

        error = null;

        return true;
    }

    private LoginResult Authenticate(LoginParameters loginParameters)
    {
        try
        {
            var authResult = _loginManager.Authenticate(
                loginParameters.Username,
                loginParameters.Password,
                loginParameters.PersistCookie,
                loginParameters.LogUserLogin,
                out var principal);

            return authResult switch
            {
                LoginManager.AuthenticationResult.Authenticated => new LoginResult { Success = true, KhumaloCraftPrincipal = principal },
                LoginManager.AuthenticationResult.UserRequiresActivation => new LoginResult { Success = false, Error = LoginError.UserRequiresActivation },
                _ => throw new ArgumentOutOfRangeException(nameof(authResult), authResult, "Could not map Authentication Result"),
            };
        }
        catch (SecurityException)
        {
            return new LoginResult { Success = false, Error = LoginError.NotAuthenticated };
        }
    }

    private RegistrationError Map(RegistrationStatus registrationStatus)
    {
        return registrationStatus switch
        {
            RegistrationStatus.InvalidFirstNameOrLastName => RegistrationError.InvalidFirstNameOrLastName,
            RegistrationStatus.InvalidPassword => RegistrationError.InvalidPassword,
            RegistrationStatus.UserAlreadyExists => RegistrationError.UserAlreadyExists,
            _ => throw new ArgumentOutOfRangeException(nameof(registrationStatus), registrationStatus, "Could not map RegistrationStatus"),
        };
    }

    private LoginSource Map(LoginSource loginSource)
    {
        switch (loginSource)
        {
            case LoginSource.Manage:
                return LoginSource.Manage;

            case LoginSource.Mobile:
                return LoginSource.Mobile;

            case LoginSource.Portal:
                return LoginSource.Portal;

            default:
                throw new ArgumentOutOfRangeException(nameof(loginSource), loginSource, "Could not map LoginSource");
        }
    }

    public VerifyAccountResult VerifyAccount(string username, string loginUrl, PassThroughParameters parameters)
    {
        var user = _userRepository.Query().SingleOrDefault(u => u.Username == username);

        if (user != null)
        {
            var userAccountManager = new UserAccountManager(
                    username,
                    loginUrl: UserHelper.GetLoginUrl(_settings));

            if (user.IsRegistered && user.IsActivated)
            {
                //TODO-LP : Send SendAccountPasswordActivation email
                userAccountManager.SendAccountPasswordActivation(_fullUriBuilder, _resetPasswordHasher);

                return new VerifyAccountResult { Success = true, State = VerifyAccountStatus.PasswordRequired };
            }
            else if (user.IsRegistered && !user.IsActivated)
            {
                userAccountManager.RegisterWithNoPassword(
                    _resetPasswordHasher,
                    buildAccountActivationUri: (username, accountRegisterToken, returnUrl) => _fullUriBuilder.BuildAccountActivationUriForRegistrationWithNoPassword(username, accountRegisterToken, returnUrl, parameters),
                    returnUrl: parameters.ReturnUrl);

                return new VerifyAccountResult { Success = true, State = VerifyAccountStatus.UserRequiresActivation };
            }
            else if (user.IsRegistered && user.IsActivated)
            {
                return new VerifyAccountResult { Success = true, State = VerifyAccountStatus.AccountExists };
            }
        }

        var manager = new UserAccountManager(
                        username,
                        UserHelper.GetLoginUrl(_settings));

        var registrationResult = manager.RegisterWithNoPassword(
            _resetPasswordHasher,
            buildAccountActivationUri: (username, accountRegisterToken, returnUrl) => _fullUriBuilder.BuildAccountActivationUriForRegistrationWithNoPassword(username, accountRegisterToken, returnUrl, parameters),
            returnUrl: parameters.ReturnUrl
        );

        if (registrationResult.IsSuccess)
        {
            return new VerifyAccountResult { Success = true, State = VerifyAccountStatus.AccountCreated };
        }
        else
        {
            return new VerifyAccountResult { Success = false, State = VerifyAccountStatus.AccountCreationError, Message = registrationResult.Message };
        }
    }
}