namespace KhumaloCraft.Application.Routing.Routes;

public interface IKCRouteBuilder
{
    string BuildUrl(string routeName, object routeValues = null);
    string BuildPackedUrl(string routeName, object routeValues = null);
}