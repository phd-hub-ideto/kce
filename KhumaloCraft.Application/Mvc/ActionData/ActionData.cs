using Microsoft.AspNetCore.Routing;

namespace KhumaloCraft.Application.Mvc;

public abstract class ActionData
{
    protected ActionData()
    {
    }

    protected ActionData(string routeName, string @namespace, string area, string action, string controller, object routeValues, params string[] ignoreRouteValues)
    {
        RouteName = routeName;
        Namespace = @namespace;
        Area = area;
        Action = action;
        Controller = controller;
        RouteValues = new CustomRouteValueDictionary(routeValues, true);

        if (!ignoreRouteValues.IsNullOrEmpty())
        {
            IgnoreRouteValues = new HashSet<string>(ignoreRouteValues);
        }
    }

    public string Namespace { get; set; }

    public string Area { get; set; }

    public string RouteName { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public CustomRouteValueDictionary RouteValues { get; set; }

    public HashSet<string> IgnoreRouteValues { get; set; }

    public bool HasRouteName => !string.IsNullOrEmpty(RouteName);

    public bool HasRouteValues => !RouteValues.IsNullOrEmpty();

    public override int GetHashCode()
    {
        var result = 0;

        if (Namespace != null)
        {
            result ^= Namespace.GetHashCode();
        }
        if (Area != null)
        {
            result ^= Area.GetHashCode();
        }
        if (RouteName != null)
        {
            result ^= RouteName.GetHashCode();
        }
        if (Controller != null)
        {
            result ^= Controller.GetHashCode();
        }
        if (Action != null)
        {
            result ^= Action.GetHashCode();
        }
        if (RouteValues != null)
        {
            result ^= RouteValues.GetHashCode();
        }
        if (IgnoreRouteValues != null)
        {
            result ^= IgnoreRouteValues.Count;
        }

        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(obj, this))
        {
            return true;
        }

        if (!(obj is ActionData actionData))
        {
            return false;
        }

        if (Namespace != actionData.Namespace)
        {
            return false;
        }
        if (Area != actionData.Area)
        {
            return false;
        }
        if (RouteName != actionData.RouteName)
        {
            return false;
        }
        if (Action != actionData.Action)
        {
            return false;
        }
        if (Controller != actionData.Controller)
        {
            return false;
        }

        if (RouteValues == null)
        {
            if (actionData.RouteValues != null)
            {
                return false;
            }
        }
        else
        {
            if (!RouteValues.Equals(actionData.RouteValues))
            {
                return false;
            }
        }

        if (IgnoreRouteValues == null)
        {
            if (actionData.IgnoreRouteValues != null)
            {
                return false;
            }
        }
        else
        {
            if (actionData.IgnoreRouteValues == null)
            {
                return false;
            }

            if (IgnoreRouteValues.Count != actionData.IgnoreRouteValues.Count)
            {
                return false;
            }

            foreach (var item in IgnoreRouteValues)
            {
                if (actionData.IgnoreRouteValues.Contains(item))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public CustomRouteValueDictionary ToRouteValueDictionary(RouteValueDictionary routeValueDictionary)
    {
        return ToRouteValueDictionary(new[] { routeValueDictionary });
    }

    public CustomRouteValueDictionary ToRouteValueDictionary(params object[] mergeModels)
    {
        var result = new CustomRouteValueDictionary(RouteValues);

        //merge in the models
        if (!mergeModels.IsNullOrEmpty())
        {
            foreach (var model in mergeModels.Where(m => m != null))
            {
                result.Merge(model);
            }
        }

        //remove the ignore values
        if (!IgnoreRouteValues.IsNullOrEmpty())
        {
            result.RemoveAll(r => IgnoreRouteValues.Contains(r.Key));
        }

        //lastly do the important ones
        if (Namespace != null)
        {
            result[MvcRouteDataTokenKeys.Namespaces] = new[] { Namespace, };
        }
        if (Area != null)
        {
            result[MvcRouteDataTokenKeys.Area] = Area;
        }
        if (Controller != null)
        {
            result[MvcRouteDataTokenKeys.Controller] = Controller;
        }
        if (Action != null)
        {
            result[MvcRouteDataTokenKeys.Action] = Action;
        }
        return result;
    }

    public abstract ActionData Clone();

    public ActionDefinition ToActionDefinition()
    {
        return new ActionDefinition(Namespace, Area, Controller, Action, RouteName);
    }

    private static class MvcRouteDataTokenKeys
    {
        public static string Namespaces => "namespaces";
        public static string Area => "area";
        public static string Controller => "controller";
        public static string Action => "action";
    }
}

public class ActionData<T> : ActionData
{
    public ActionData()
    {
    }

    private ActionData(string routeName, string @namespace, string area, string action, string controller, object routeValues, params string[] ignoreRouteValues)
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

        return new ActionData<T>(RouteName, Namespace, Area, Action, Controller, RouteValues, ignoreRouteValues);
    }
}