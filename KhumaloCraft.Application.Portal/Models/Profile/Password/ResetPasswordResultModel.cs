namespace KhumaloCraft.Application.Portal.Models.Profile.Password;

public class ResetPasswordResultModel
{
    public bool IsError { get; private set; }
    public string Message { get; private set; }
    public string LoginUrl { get; set; }

    public static ResetPasswordResultModel Success(string message)
    {
        return new ResetPasswordResultModel
        {
            IsError = false,
            Message = message,
        };
    }

    public static ResetPasswordResultModel Error(string message)
    {
        return new ResetPasswordResultModel
        {
            IsError = true,
            Message = message,
        };
    }
}