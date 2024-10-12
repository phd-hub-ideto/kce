using KhumaloCraft.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Linq.Expressions;
using System.Reflection;

namespace KhumaloCraft.Application.Mvc;

public static class ActionHelper
{
    public static IHtmlContent ActionLink<TController>(this IHtmlHelper @this, string linkText, Expression<Func<TController, ActionResult>> action,
        object routeValues = null, object htmlAttributes = null)
        where TController : Controller
    {
        var actionData = GetActionData(action, routeValues);

        return @this.ActionLink(linkText, actionData.Action, actionData.Controller, null, null, null, actionData.RouteValues, htmlAttributes);
    }

    public static IHtmlContent ActionLinkAsync<TController>(this IHtmlHelper @this, string linkText, Expression<Func<TController, Task<ActionResult>>> action, object htmlAttributes = null)
        where TController : Controller
    {
        var actionData = GetAsyncActionData(action);

        return @this.ActionLink(linkText, actionData.Action, actionData.Controller, null, null, null, actionData.RouteValues, htmlAttributes);
    }

    public static IHtmlContent ActionRaw<TController, TActionResult>(this IUrlHelper helper, Expression<Func<TController, TActionResult>> expression, object defaultValues = null)
            where TController : Controller
            where TActionResult : ActionResult
    {
        return new HtmlString(Action(helper, expression, defaultValues));
    }

    public static IHtmlContent ActionRaw<TController>(this IUrlHelper helper, Expression<Func<TController, ActionResult>> expression, object defaultValues = null)
            where TController : Controller
    {
        return new HtmlString(Action(helper, expression, defaultValues));
    }

    public static string Action<TController, TActionResult>(this IUrlHelper urlHelper, Expression<Func<TController, TActionResult>> expression, object defaultValues = null)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return Action(urlHelper, GetActionData(urlHelper, expression), defaultValues);
    }

    public static string AsyncAction<TController, TActionResult>(this IUrlHelper urlHelper, Expression<Func<TController, Task<TActionResult>>> expression, object defaultValues = null)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return Action(urlHelper, GetAsyncActionData(urlHelper, expression), defaultValues);
    }

    public static string Action<TController>(this IUrlHelper urlHelper, Expression<Func<TController, ActionResult>> expression, object defaultValues = null)
        where TController : Controller
    {
        return Action<TController, ActionResult>(urlHelper, expression, defaultValues);
    }

    public static string AsyncAction<TController>(this IUrlHelper urlHelper, Expression<Func<TController, Task<ActionResult>>> expression, object defaultValues = null)
        where TController : Controller
    {
        return AsyncAction<TController, ActionResult>(urlHelper, expression, defaultValues);
    }

    public static string ActionNoDefaults<TController>(this IUrlHelper urlHelper, Expression<Func<TController, ActionResult>> expression, object defaultValues = null)
        where TController : Controller
    {
        return Action(urlHelper, GetActionData(null, expression), defaultValues);
    }

    public static string AsyncActionNoDefaults<TController>(this IUrlHelper urlHelper, Expression<Func<TController, Task<ActionResult>>> expression, object defaultValues = null)
        where TController : Controller
    {
        return Action(urlHelper, GetAsyncActionData(null, expression), defaultValues);
    }

    public static ActionInfo ActionInfo<TController>(this IUrlHelper urlHelper, Expression<Func<TController, ActionResult>> expression, object defaultValues = null)
        where TController : Controller
    {
        var actionDataDescriptor = GetActionDataDescriptor(null, expression, true);
        var actionData = actionDataDescriptor.Data.Clone();

        return new ActionInfo(actionData, actionDataDescriptor.Descriptor, Action(urlHelper, actionData, defaultValues));
    }

    public static ActionInfo AsyncActionInfo<TController>(this IUrlHelper urlHelper, Expression<Func<TController, Task<ActionResult>>> expression, object defaultValues = null)
        where TController : Controller
    {
        var actionDataDescriptor = GetAsyncActionDataDescriptor(null, expression, true);
        var actionData = actionDataDescriptor.Data.Clone();

        return new ActionInfo(actionData, actionDataDescriptor.Descriptor, Action(urlHelper, actionData, defaultValues));
    }

    public static string Action(this IUrlHelper urlHelper, ActionData actionData, object defaultValues = null)
    {
        if (actionData is LiteralActionData literalActionData)
        {
            return literalActionData.Value;
        }

        var result = new CustomRouteValueDictionary();

        result.Merge(defaultValues);

        foreach (var item in actionData.ToRouteValueDictionary())
        {
            //the same key (probably with different values) is present in both actionData.ToRouteValueDictionary (from actionData) and result (from defaultValues)
            //if the value for the key inside actionData is null we assume that the key was specified as part of actionData for url generation,
            //in that case the value for the key specified in result (from defaultValues), not the one in actionData (from actionData) is considered 'more correct' since it's explicitly provided

            if (result.ContainsKey(item.Key) && item.Value == null)
            {
                continue;
            }

            result[item.Key] = item.Value;
        }

        var filteredUrlHelper = urlHelper.CreateFilteredUrlHelper(actionData.IgnoreRouteValues);
        if (actionData.RouteName == null)
        {
            return filteredUrlHelper.Action(null, null, result);
        }

        var routeUrl = filteredUrlHelper.RouteUrl(actionData.RouteName, result);
        return routeUrl;
    }

    public static RouteValueDictionary GetActionValues<TController>(Expression<Func<TController, ActionResult>> expression, object defaultValues = null)
        where TController : Controller
    {
        var result = new CustomRouteValueDictionary(defaultValues);
        foreach (var item in GetActionData(expression).ToRouteValueDictionary())
        {
            result[item.Key] = item.Value;
        }

        return result;
    }

    private static (ActionData<TActionResult> Data, ActionDescriptor Descriptor) GetActionDataDescriptor<TController, TActionResult>(IUrlHelper urlHelper,
        Expression<Func<TController, TActionResult>> expression, ActionData actionData = null, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        var expressionRouteInfo = GetRouteInfoFromExpression(expression, excludeDefaultValues);

        if (urlHelper != null)
        {
            var newRouteValues = new CustomRouteValueDictionary(urlHelper.GetRouteValues());

            foreach (var routeValue in expressionRouteInfo.RouteValues)
            {
                newRouteValues[routeValue.Key] = routeValue.Value;
            }

            expressionRouteInfo.RouteValues = newRouteValues;
        }

        var actionDataResult = new ActionData<TActionResult>
        {
            Namespace = (string)expressionRouteInfo.RouteValues["namespace"],
            Controller = (string)expressionRouteInfo.RouteValues["controller"],
            Action = (string)expressionRouteInfo.RouteValues["action"],
            RouteValues = expressionRouteInfo.RouteValues,
            IgnoreRouteValues = expressionRouteInfo.ExcludeRouteValues,
        };

        if (actionData != null)
        {
            actionDataResult.RouteName = actionData.RouteName;
            actionDataResult.Namespace = actionData.Namespace;
            actionDataResult.Controller = actionData.Controller;
            actionDataResult.Action = actionData.Action;
        }

        ProcessInferredActionData?.Invoke(expressionRouteInfo, actionDataResult);

        return (actionDataResult, expressionRouteInfo.Action);
    }

    private static (ActionData<Task<TActionResult>> Data, ActionDescriptor Descriptor) GetAsyncActionDataDescriptor<TController, TActionResult>(IUrlHelper urlHelper,
        Expression<Func<TController, Task<TActionResult>>> expression, ActionData actionData = null, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        var expressionRouteInfo = GetAsyncRouteInfoFromExpression(expression, excludeDefaultValues);

        if (urlHelper != null)
        {
            var newRouteValues = new CustomRouteValueDictionary(urlHelper.GetRouteValues());

            foreach (var routeValue in expressionRouteInfo.RouteValues)
            {
                newRouteValues[routeValue.Key] = routeValue.Value;
            }

            expressionRouteInfo.RouteValues = newRouteValues;
        }

        var actionDataResult = new ActionData<Task<TActionResult>>
        {
            Namespace = (string)expressionRouteInfo.RouteValues["namespace"],
            Controller = (string)expressionRouteInfo.RouteValues["controller"],
            Action = (string)expressionRouteInfo.RouteValues["action"],
            RouteValues = expressionRouteInfo.RouteValues,
            IgnoreRouteValues = expressionRouteInfo.ExcludeRouteValues,
        };

        if (actionData != null)
        {
            actionDataResult.RouteName = actionData.RouteName;
            actionDataResult.Namespace = actionData.Namespace;
            actionDataResult.Controller = actionData.Controller;
            actionDataResult.Action = actionData.Action;
        }

        ProcessInferredActionData?.Invoke(expressionRouteInfo, actionDataResult);

        return (actionDataResult, expressionRouteInfo.Action);
    }

    private static (ActionData<ActionResult> Data, ActionDescriptor Descriptor) GetActionDataDescriptor<TController>(IUrlHelper urlHelper,
        Expression<Func<TController, ActionResult>> expression, bool excludeDefaultValues = true)
        where TController : Controller
    {
        return GetActionDataDescriptor(urlHelper, expression, null, excludeDefaultValues);
    }

    private static (ActionData<Task<ActionResult>> Data, ActionDescriptor Descriptor) GetAsyncActionDataDescriptor<TController>(IUrlHelper urlHelper,
        Expression<Func<TController, Task<ActionResult>>> expression, bool excludeDefaultValues = true)
        where TController : Controller
    {
        return GetAsyncActionDataDescriptor(urlHelper, expression, null, excludeDefaultValues);
    }

    public static ActionData<TActionResult> GetActionData<TController, TActionResult>(IUrlHelper urlHelper, Expression<Func<TController, TActionResult>> expression,
        ActionData actionData = null, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return GetActionDataDescriptor(urlHelper, expression, actionData, excludeDefaultValues).Data;
    }

    public static ActionData<Task<TActionResult>> GetAsyncActionData<TController, TActionResult>(IUrlHelper urlHelper, Expression<Func<TController, Task<TActionResult>>> expression,
        ActionData actionData = null, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return GetAsyncActionDataDescriptor(urlHelper, expression, actionData, excludeDefaultValues).Data;
    }

    public static ActionData<TActionResult> GetActionData<TController, TActionResult>(Expression<Func<TController, TActionResult>> expression, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return GetActionData(null, expression, null, excludeDefaultValues);
    }

    public static ActionData<Task<TActionResult>> GetAsyncActionData<TController, TActionResult>(Expression<Func<TController, Task<TActionResult>>> expression, bool excludeDefaultValues = true)
        where TController : Controller
        where TActionResult : ActionResult
    {
        return GetAsyncActionData(null, expression, null, excludeDefaultValues);
    }

    public static ActionData<TActionResult> GetActionData<TController, TActionResult>(Expression<Func<TController, TActionResult>> expression, object addRouteValues)
        where TController : Controller
        where TActionResult : ActionResult
    {
        var rv = new CustomRouteValueDictionary(addRouteValues);
        var result = GetActionData(expression);
        foreach (var item in rv)
        {
            result.RouteValues[item.Key] = item.Value;
        }

        return result;
    }

    public static ActionData<ActionResult> GetActionData<TController>(IUrlHelper urlHelper, Expression<Func<TController, ActionResult>> expression, ActionData actionData = null,
        bool excludeDefaultValues = true)
        where TController : Controller
    {
        return GetActionData<TController, ActionResult>(urlHelper, expression, actionData, excludeDefaultValues);
    }

    public static ActionData<ActionResult> GetActionData<TController>(Expression<Func<TController, ActionResult>> expression, bool excludeDefaultValues = true)
        where TController : Controller
    {
        return GetActionData<TController, ActionResult>(expression, excludeDefaultValues);
    }

    public static ActionData<Task<ActionResult>> GetAsyncActionData<TController>(Expression<Func<TController, Task<ActionResult>>> expression, bool excludeDefaultValues = true)
        where TController : Controller
    {
        return GetAsyncActionData<TController, ActionResult>(expression, excludeDefaultValues);
    }

    public static ActionData<ActionResult> GetActionData<TController>(Expression<Func<TController, ActionResult>> expression, object addRouteValues)
         where TController : Controller
    {
        return GetActionData<TController, ActionResult>(expression, addRouteValues);
    }

    public static ExpressionRouteInfo GetRouteInfoFromExpression<TController, TActionResult>(Expression<Func<TController, TActionResult>> action,
        bool excludeDefaultValues = false, bool preserveTypes = false)
        where TController : Controller
        where TActionResult : ActionResult
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var call = action.Body as MethodCallExpression;
        if (call == null)
        {
            throw new ArgumentException("Expression must represent a method call.", nameof(action));
        }

        var controllerName = typeof(TController).Name;
        if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Expression target must end in a controller.", nameof(action));
        }

        controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
        if (controllerName.Length == 0)
        {
            throw new ArgumentException("Expression target must end in a controller name suffixed with \"Controller\".", nameof(action));
        }

        var rvd = new CustomRouteValueDictionary
        {
            { "Controller", controllerName },
            { "Action", call.Method.Name }
        };

        AddParameterValuesFromExpressionToDictionary(rvd, call, excludeDefaultValues, preserveTypes);

        var ignoreRouteValues = new HashSet<string>();
        foreach (var ignoreRouteValueFilterAttribute in AttributeCache.GetCustomAttributes<IgnoreRouteValueFilterAttribute>(call.Method, true).Concat(
                                                            AttributeCache.GetCustomAttributes<IgnoreRouteValueFilterAttribute>(call.Method.DeclaringType, true)))
        {
            var keys = ignoreRouteValueFilterAttribute.Keys;
            if (keys != null)
            {
                ignoreRouteValues.AddRange(keys);
            }
        }

        var allowAnonymousIgnoreRouteValues = AllowAnonymousIgnoreRouteValues;
        if (allowAnonymousIgnoreRouteValues != null && AttributeCache.GetCustomAttributes<AllowAnonymousAttribute>(call.Method, true).Concat(
                AttributeCache.GetCustomAttributes<AllowAnonymousAttribute>(call.Method.DeclaringType, true)).Any())
        {
            foreach (var allowAnonymousIgnoreRouteValue in allowAnonymousIgnoreRouteValues)
            {
                ignoreRouteValues.Add(allowAnonymousIgnoreRouteValue);
            }
        }

        var result = new ExpressionRouteInfo
        {
            Action = new ControllerActionDescriptor()
            {
                MethodInfo = call.Method,
                ActionName = call.Method.Name,
                ControllerTypeInfo = typeof(TController).GetTypeInfo()
            },
            ExcludeRouteValues = ignoreRouteValues,
            RouteValues = rvd,
        };

        ExpressionRouteInfoProxy?.Invoke(result);

        return result;
    }

    public static ExpressionRouteInfo GetAsyncRouteInfoFromExpression<TController, TActionResult>(Expression<Func<TController, Task<TActionResult>>> action,
        bool excludeDefaultValues = false, bool preserveTypes = false)
        where TController : Controller
        where TActionResult : ActionResult
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var call = action.Body as MethodCallExpression;
        if (call == null)
        {
            throw new ArgumentException("Expression must represent a method call.", nameof(action));
        }

        var controllerName = typeof(TController).Name;
        if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Expression target must end in a controller.", nameof(action));
        }

        controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
        if (controllerName.Length == 0)
        {
            throw new ArgumentException("Expression target must end in a controller name suffixed with \"Controller\".", nameof(action));
        }

        var rvd = new CustomRouteValueDictionary
        {
            { "Controller", controllerName },
            { "Action", call.Method.Name }
        };

        AddParameterValuesFromExpressionToDictionary(rvd, call, excludeDefaultValues, preserveTypes);

        var ignoreRouteValues = new HashSet<string>();
        foreach (var ignoreRouteValueFilterAttribute in AttributeCache.GetCustomAttributes<IgnoreRouteValueFilterAttribute>(call.Method, true).Concat(
                                                            AttributeCache.GetCustomAttributes<IgnoreRouteValueFilterAttribute>(call.Method.DeclaringType, true)))
        {
            var keys = ignoreRouteValueFilterAttribute.Keys;
            if (keys != null)
            {
                ignoreRouteValues.AddRange(keys);
            }
        }

        var allowAnonymousIgnoreRouteValues = AllowAnonymousIgnoreRouteValues;
        if (allowAnonymousIgnoreRouteValues != null && AttributeCache.GetCustomAttributes<AllowAnonymousAttribute>(call.Method, true).Concat(
                AttributeCache.GetCustomAttributes<AllowAnonymousAttribute>(call.Method.DeclaringType, true)).Any())
        {
            foreach (var allowAnonymousIgnoreRouteValue in allowAnonymousIgnoreRouteValues)
            {
                ignoreRouteValues.Add(allowAnonymousIgnoreRouteValue);
            }
        }

        var result = new ExpressionRouteInfo
        {
            Action = new ControllerActionDescriptor()
            {
                MethodInfo = call.Method,
                ActionName = call.Method.Name,
                ControllerTypeInfo = typeof(TController).GetTypeInfo()
            },
            ExcludeRouteValues = ignoreRouteValues,
            RouteValues = rvd,
        };

        ExpressionRouteInfoProxy?.Invoke(result);

        return result;
    }

    public static event Action<ExpressionRouteInfo> ExpressionRouteInfoProxy;

    public static event Action<ExpressionRouteInfo, ActionData> ProcessInferredActionData;

    public static string[] AllowAnonymousIgnoreRouteValues { get; set; }

    private static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary rvd, MethodCallExpression call, bool excludeDefaultValues, bool preserveTypes)
    {
        var parameters = call.Method.GetParameters();
        if (parameters.Length == 0)
        {
            return;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            var arg = call.Arguments[i];
            object value = null;
            var ce = arg as ConstantExpression;

            if (ce != null)
            {
                // If argument is a constant expression, just get the value
                value = ce.Value;
            }
            else
            {
                // Otherwise, convert the argument subexpression to type object,
                // make a lambda out of it, compile it, and invoke it to get the value
                var lambdaExpression = Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)));
                var func = lambdaExpression.Compile();

                value = func();
            }

            if ((value != null) && (!preserveTypes))
            {
                var type = value.GetType();
                if (type != typeof(string) && type.IsClass)
                {
                    var subValues = new CustomRouteValueDictionary(value, excludeDefaultValues);
                    foreach (var subValue in subValues)
                    {
                        rvd.Add(subValue.Key, subValue.Value);
                    }

                    continue;
                }
            }

            rvd.Add(parameters[i].Name, value);
        }
    }
}