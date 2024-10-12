using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Http;

public class TemplateNameProvider : ITemplateNameProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TemplateNameProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool TryGetTemplateName(out string templateName)
    {
        if (_httpContextAccessor.HttpContext is not null)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData().Values;
            var controller = routeData["controller"] as string;
            var action = routeData["action"] as string;

            if (!string.IsNullOrEmpty(controller) && !string.IsNullOrWhiteSpace(action))
            {
                templateName = controller + "\\" + action;
                return true;
            }
        }

        templateName = string.Empty;

        return false;
    }
}