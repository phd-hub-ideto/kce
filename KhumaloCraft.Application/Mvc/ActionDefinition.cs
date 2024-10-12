namespace KhumaloCraft.Application.Mvc
{
    public class ActionDefinition
    {
        public static readonly string DefaultAreaName = string.Empty;

        public ActionDefinition(string @namespace, string controller, string action)
            : this(@namespace, DefaultAreaName, controller, action, null)
        {
        }

        public ActionDefinition(string @namespace, string area, string controller, string action, string routeName)
        {
            Namespace = @namespace;
            Area = area;
            Controller = controller;
            Action = action;
            RouteName = routeName;
        }

        public string RouteName { get; }

        public string Namespace { get; }

        public string Area { get; }

        public string Controller { get; }

        public string Action { get; }

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
            if (Controller != null)
            {
                result ^= Controller.GetHashCode();
            }
            if (Action != null)
            {
                result ^= Action.GetHashCode();
            }
            if (RouteName != null)
            {
                result ^= RouteName.GetHashCode();
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

            if (!(obj is ActionDefinition actionDefinition))
            {
                return false;
            }

            if (Namespace != actionDefinition.Namespace)
            {
                return false;
            }
            if (Area != actionDefinition.Area)
            {
                return false;
            }
            if (Action != actionDefinition.Action)
            {
                return false;
            }
            if (Controller != actionDefinition.Controller)
            {
                return false;
            }
            if (RouteName != actionDefinition.RouteName)
            {
                return false;
            }

            return true;
        }

        public ActionData ToActionData(object routeValues = null)
        {
            var result = new DefinedActionData(Namespace, Area, Controller, Action, RouteName);
            if (routeValues != null)
            {
                if (result.RouteValues == null)
                {
                    result.RouteValues = new CustomRouteValueDictionary(routeValues);
                }
                else
                {
                    result.RouteValues.Merge(routeValues);
                }
            }

            return result;
        }
    }
}