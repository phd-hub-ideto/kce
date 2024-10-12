namespace KhumaloCraft.Application.Portal.Models.Account.Activation;

public class ActivateUserAccountModel
{
    public string Username { get; set; }
    public string Token { get; set; }
    public string LoginUrl { get; set; }
    public string ReturnUrl { get; set; }
}