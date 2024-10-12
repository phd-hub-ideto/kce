namespace KhumaloCraft.Domain.Security;

public interface IResetPasswordHasher
{
    string GenerateResetToken(string token, string username, string returnUrl = null);
    bool IsValidToken(string username, string token, string returnUrl = null);
}