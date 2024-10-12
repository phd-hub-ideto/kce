namespace KhumaloCraft.Domain.Users.Access.External;

public class SignInResult
{
    public ProfileInformation ProfileInfo { get; }

    //TODO-LP Implement OAuth2
    //public OAuth2.TokenResponse TokenResponse { get; }

    public string ErrorMessage { get; }

    public bool HasError { get; }

    public SignInResult(ProfileInformation profileInfo)
    {
        ProfileInfo = profileInfo;
        ErrorMessage = string.Empty;
        HasError = false;
    }

    public SignInResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        HasError = true;
    }
}