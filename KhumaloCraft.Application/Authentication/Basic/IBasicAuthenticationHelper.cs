using System.Net.Http.Headers;

namespace KhumaloCraft.Application.Authentication.Basic;

public interface IBasicAuthenticationHelper
{
    AuthorizationParseResult TryParse(AuthenticationHeaderValue authorization, out string username, out string password);
}