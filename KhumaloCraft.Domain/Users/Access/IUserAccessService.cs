namespace KhumaloCraft.Domain.Users.Access;

public interface IUserAccessService
{
    LoginResult Login(LoginParameters loginParameters);
    RegistrationResult Register(RegistrationParameters registrationParameters);
    bool SetPassword(string password, string username, string token, out string error);
    VerifyAccountResult VerifyAccount(string username, string loginUrl, PassThroughParameters parameters);
}