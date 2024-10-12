using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Authentication.AccessTokens;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Utilities;

namespace KhumaloCraft.Domain.Authentication.Passwords;

[Singleton(Contract = typeof(IPasswordValidator))]
public sealed class PasswordValidator(
    IUserRepository userRepository) : IPasswordValidator
{
    public bool TryValidatePassword(string username, string password, bool logUserLogin, out User user, out bool userRequiresActivation)
    {
        user = null;
        userRequiresActivation = false;

        var candidateUser = ValidatePassword(username, password);

        if (candidateUser?.IsActivated == true)
        {
            if (logUserLogin)
            {
                userRepository.UpdateLastLoginDate(candidateUser.Id.Value);
            }

            user = candidateUser;
        }
        else if (candidateUser?.IsActivated == false)
        {
            userRequiresActivation = true;
        }

        return user != null;
    }

    /// <summary>
    /// validates the user's password
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password">the users unecrypted password or auth token</param>
    /// <returns>the user if password is valid or null</returns>
    public User ValidatePassword(string username, string passwordOrToken)
    {
        var user = userRepository.Query().SingleOrDefault(u => u.Username == username);

        if (user != null)
        {
            return ValidatePassword(user, passwordOrToken);
        }

        return null;
    }

    private User ValidatePassword(User user, string passwordOrToken)
    {
        if (TryValidatePassword(user, passwordOrToken))
        {
            return user;
        }

        return null;
    }

    internal bool TryValidatePassword(User user, string passwordOrToken)
    {
        // Hash the provided password and compare it to that of the user
        var passwordHash = SecurityHelper.GenerateWeakHash(passwordOrToken, user.PasswordSalt);

        if (ByteArrayUtils.Compare(passwordHash, user.PasswordHash))
        {
            return true;
        }

        // CHECK IF THE PASSWORD IS A TOKEN
        var token = AccessToken.TryDecode(passwordOrToken);

        if (token != null)
        {
            if (token.PasswordHash != SecurityHelper.BytesToString(user.PasswordHash))
            {
                return false;
            }

            return true;
        }

        return false;
    }
}