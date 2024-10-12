using KhumaloCraft.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace KhumaloCraft.Application.Portal.Routing.Helper;

public class HttpReferrerHelper
{
    private readonly IHttpRequestProvider _httpRequestProvider;

    private readonly EndpointDataSource _endpointDataSource;
    private readonly LinkParser _linkParser;

    public HttpReferrerHelper(
        IHttpRequestProvider httpRequestProvider,
        EndpointDataSource endpointDataSource,
        LinkParser linkParser)
    {
        _httpRequestProvider = httpRequestProvider;
        _endpointDataSource = endpointDataSource;
        _linkParser = linkParser;
    }

    public bool FromControllerAndAction<T>(string actionName) where T : Controller
    {
        if (!TryGetReferrerUri(out var referrerUri))
        {
            return false;
        }

        foreach (var endpoint in _endpointDataSource.Endpoints)
        {
            var name = endpoint.Metadata.OfType<EndpointNameMetadata>().FirstOrDefault()?.EndpointName;
            if (name is null) continue;

            var controllerActionDescriptor = endpoint.Metadata.OfType<ControllerActionDescriptor>().FirstOrDefault();

            if (controllerActionDescriptor != null && controllerActionDescriptor.ActionName == actionName
                && controllerActionDescriptor.ControllerTypeInfo.Name == typeof(T).Name)
            {
                return _linkParser.ParsePathByEndpointName(name, referrerUri.AbsolutePath) is not null;
            }
        }

        return false;
    }

    public bool TryGetReferrerUri(out Uri uri)
    {
        var referrerUri = _httpRequestProvider.UrlReferrer;

        if (referrerUri != null && referrerUri.IsAbsoluteUri)
        {
            uri = referrerUri;

            return true;
        }

        uri = default;

        return false;
    }
}