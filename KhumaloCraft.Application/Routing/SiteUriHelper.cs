using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Helpers;
using KhumaloCraft.Urls;
using System.Security;

namespace KhumaloCraft.Application.Routing;

[Singleton(Contract = typeof(ISiteUriHelper))]
public class SiteUriHelper : ISiteUriHelper
{
    private readonly ISettings _settings;

    public SiteUriHelper(ISettings settings)
    {
        _settings = settings;
    }

    public Uri PortalBaseUri => new Uri(PortalBaseUrl);
    public string PortalBaseUrl => _settings.PortalBaseUri ?? Constants.KhumaloCraftUri.AbsoluteUri;

    private Uri GetPortalBaseUri()
    {
        return new Uri(PortalBaseUrl);
    }

    public Uri GetSiteUri(string pathAndQuery = "")
    {
        var baseUri = GetPortalBaseUri();

        if (!string.IsNullOrEmpty(pathAndQuery))
        {
            var uri = new UriBuilder(baseUri);

            HttpHelper.SetPathAndQuery(uri, pathAndQuery);

            return uri.Uri;
        }

        return baseUri;
    }

    public void ValidateUri(Uri uri, params string[] queryStringTokens)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        if (queryStringTokens == null)
            throw new ArgumentNullException(nameof(queryStringTokens));

        var queryStrings = HttpHelper.GetNameValuePairs(uri.Query);

        if (!queryStrings.ContainsKey("Token"))
        {
            throw new Exception("Uri does not contain a token to validate");
        }

        var generatedToken = GenerateToken(queryStringTokens, queryStrings);

        var token = queryStrings["Token"].Value;

        if (generatedToken != token)
        {
            throw new SecurityException("Uri was not valid.");
        }
    }

    private string GenerateToken(IEnumerable<string> queryStringTokens, Dictionary<string, NameValuePair> queryStrings)
    {
        var tokenString = string.Empty;

        foreach (var queryString in queryStringTokens)
        {
            if (!queryStrings.TryGetValue(queryString, out NameValuePair query))
            {
                throw new Exception($"Uri was missing expected query string token '{queryString}'");
            }

            tokenString += query.Value;
        }

        return SecurityHelper.GenerateStaticHash(tokenString);
    }

    private string GenerateToken(IEnumerable<NameValuePair> nvp)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var value in nvp)
        {
            sb.Append(value.Value);
        }

        return SecurityHelper.GenerateStaticHash(sb.ToString());
    }

    public Uri GenerateValidatableUri(Uri uri, params string[] queryStringTokens)
    {
        var queryStrings = HttpHelper.GetNameValuePairs(uri.Query);

        var token = GenerateToken(queryStringTokens, queryStrings);

        return HttpHelper.SetQuerystringValue(uri, "Token", token);
    }

    public string GenerateValidatableUrl(string path, params NameValuePair[] nvp)
    {
        var token = GenerateToken(nvp);

        var portalUri = GetSiteUri(HttpHelper.BuildPathAndQuery(path, nvp));

        return HttpHelper.SetQuerystringValue(portalUri, "Token", token).AbsoluteUri;
    }
}
