using KhumaloCraft.Dependencies;
using System.Net.Http.Headers;

namespace KhumaloCraft.Application.Authentication.Basic;

[Singleton(Contract = typeof(IBasicAuthenticationHelper))]
public class BasicAuthenticationHelper : IBasicAuthenticationHelper
{
    public AuthorizationParseResult TryParse(AuthenticationHeaderValue authorization, out string username, out string password)
    {
        if (string.IsNullOrEmpty(authorization?.Parameter))
        {
            username = string.Empty;
            password = string.Empty;

            return AuthorizationParseResult.MissingCredentials;
        }

        if (!string.Equals(authorization.Scheme, "basic", StringComparison.InvariantCultureIgnoreCase))
        {
            username = string.Empty;
            password = string.Empty;

            return AuthorizationParseResult.InvalidBasicScheme;
        }

        if (TryGetUsernamePassword(authorization, out username, out password))
        {
            return AuthorizationParseResult.Success;
        }

        username = string.Empty;
        password = string.Empty;

        return AuthorizationParseResult.Failure;

    }

    private bool TryGetUsernamePassword(AuthenticationHeaderValue authorization, out string username, out string password)
    {
        username = null;
        password = null;

        if (string.IsNullOrEmpty(authorization.Parameter))
        {
            return false;
        }

        var bytes = Convert.FromBase64String(authorization.Parameter);

        var rawDetails = System.Text.Encoding.ASCII.GetString(bytes);
        var details = rawDetails.Split(':');

        if (details.Any() && details.Length == 2)
        {
            username = details[0];
            password = details[1];

            return true;
        }

        return false;
    }
}