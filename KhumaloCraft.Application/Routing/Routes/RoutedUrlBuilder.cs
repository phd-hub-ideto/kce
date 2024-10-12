using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace KhumaloCraft.Application.Routing.Routes;

public class RoutedUrlBuilder : IRoutedUrlBuilder
{
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoutedUrlBuilder(IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAccessor)
    {
        _urlHelperFactory = urlHelperFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public string BuildUrl(string routeName, RouteValueDictionary routeValues)
    {
        var uriHelper = _urlHelperFactory.GetUrlHelper(new ActionContext()
        {
            RouteData = new RouteData(),
            HttpContext = _httpContextAccessor.HttpContext
        });

        var virtualPath = uriHelper.RouteUrl(routeName, routeValues);

        if (virtualPath == null)
        {
            throw new InvalidOperationException($"Could not get virtual path for route \"{routeName}\" using values: {JsonConvert.SerializeObject(routeValues)}");
        }

        return virtualPath;
    }
}