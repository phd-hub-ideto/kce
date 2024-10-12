namespace KhumaloCraft.Application.ImageServer;

public class ImageServerTemplateNameProvider(IHttpContextAccessor httpContextAccessor) : ITemplateNameProvider
{
    public bool TryGetTemplateName(out string templateName)
    {
        var routeData = httpContextAccessor.HttpContext.GetRouteData().Values;

        var sizeOption = routeData["sizeOption"] as string;
        if (!string.IsNullOrEmpty(sizeOption))
        {
            templateName = sizeOption.ToLower();
            return true;
        }

        var imageId = routeData["imageId"] as string;

        if (!string.IsNullOrEmpty(imageId))
        {
            templateName = "original";
            return true;
        }

        var controller = routeData["controller"] as string;
        var action = routeData["action"] as string;

        if (!string.IsNullOrEmpty(controller) && !string.IsNullOrWhiteSpace(action))
        {
            templateName = controller + "\\" + action;
            return true;
        }

        templateName = string.Empty;
        return false;
    }
}