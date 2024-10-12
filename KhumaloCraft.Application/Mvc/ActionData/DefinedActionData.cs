namespace KhumaloCraft.Application.Mvc;

public class DefinedActionData : ActionData
{
    public DefinedActionData(string @namespace, string area, string controller, string action, string routeName = null)
    {
        RouteName = routeName;
        Namespace = @namespace;
        Area = area;
        Controller = controller;
        Action = action;
    }

    internal DefinedActionData(string routeName, string @namespace, string area, string action, string controller,
        object routeValues, params string[] ignoreRouteValues)
        : base(routeName, @namespace, area, action, controller, routeValues, ignoreRouteValues)
    {
    }

    public override ActionData Clone()
    {
        string[] ignoreRouteValues = null;
        if (IgnoreRouteValues != null)
        {
            ignoreRouteValues = IgnoreRouteValues.ToArray();
        }

        return new DefinedActionData(RouteName, Namespace, Area, Action, Controller, RouteValues, ignoreRouteValues);
    }
}