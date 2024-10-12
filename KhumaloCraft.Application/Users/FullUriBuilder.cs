using KhumaloCraft.Application.Http;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Users.Access;
using KhumaloCraft.Helpers;
using KhumaloCraft.Urls;
using static KhumaloCraft.Constants;

namespace KhumaloCraft.Application.Users;

[Singleton]
public class FullUriBuilder(ISiteUriHelper siteUriHelper)
{
    public Uri BuildAccountActivationUri(string userEmail,
                                         byte[] passwordSalt,
                                         string token,
                                         PassThroughParameters passThroughParameters)
    {
        return BuildAccountActivationUri(userEmail, token, passThroughParameters, Pages.AccountActivationUrl);
    }

    public Uri BuildAccountActivationUriForRegistrationWithNoPassword(
        string userEmail, string token, string resetUrl,
        PassThroughParameters passThroughParameters)
    {
        return BuildAccountActivationUri(userEmail, token, passThroughParameters, Pages.AccountActivationUrl);
    }

    private Uri BuildAccountActivationUri(string userEMail, string token, PassThroughParameters passThroughParameters, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(siteUriHelper.PortalBaseUrl))
        {
            throw new Exception($"{nameof(ISiteUriHelper.PortalBaseUrl)} is not set in the .config file");
        }

        var pairs = QueryStringBuilder.Create()
                .AddToQuery("Username", userEMail)
                .AddToQuery("Token", token)
                .AddToQuery("returnUrl", passThroughParameters?.ReturnUrl, () => !string.IsNullOrEmpty(passThroughParameters?.ReturnUrl))
                .AddToQuery("returnUrlDescription", passThroughParameters?.ReturnUrlDescription, () => !string.IsNullOrEmpty(passThroughParameters?.ReturnUrlDescription))
                .BuildArray();

        return HttpHelper.BuildUri(siteUriHelper.PortalBaseUrl, relativePath, pairs);
    }

    public Uri BuildAccountActivationUri(string userEmail, byte[] passwordSalt, string token)
        => BuildAccountActivationUri(userEmail, passwordSalt, token, null);

    public Uri BuildPasswordResetUri(string userEMail, string resetPasswordHash)
    {
        return HttpHelper.BuildUri(siteUriHelper.PortalBaseUrl, Pages.PasswordResetUrl,
            new NameValuePair("Username", userEMail),
            new NameValuePair("Token", resetPasswordHash));
    }

    public Uri BuildPasswordChangeUri(string userEMail, string resetPasswordHash)
    {
        return HttpHelper.BuildUri(siteUriHelper.PortalBaseUrl, Pages.PasswordChangeUrl,
            new NameValuePair("Username", userEMail),
            new NameValuePair("Token", resetPasswordHash));
    }

    public Uri BuildAccountDeletionUri(int id, string token, string relativepath)
    {
        if (string.IsNullOrWhiteSpace(siteUriHelper.PortalBaseUrl))
        {
            throw new Exception($"{nameof(ISiteUriHelper.PortalBaseUrl)} is not set in the .config file");
        }

        var pairs = QueryStringBuilder.Create()
                .AddToQuery("UserId", id.ToString())
                .AddToQuery("Token", token)
                .BuildArray();

        return HttpHelper.BuildUri(siteUriHelper.PortalBaseUrl, relativepath, pairs);
    }

    public string GetLoginUrl => $"{siteUriHelper.PortalBaseUri.AbsoluteUri}login";
}