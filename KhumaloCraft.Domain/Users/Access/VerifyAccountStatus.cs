namespace KhumaloCraft.Domain.Users.Access;

public enum VerifyAccountStatus
{
    UserRequiresActivation = 1,
    AccountCreated = 2,
    PasswordRequired = 3,
    AccountExists = 4,
    AccountCreationError = 5,
    ExistingAuthentication = 6
}