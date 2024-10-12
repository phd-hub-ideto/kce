using KhumaloCraft.Helpers;
using System.Web;

namespace KhumaloCraft.Application.Http;

public class UrlMarketingQueryStringPreserver : UrlQueryStringPreserver
{
    private const string UtmKeyPrefix = "utm_";
    private const string GoogleAdsClickIdentifier = "gclid";
    private const string FacebookAdsClickIdentifier = "fbclid";

    private static readonly string[] _keys = [
        Constants.UrchinTrackingModuleKey.Campaign,
        Constants.UrchinTrackingModuleKey.Content,
        Constants.UrchinTrackingModuleKey.Medium,
        Constants.UrchinTrackingModuleKey.Source,
        Constants.UrchinTrackingModuleKey.Term,
        GoogleAdsClickIdentifier,
        FacebookAdsClickIdentifier
    ];

    protected override bool TryGetValuesFromRequestUrl(Uri requestUri, out IEnumerable<NameValuePair> pairs)
    {
        var requestUrlQuery = Uri.UnescapeDataString(requestUri.Query);

        if (requestUrlQuery.ContainsAny(UtmKeyPrefix, GoogleAdsClickIdentifier, FacebookAdsClickIdentifier))
        {
            var queryPairs = HttpUtility.ParseQueryString(requestUrlQuery).ToNameValuePairs();

            pairs = queryPairs.Where(k => k.Name.In(_keys, StringComparer.InvariantCultureIgnoreCase));
            return true;
        }

        pairs = default;

        return false;
    }
}