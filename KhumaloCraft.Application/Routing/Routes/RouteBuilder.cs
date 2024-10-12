using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Routing;

namespace KhumaloCraft.Application.Routing.Routes;

public class RouteBuilder(IRoutedUrlBuilder routedUrlBuilder) : IKCRouteBuilder
{
    private static RouteValueDictionary ToRouteValues(object obj)
    {
        if (obj?.GetType().IsAnonymous() == true)
        {
            return new RouteValueDictionary(obj);
        }

        return new CustomRouteValueDictionary(obj, excludeDefaultValues: true);
    }

    private static RouteValueDictionary Pack(RouteValueDictionary routeValues, bool stripNulls = false)
    {
        var result = new RouteValueDictionary();

        foreach (var kvp in routeValues)
        {
            if (kvp.Value is string stringValue)
            {
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    result.Add(kvp.Key, FormattingHelper.PackUrlPartOrDefault(stringValue));
                }
            }
            else if (kvp.Value == null && !stripNulls)
            {
                result.Add(kvp.Key, FormattingHelper.PackUrlPartShortDash.ToString()); // ToString call is VITAL here, since char doesn't match string route constraints!
            }
            else
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }

        return result;
    }

    private string BuildUrl(string routeName, RouteValueDictionary routeValues, bool pack)
    {
        if (pack && routeValues != null)
        {
            routeValues = Pack(routeValues);
        }

        return routedUrlBuilder.BuildUrl(routeName, routeValues);
    }

    public string BuildUrl(string routeName, object routeValues = null) => BuildUrl(routeName, ToRouteValues(routeValues), false);

    public string BuildPackedUrl(string routeName, object routeValues = null) => BuildUrl(routeName, ToRouteValues(routeValues), true);
}