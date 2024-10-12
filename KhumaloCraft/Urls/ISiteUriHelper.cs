using KhumaloCraft.Helpers;

namespace KhumaloCraft.Urls;

public interface ISiteUriHelper
{
    Uri PortalBaseUri { get; }
    string PortalBaseUrl { get; }
    Uri GetSiteUri(string pathAndQuery = "");
    void ValidateUri(Uri uri, params string[] queryStringTokens);
    Uri GenerateValidatableUri(Uri uri, params string[] queryStringTokens);
    string GenerateValidatableUrl(string path, params NameValuePair[] nvp);
}