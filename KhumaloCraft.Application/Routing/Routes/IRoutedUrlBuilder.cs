using Microsoft.AspNetCore.Routing;

namespace KhumaloCraft.Application.Routing.Routes;

public interface IRoutedUrlBuilder
{
    string BuildUrl(string routeName, RouteValueDictionary routeValues);
}