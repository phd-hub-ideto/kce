using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Domain.Users.Access;

public class LoginResult
{
    public bool Success { get; set; }
    public LoginError Error { get; set; }
    public KhumaloCraftPrincipal KhumaloCraftPrincipal { get; set; }
}

public enum LoginError
{
    NotAuthenticated = 1,
    UserRequiresActivation = 2,
    PasswordRequired = 3,
    AccountExists = 4,
    AccountDoesNotExist = 5
}