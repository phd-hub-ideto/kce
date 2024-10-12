using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace KhumaloCraft.Helpers;

public static class HttpHelper
{
    public static UriBuilder CreateBaseUriBuilder(Uri url)
        => new UriBuilder(url.Scheme, url.Host, url.Port);

    public static Uri CreateBaseUri(Uri url)
        => CreateBaseUriBuilder(url).Uri;

    /*
    DO NOT make this a readonly field (or c# 6 autoproperty, which has the same effect).
    If done so and this class is being accessed from a context where Configuration.PortalBaseUri is NULL - e.g. event logging in the search engine -
    then the UriBuilder ctor will explode, causing this class's static ctor to explode, causing pretty much everything else to explode.
    */
    private static UriBuilder _defaultRequestBaseUriBuilder;

    private static UriBuilder DefaultRequestBaseUriBuilder
        => _defaultRequestBaseUriBuilder ??= new UriBuilder(Settings.Instance.PortalBaseUri)
        {
            Scheme = Uri.UriSchemeHttp,
            Port = -1,
        };

    public static UriBuilder CreateRequestBaseUriBuilder()
    {
        if (Dependencies.DependencyManager.HttpRequestProvider != null)
        {
            return CreateBaseUriBuilder(Dependencies.DependencyManager.HttpRequestProvider.RequestUri);
        }

        return DefaultRequestBaseUriBuilder;
    }

    public static void SetSessionValue(string key, object value)
    {
        Dependencies.DependencyManager.HttpContextProvider.SetSessionValue(key, value);
    }

    public static object GetSessionValue(string key)
    {
        return Dependencies.DependencyManager.HttpContextProvider.GetSessionValue(key);
    }

    public static void SplitPathAndQuery(string pathAndQuery, out string path, out string query)
    {
        if (pathAndQuery == null)
        {
            throw new ArgumentNullException(nameof(pathAndQuery));
        }

        var pos = pathAndQuery.IndexOf('?');

        if (pos == -1)
        {
            path = pathAndQuery;
            query = null;
        }
        else
        {
            path = pathAndQuery.Substring(0, pos);
            query = pathAndQuery.Substring(pos + 1);
        }
    }

    public static Uri MakeRequestBaseUriRelativeUri(string pathAndQuery)
    {
        if (Uri.TryCreate(pathAndQuery, UriKind.Absolute, out var absoluteUri))
            return absoluteUri;

        if (string.IsNullOrEmpty(pathAndQuery))
        {
            throw new ArgumentNullException(nameof(pathAndQuery));
        }

        pathAndQuery = pathAndQuery.TrimStart('/');
        var uriBuilder = CreateRequestBaseUriBuilder();
        SetPathAndQuery(uriBuilder, pathAndQuery);

        return uriBuilder.Uri;
    }

    public static void SetPathAndQuery(UriBuilder uriBuilder, string pathAndQuery)
    {
        SplitPathAndQuery(pathAndQuery, out var path, out var query);
        uriBuilder.Path = path;
        uriBuilder.Query = query;
    }

    private static Uri BuildUri(UriBuilder builder, params NameValuePair[] nameValuePairs)
    {
        builder.Query = BuildQuery(nameValuePairs);

        return builder.Uri;
    }

    public static Uri BuildUri(string uri, string relativePath, params NameValuePair[] nameValuePairs)
    {
        var uriBuilder = new UriBuilder(uri);

        return BuildUri(uriBuilder, relativePath, nameValuePairs);
    }

    public static Uri BuildUri(UriBuilder uriBuilder, string relativePath, params NameValuePair[] nameValuePairs)
    {
        SetPathAndQuery(uriBuilder, relativePath);

        return BuildUri(uriBuilder, nameValuePairs);
    }

    public static Uri SetQuerystringValue(Uri uri, string name1, string value1)
    {
        return SetQuerystringValues(uri, (string)null, new NameValuePair(name1, value1));
    }

    public static string SetQuerystringValueForPath(string path, string name, string value)
    {
        return BuildPathAndQuery(path, new NameValuePair(name, value));
    }

    public static string SetQuerystringValueForUrl(string url, string name, string value)
    {
        return SetQuerystringValue(MakeRequestBaseUriRelativeUri(url), name, value).ToString();
    }

    public static Uri SetQuerystringValues(Uri url, params NameValuePair[] nameValuePairs)
    {
        return SetQuerystringValues(url, null, nameValuePairs);
    }

    public static Uri SetQuerystringValues(Uri url, string newPath, params NameValuePair[] nameValuePairs)
    {
        var uriBuilder = new UriBuilder(url);
        if (newPath != null)
        {
            uriBuilder.Path = newPath;
        }

        var nameValuePairsDictionary = GetNameValuePairs(url.Query);

        foreach (var nameValuePair in nameValuePairs)
        {
            nameValuePairsDictionary[nameValuePair.Name] = nameValuePair;
        }

        return BuildUri(CreateBaseUriBuilder(uriBuilder.Uri), uriBuilder.Uri.AbsolutePath, nameValuePairsDictionary.Values.ToArray());
    }

    public static Dictionary<string, NameValuePair> GetNameValuePairs(string queryString, Encoding nullableUrlEncoding)
    {
        if (queryString != null)
        {
            queryString = queryString.TrimStart('?');
        }

        var result = new Dictionary<string, NameValuePair>();
        var num = queryString?.Length;

        for (var i = 0; i < num; i++)
        {
            var startIndex = i;
            var num4 = -1;

            while (i < num)
            {
                var ch = queryString[i];
                if (ch == '=')
                {
                    if (num4 < 0)
                    {
                        num4 = i;
                    }
                }
                else if (ch == '&')
                {
                    break;
                }

                i++;
            }

            var str = string.Empty;
            var str2 = string.Empty;

            if (num4 >= 0)
            {
                str = queryString.Substring(startIndex, num4 - startIndex);
                str2 = queryString.Substring(num4 + 1, (i - num4) - 1);
            }
            else
            {
                str2 = queryString.Substring(startIndex, i - startIndex);
            }

            NameValuePair nameValuePair;
            if (nullableUrlEncoding != null)
            {
                nameValuePair = new NameValuePair(HttpUtility.UrlDecode(str, nullableUrlEncoding), HttpUtility.UrlDecode(str2, nullableUrlEncoding));
                result[nameValuePair.Name] = nameValuePair;
            }
            else
            {
                nameValuePair = new NameValuePair(str, str2);
                result[nameValuePair.Name] = nameValuePair;
            }
        }

        return result;
    }

    public static Dictionary<string, NameValuePair> GetNameValuePairs(string queryString)
    {
        return GetNameValuePairs(queryString, Encoding.UTF8);
    }

    public static string BuildQuery(NameValueCollection nameValueCollection, bool urlEncode = true)
    {
        return BuildQuery(nameValueCollection.ToNameValuePairs(), urlEncode);
    }

    public static string BuildQuery(IEnumerable<NameValuePair> nameValuePairs, bool urlEncode = true)
    {
        Func<string, string> maybeUrlEncode = value => urlEncode ? HttpUtility.UrlEncode(value) : value;

        var sb = new StringBuilder();
        var first = true;

        static void AppendNameValuePair(StringBuilder sb, NameValuePair nameValuePair, Func<string, string> maybeUrlEncode, ref bool first)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append("&");
            }

            sb.Append(maybeUrlEncode(nameValuePair.Name));
            sb.Append("=");
            sb.Append(maybeUrlEncode(nameValuePair.Value));
        }

        foreach (var nameValuePair in nameValuePairs)
        {
            //Cater for name value pairs where the value contains multiple entries separated by commas.
            var splitValues = nameValuePair.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var splitValue in splitValues)
            {
                AppendNameValuePair(sb, new NameValuePair(nameValuePair.Name, splitValue), maybeUrlEncode, ref first);
            }
        }

        return sb.ToString();
    }

    public static string RemoveAllQueryStringArguments(string url)
    {
        var questionMarkPos = url.IndexOf('?');
        if (questionMarkPos > -1)
        {
            return url.Substring(0, questionMarkPos);
        }

        return url;
    }

    public static string BuildPathAndQuery(string path, params NameValuePair[] nameValuePairs)
    {
        if (nameValuePairs.IsNullOrEmpty())
        {
            return path;
        }

        SplitPathAndQuery(path, out var p, out var q);

        if (q != null)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(q);
            foreach (var nameValuePair in nameValuePairs)
            {
                nameValueCollection[nameValuePair.Name] = nameValuePair.Value;
            }

            return p + '?' + BuildQuery(nameValueCollection);
        }

        return p + '?' + BuildQuery(nameValuePairs);
    }

    public static bool AllowCookiesInResponse
    {
        get
        {
            return Dependencies.DependencyManager.HttpContextProvider.AllowCookiesInResponse;
        }
        set
        {
            Dependencies.DependencyManager.HttpContextProvider.AllowCookiesInResponse = value;
        }
    }

    // http://stackoverflow.com/a/576209
    // To get the current cookie, we need to check both response and request cookie collections
    // Also you need to be careful how you check, or you will *create* a cookie if it doesn't exist http://stackoverflow.com/a/30223103
    public static string GetCurrentCookie(string name)
    {
        return Dependencies.DependencyManager.HttpContextProvider.GetCurrentCookie(name);
    }
}

[System.Diagnostics.DebuggerDisplay(nameof(NameValuePair) + ": "
    + nameof(Name) + " = {" + nameof(Name) + ",nq}" + ", "
    + nameof(Value) + " = {" + nameof(Value) + ",nq}")]
public class NameValuePair
{
    public string Name { get; }
    public string Value { get; }

    public NameValuePair(string name, string value)
    {
        Name = name;
        Value = value;
    }
}

public static class NameValueCollectionExtensions
{
    public static void AddRange(this NameValueCollection @this, IEnumerable<NameValuePair> pairs)
    {
        foreach (var pair in pairs)
        {
            @this.Add(pair.Name, pair.Value);
        }
    }

    public static IEnumerable<NameValuePair> ToNameValuePairs(this NameValueCollection @this)
    {
        foreach (string key in @this)
        {
            yield return new NameValuePair(key, @this[key]);
        }
    }
}
