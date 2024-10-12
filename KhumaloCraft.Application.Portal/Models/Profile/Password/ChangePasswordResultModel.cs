namespace KhumaloCraft.Application.Portal.Models.Profile.Password;

public class ChangePasswordResultModel
{
    public bool IsError { get; private set; }
    public string Message { get; private set; }
    public string ReturnUrl { get; private set; }
    public bool IsLoggedIn { get; private set; }
    public bool PasswordResetOrActivationMailResent { get; private set; }

    public static ChangePasswordResultModel Success(
        string message,
        string returnUrl = null,
        bool isLoggedIn = false)
    {
        return new ChangePasswordResultModel
        {
            Message = message,
            ReturnUrl = returnUrl,
            IsLoggedIn = isLoggedIn,

        };
    }

    public static ChangePasswordResultModel Error(
        string message,
        bool passwordResetOrActivationResent = false,
        string returnUrl = null)
    {
        return new ChangePasswordResultModel
        {
            IsError = true,
            Message = message,
            PasswordResetOrActivationMailResent = passwordResetOrActivationResent,
            ReturnUrl = returnUrl
        };
    }
}