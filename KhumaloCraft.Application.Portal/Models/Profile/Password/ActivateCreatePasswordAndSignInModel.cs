namespace KhumaloCraft.Application.Portal.Models.Profile.Password;

public class ActivateCreatePasswordAndSignInModel
{
    public ActivateCreatePasswordAndSignInRequest State { get; set; }

    public static ActivateCreatePasswordAndSignInModel Create(
        string username, string token, string returnUrl)
    {
        return new ActivateCreatePasswordAndSignInModel()
        {
            State = new ActivateCreatePasswordAndSignInRequest()
            {
                ReturnUrl = returnUrl,
                Token = token,
                Username = username,
                RememberMe = true
            }
        };
    }
}