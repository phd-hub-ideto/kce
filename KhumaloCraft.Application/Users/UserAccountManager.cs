using KhumaloCraft.Application.Exceptions;
using KhumaloCraft.Application.Validation;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Security.Role;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Domain.Users.Access;
using KhumaloCraft.Helpers;
using System.Transactions;
using static KhumaloCraft.Application.Validation.PasswordValidator;

namespace KhumaloCraft.Application.Users;

//TODO-LP : Make the class a Singleton ???
public class UserAccountManager
{
    private string LoginUrl { get; set; }
    public string UserName { get; private set; }
    public User User { get; private set; }

    public UserAccountManager(string userName, string loginUrl = null)
    {
        LoginUrl = loginUrl;

        if (string.IsNullOrEmpty(userName))
        {
            throw new ClientException("Email address required.");
        }

        if (!ValidationHelper.IsValidEmailAddress(userName))
        {
            throw new ClientException("Invalid email address (username) " + userName);
        }

        UserName = userName;

        var userRepository = DependencyManager.Container.GetInstance<IUserRepository>();

        User = userRepository.Query().SingleOrDefault(u => u.Username == userName);
    }

    public UserAccountManager(User user)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        UserName = user.Username;
    }

    public User FetchOrCreate()
    {
        if (!IsRegistered())
        {
            if (User == null)
            {
                User = new User(UserName);
            }
            else
            {
                User.Reset();
            }

            SaveUser();
        }

        return User;
    }

    public RegistrationResult Register(
        FullUriBuilder fullUriBuilder,
        string password,
        bool enforcePasswordStrength,
        PassThroughParameters passThroughParameters,
        string firstname = "",
        string lastname = "",
        string mobile = "",
        bool emailActivate = true)
    {
        var result = ValidateRegistrationParameters(password, enforcePasswordStrength);

        if (result != null)
            return result;

        using var scope = new TransactionScope();

        var userResult = FetchOrCreateUser(password, mobile, firstname, lastname);

        if (userResult != null)
            return userResult;

        if (emailActivate)
        {
            SendActivation(fullUriBuilder, passThroughParameters);
        }

        scope.Complete();

        return RegistrationResult.Success(User.Id.Value);
    }

    public RegistrationResult RegisterWithNoPassword(
        IResetPasswordHasher resetPasswordHasher,
        Func<string, string, string, Uri> buildAccountActivationUri,
        string firstname = "",
        string lastname = "",
        string mobile = "",
        string returnUrl = null)
    {
        using var scope = new TransactionScope();

        var userResult = FetchOrCreateUser(null, mobile, firstname, lastname);

        if (userResult != null)
            return userResult;

        var resetAcccountToken = resetPasswordHasher.GenerateResetToken(GetResetPasswordToken(), User.Username, returnUrl: returnUrl);

        var activateUrl = buildAccountActivationUri(UserName, resetAcccountToken, returnUrl).AbsoluteUri;

        SendActivation(activateUrl);

        scope.Complete();

        return RegistrationResult.Success(User.Id.Value);
    }

    public RegistrationResult SendActivationEmailForRegisteredUser(
        Func<string, byte[], Uri> buildAccountActivationUri,
        string firstname = "",
        string lastname = "",
        string mobile = "")
    {
        using var scope = new TransactionScope();

        var userResult = FetchOrCreateUser(null, mobile, firstname, lastname);

        var activateUrl = buildAccountActivationUri(UserName, User.PasswordSalt).AbsoluteUri;

        SendActivation(activateUrl);

        scope.Complete();

        return RegistrationResult.Success(User.Id.Value);
    }

    public RegistrationResult Register(string firstname, string lastname)
    {
        using var scope = new TransactionScope();

        var userResult = FetchOrCreateUser(null, null, firstname, lastname);

        if (userResult != null)
            return userResult;

        scope.Complete();

        return RegistrationResult.Success(User.Id.Value);
    }

    public RegistrationResult Register(
        FullUriBuilder fullUriBuilder,
        string password, bool enforcePasswordStrength,
        string firstname = "", string lastname = "",
        string mobile = "", bool emailActivate = true,
        string returnUrl = null, string returnUrlDescription = null)
    {
        var result = ValidateRegistrationParameters(password, enforcePasswordStrength);

        if (result != null)
            return result;

        using var scope = new TransactionScope();

        var userResult = FetchOrCreateUser(password, mobile, firstname, lastname);

        if (userResult != null)
            return userResult;

        if (emailActivate)
        {
            SendActivation(fullUriBuilder, returnUrl, returnUrlDescription);
        }

        scope.Complete();

        return RegistrationResult.Success(User.Id.Value);
    }

    private RegistrationResult ValidateRegistrationParameters(string password, bool enforcePasswordStrength)
    {
        if (!string.IsNullOrWhiteSpace(password))
        {
            if (enforcePasswordStrength)
            {
                var result = IsPasswordComplex(password);

                if (result.Status != PasswordComplexityInValidationReason.None)
                {
                    return RegistrationResult.Failure(RegistrationStatus.InvalidPassword, result.Message);
                }
            }
        }

        return null;
    }

    private RegistrationResult FetchOrCreateUser(
        string password, string mobile,
        string firstname, string lastname)
    {
        FetchOrCreate();

        if (IsActivated())
        {
            return RegistrationResult.Failure(RegistrationStatus.UserAlreadyExists, $"Users {UserName} already exists.");
        }

        if (string.IsNullOrWhiteSpace(User.FirstName) || !string.IsNullOrWhiteSpace(firstname))
        {
            User.FirstName = firstname;
        }

        if (string.IsNullOrWhiteSpace(User.LastName) || !string.IsNullOrWhiteSpace(lastname))
        {
            User.LastName = lastname;
        }

        if (string.IsNullOrWhiteSpace(User.MobileNumber) || !string.IsNullOrWhiteSpace(mobile))
        {
            User.MobileNumber = mobile;
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
            User.SetPassword(password);
        }

        SaveUser();

        return null;
    }

    public void SendActivation(
        FullUriBuilder fullUriBuilder,
        string returnUrl = null,
        string returnUrlDescription = null)
    {
        var resetPasswordHasher = DependencyManager.Container.GetInstance<IResetPasswordHasher>();

        string activateUrl = BuildActivateUrl(
            fullUriBuilder,
            new PassThroughParameters()
            {
                ReturnUrl = returnUrl,
                ReturnUrlDescription = returnUrlDescription
            }, resetPasswordHasher);

        SendActivation(activateUrl);
    }

    private void SendActivation(FullUriBuilder fullUriBuilder, PassThroughParameters returnParameters)
    {
        var passwordResetHasher = DependencyManager.Container.GetInstance<IResetPasswordHasher>();

        string activateUrl = BuildActivateUrl(fullUriBuilder, returnParameters, passwordResetHasher);

        SendActivation(fullUriBuilder, activateUrl);
    }

    private void SendActivation(string activateUrl)
    {
        var token = GetResetPasswordToken();

        if (string.IsNullOrEmpty(token))
        {
            throw new UserNotRegisteredException("Users is not registered or deleted.");
        }

        /* TODO-LP : Enable / Implement SendActivation
        if (!User.IsActivated || (User.IsActivated && User.RequiresPassword))
        {
            var userAccountEmailModel = new UserAccountActivationEmailModel
            {
                UserEmail = User.Username,
                UserFullName = string.IsNullOrEmpty(User.Fullname) ? "Users" : User.Fullname,
                UserId = User.Id.Value,
                ActivateUrl = activateUrl,
                LoginUrl = LoginUrl
            };

            userAccountEmailModel.To.Add(User.EMailAddress);
            userAccountEmailModel.EnqueueMessage();
        }

        var tmpUser = Domain.User.FetchByUserId(User.Id.Value);
        tmpUser.ActivationEmailSentDate = DateTime.Now;
        tmpUser.Save();
        */
    }

    public string BuildActivateUrl(FullUriBuilder fullUriBuilder,
                                   PassThroughParameters returnParameters,
                                   IResetPasswordHasher resetPasswordHasher)
    {
        var token = GetResetPasswordToken();

        string activateUrl;

        var accountResetToken = resetPasswordHasher.GenerateResetToken(token, User.Username, returnUrl: returnParameters?.ReturnUrl);

        if (returnParameters == null)
        {
            activateUrl = fullUriBuilder.BuildAccountActivationUri(User.Username, User.PasswordSalt, accountResetToken).AbsoluteUri;
        }
        else
        {
            activateUrl = fullUriBuilder.BuildAccountActivationUri(User.Username, User.PasswordSalt, accountResetToken, returnParameters).AbsoluteUri;
        }

        return activateUrl;
    }

    /// <summary>
    /// Checks a user is registered. A user is registered when they have an entry in our Users table that has not been marked as deleted. This
    /// does not necessarily mean that they are activated yet.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool IsRegistered()
    {
        return User?.Deleted == false;
    }

    public void ResetPassword(
        FullUriBuilder fullUriBuilder,
        IResetPasswordHasher resetPasswordHasher,
        Uri resetUrl = null)
    {
        var token = GetResetPasswordToken(); // checks IsRegistered

        if (string.IsNullOrEmpty(token))
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        var passwordResetResult = resetPasswordHasher.GenerateResetToken(token, User.Username);

        /* TODO-LP : Implement sending of ResetPassword email
        var userResetPasswordEmailModel = new UserResetPasswordEmailModel
        {
            ResetUrl = resetUrl?.AbsoluteUri ?? fullUriBuilder.BuildPasswordChangeUri(User.EMailAddress, passwordResetResult).AbsoluteUri,
            LoginUrl = fullUriBuilder.GetLoginUrl,
            Token = token,
            UserEmail = User.EMailAddress,
            UserFullName = User.FriendlyName,
            UserId = User.Id.Value
        };

        userResetPasswordEmailModel.To.Add(User.EMailAddress);
        userResetPasswordEmailModel.EnqueueMessage();
        */
    }

    public void SendAccountPasswordActivation(
        FullUriBuilder fullUriBuilder,
        IResetPasswordHasher resetPasswordHasher,
        Uri resetUrl = null)
    {
        var token = GetResetPasswordToken();

        if (string.IsNullOrEmpty(token))
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        var resetToken = resetPasswordHasher.GenerateResetToken(token, User.Username);

        /* TODO-LP : Implement sending of SendAccountPasswordActivation email
        var accountPasswordActivationModel = new AccountPasswordActivationEmailModel
        {
            ResetUrl = resetUrl?.AbsoluteUri ?? fullUriBuilder.BuildPasswordChangeUri(User.EMailAddress, resetToken).AbsoluteUri,
            Token = token,
            UserEmail = User.EMailAddress,
            UserFullName = User.FriendlyName,
            UserId = User.Id.Value,
            UIContextId = UIContextId,
            LoginUrl = LoginUrl
        };

        accountPasswordActivationModel.To.Add(User.EMailAddress);
        accountPasswordActivationModel.EnqueueMessage();
        */
    }

    /// <summary>
    /// Only changes the user's password. Does not validate the email address or activate the user.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="newPassword"></param>
    public void ChangePassword(string newPassword, bool enforcePasswordStrength)
    {
        if (!IsRegistered())
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        if (enforcePasswordStrength)
        {
            var result = IsPasswordComplex(newPassword);

            if (result.Status != PasswordComplexityInValidationReason.None)
            {
                throw new ClientException(result.Message);
            }
        }

        var userService = DependencyManager.Container.GetInstance<Domain.Users.IUserService>();

        userService.UpdatePassword(User, newPassword);
    }

    public PasswordComplexityValidationResult TryChangePassword(string newPassword, bool enforcePasswordStrength)
    {
        if (!IsRegistered())
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        PasswordComplexityValidationResult result;

        if (enforcePasswordStrength)
        {
            result = IsPasswordComplex(newPassword);
            if (result.Status != PasswordComplexityInValidationReason.None)
            {
                return result;
            }
        }
        else
        {
            result = new PasswordComplexityValidationResult
            {
                Status = PasswordComplexityInValidationReason.None,
            };
        }

        var userService = DependencyManager.Container.GetInstance<Domain.Users.IUserService>();

        userService.UpdatePassword(User, newPassword);

        return result;
    }

    public bool IsValidated()
    {
        if (IsRegistered())
        {
            return User.ValidatedEmail;
        }

        return false;
    }

    /// <summary>
    /// Validates email address as being owned by the user. This must be called once we have proven that the user ownes the email
    /// address they entered. Usually by clicking a link in an email sent to them.
    /// </summary>
    /// <param name="user"></param>
    public void ValidateEmail()
    {
        if (IsRegistered())
        {
            User.ValidatedEmail = true;

            SaveUser();
        }
    }

    public bool IsActivated()
    {
        if (IsRegistered())
        {
            return User.IsActivated;
        }

        return false;
    }

    /// <summary>
    /// Activates user and validates email. A user is active once we have validated that they own the email address that they have registered.
    /// </summary>
    public void ActivateUser()
    {
        if (!IsRegistered())
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        if (User.ActivatedDate == null)
        {
            User.ActivatedDate = DomainScope.DBServerDate();
        }

        ValidateEmail();

        User.Deleted = false;

        SaveUser();
    }

    public string GetResetPasswordToken()
    {
        if (IsRegistered())
        {
            return SecurityHelper.GenerateHashForResetPassword(User.PasswordSalt);
        }

        return string.Empty;
    }

    public UserRole AddUserRole(
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository, int roleId,
        SecurityEntityType entityType)
    {
        if (!IsRegistered())
        {
            throw Exceptions.UserIsNotRegistered(UserName);
        }

        var role = roleRepository.Query().SingleOrDefault(r => r.Id == roleId);

        var userRoles = userRoleRepository.Query()
                            .Where(ur => ur.UserId == User.Id.Value)
                            .ToList();

        var userRole = new UserRole
        {
            UserId = User.Id.Value,
            RoleId = roleId,
            SecurityEntityType = entityType
        };

        if (userRoles.Contains(userRole))
        {
            throw Exceptions.RoleAlreadyExists(UserName, role.Name, entityType);
        }

        SecurityHelper.AddRolesToUser(userRoleRepository, User.Id.Value, userRole.SecurityEntityType, roleId);

        return userRole;
    }

    public void ValidateResetPasswordToken(string token)
    {
        if (!IsValidResetPasswordToken(token))
        {
            throw Exceptions.TokenIsInvalid();
        }
    }

    public bool IsValidResetPasswordToken(string token)
    {
        var tokenInfo = token.Split('|');

        return SecurityHelper.GenerateHashForResetPassword(User.PasswordSalt) == tokenInfo[0];
    }

    public void ValidateResetPasswordToken(string token, IResetPasswordHasher resetPasswordHasher, string returnUrl = null)
    {
        if (!IsValidResetPasswordToken(token, resetPasswordHasher, returnUrl))
        {
            throw Exceptions.TokenIsInvalid();
        }
    }

    public bool IsValidResetPasswordToken(string token, IResetPasswordHasher resetPasswordHasher, string returnUrl = null)
    {
        return resetPasswordHasher.IsValidToken(User.Username, token, returnUrl);
    }

    public void SendDeleteAccountEmail(
        IResetPasswordHasher resetPasswordHasher,
        FullUriBuilder fullUriBuilder,
        string deleteAccountResultUri)
    {
        string uri = BuildDeleteAccountUrl(
            fullUriBuilder,
            resetPasswordHasher,
            deleteAccountResultUri);

        SendDeleteAccountEmail(uri);
    }

    private void SendDeleteAccountEmail(string deleteProfileUrl)
    {
        /* TODO : Send Delete Account Confirmation Email
        var userAccountEmailModel = new UserAccountDeletionEmailModel
        {
            UserEmail = User.Username,
            UserFullName = string.IsNullOrEmpty(User.Fullname) ? "Users" : User.Fullname,
            UserId = User.Id.Value,
            DeleteProfileUrl = deleteProfileUrl
        };

        userAccountEmailModel.To.Add(UserUsername);

        userAccountEmailModel.EnqueueMessage();*/
    }

    public string BuildDeleteAccountUrl(
        FullUriBuilder fullUriBuilder,
        IResetPasswordHasher resetPasswordHasher,
        string deleteAccountResultUri)
    {
        var token = GetResetPasswordToken();

        var accountResetToken = resetPasswordHasher.GenerateResetToken(token, User.Username, returnUrl: null);

        return fullUriBuilder.BuildAccountDeletionUri(User.Id.Value, accountResetToken, deleteAccountResultUri).AbsoluteUri;
    }

    private void SaveUser()
    {
        var userRepository = DependencyManager.Container.GetInstance<IUserRepository>();

        userRepository.Upsert(User);
    }

    public static class Exceptions
    {
        internal static Exception UserIsNotRegistered(string emailAddress)
        {
            return new UserNotRegisteredException($"Email address {emailAddress} is not registered.");
        }

        public static Exception UserAlreadyRegistered(string emailAddress)
        {
            return new Exception($"Users {emailAddress} already exists.");
        }

        internal static RoleException RoleAlreadyExists(string userName, string roleName, SecurityEntityType entityType)
        {
            throw new RoleException($"Users '{userName}' already has the role {roleName} for entity {entityType.GetBestDescription()}.");
        }

        internal static Exception TokenIsInvalid()
        {
            throw new Exception("Security token is invalid.");
        }
    }
}