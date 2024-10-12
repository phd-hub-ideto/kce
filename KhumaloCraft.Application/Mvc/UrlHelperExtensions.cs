using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace KhumaloCraft.Application.Mvc;

public static class UrlHelperExtensions
{
    public static IDictionary<string, object> GetRouteValues(this IUrlHelper urlHelper) => urlHelper.ActionContext.RouteData.Values;

    public static Uri GetRequestUri(this IUrlHelper urlHelper) => new Uri(urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl());

    public static IUrlHelper CreateFilteredUrlHelper(this IUrlHelper urlHelper, HashSet<string> excludeRouteValues)
    {
        if ((excludeRouteValues == null) || (excludeRouteValues.Count == 0))
        {
            return urlHelper;
        }
        else
        {
            return new FilteredUrlHelper(urlHelper.ActionContext, excludeRouteValues);
        }
    }

    private class FilteredUrlHelper : UrlHelper
    {
        private HashSet<string> _excludeRouteValues;

        public FilteredUrlHelper(ActionContext actionContext, HashSet<string> excludeRouteValues)
            : base(actionContext)
        {
            _excludeRouteValues = excludeRouteValues;
        }

        private ActionContext _actionContext;
        public new ActionContext ActionContext
        {
            get
            {
                if (_actionContext is null)
                {
                    var routeData = new RouteData(base.ActionContext.RouteData);
                    routeData.Values.Clear();
                    routeData.DataTokens.Clear();
                    foreach (var value in base.ActionContext.RouteData.Values)
                    {
                        if (_excludeRouteValues.Contains(value.Key))
                        {
                            continue;
                        }

                        routeData.Values.Add(value.Key, value.Value);
                    }

                    foreach (var dataToken in base.ActionContext.RouteData.DataTokens)
                    {
                        if (_excludeRouteValues.Contains(dataToken.Key))
                        {
                            continue;
                        }
                        routeData.Values.Add(dataToken.Key, dataToken.Value);
                    }

                    _actionContext = new ActionContext(
                        base.ActionContext.HttpContext,
                        routeData,
                        base.ActionContext.ActionDescriptor,
                        base.ActionContext.ModelState);
                }
                return _actionContext;
            }
        }
    }
}
