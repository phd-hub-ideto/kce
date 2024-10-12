namespace KhumaloCraft.Application.Portal.Routing.Helper;

public class CustomRouteData
{
    private const string MSDirectRouteMatches = "MS_DirectRouteMatches";
    private const string ControllerString = "Controller";
    private const string ControllerLoweredString = "controller";
    private const string ActionLoweredString = "action";

    public Uri Url { get; set; }
    private string ControllerName { get; }
    private string ActionName { get; }

    public CustomRouteData(Uri uri, RouteData routeData)
    {
        Url = uri;
        ControllerName = GetControllerName(routeData);
        ActionName = GetActionName(routeData);
    }

    private string GetControllerName(RouteData routeData)
    {
        return $"{routeData?.Values[ControllerLoweredString]}{ControllerString}";
    }

    private string GetActionName(RouteData routeData)
    {
        string actionName = routeData?.Values[ActionLoweredString]?.ToString();
        if (!string.IsNullOrWhiteSpace(actionName))
        {
            return actionName;
        }

        if (routeData.Values.ContainsKey(MSDirectRouteMatches))
        {
            var routeDataAsListFromMsDirectRouteMatches = (List<RouteData>)routeData.Values[MSDirectRouteMatches];
            var routeValueDictionaryFromMsDirectRouteMatches = routeDataAsListFromMsDirectRouteMatches.FirstOrDefault();
            return routeValueDictionaryFromMsDirectRouteMatches.Values[ActionLoweredString].ToString();
        }

        return string.Empty;
    }

    public bool IsController(string controllerName)
    {
        return string.Equals(ControllerName, controllerName, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool IsAction(string actionName)
    {
        return string.Equals(ActionName, actionName, StringComparison.InvariantCultureIgnoreCase);
    }
}