namespace KhumaloCraft.Application.Models.Shared;

public class RedirectResultModel
{
    public string RedirectUrl { get; }

    public RedirectResultModel(string redirectUrl)
    {
        RedirectUrl = redirectUrl;
    }
}