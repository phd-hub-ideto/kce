using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Domain.Authentication.Passwords;

public interface IPasswordValidator
{
    bool TryValidatePassword(string username, string password, bool logUserLogin, out User user, out bool userRequiresActivation);
    User ValidatePassword(string username, string passwordOrToken);
}