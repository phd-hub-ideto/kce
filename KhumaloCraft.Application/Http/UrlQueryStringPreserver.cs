using KhumaloCraft.Helpers;
using System.Web;

namespace KhumaloCraft.Application.Http;

public abstract class UrlQueryStringPreserver
{
    public string Preserve(Uri requestUri, string url)
    {
        return TryGetValuesFromRequestUrl(requestUri, out var queryStringPairs)
           ? AppendParameters(requestUri, url, queryStringPairs)
           : new Uri(requestUri, url).AbsoluteUri;
    }

    protected abstract bool TryGetValuesFromRequestUrl(Uri requestUri, out IEnumerable<NameValuePair> pairs);

    public string AppendParameters(Uri requestUri, string destinationUrl, IEnumerable<NameValuePair> queryStringPairs)
    {
        var absoluteDestinationUri = new Uri(requestUri, destinationUrl);
        var destinationUriBuilder = new UriBuilder(absoluteDestinationUri);
        var destinationUrlQuery = HttpUtility.ParseQueryString(destinationUriBuilder.Query);

        destinationUrlQuery.AddRange(queryStringPairs);

        destinationUriBuilder.Query = HttpHelper.BuildQuery(destinationUrlQuery);

        return destinationUriBuilder.Uri.AbsoluteUri;
    }
}