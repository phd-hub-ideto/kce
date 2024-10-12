using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace KhumaloCraft.Application.Mvc;

public class ExpressionRouteInfo : IEnumerable<KeyValuePair<string, object>>
{
    public ExpressionRouteInfo()
    {
    }

    public ExpressionRouteInfo(ExpressionRouteInfo expressionRouteInfo)
    {
        Action = expressionRouteInfo.Action;

        if (expressionRouteInfo.RouteValues != null)
        {
            RouteValues = expressionRouteInfo.RouteValues.Clone();
        }

        if (expressionRouteInfo.ExcludeRouteValues != null)
        {
            ExcludeRouteValues = new HashSet<string>(expressionRouteInfo.ExcludeRouteValues);
        }
    }

    public string UniqueId => CreateUniqueId();

    private string CreateUniqueId()
    {
        var sb = new StringBuilder();

        var actionDescriptorType = Action.GetType();
        sb.Append(actionDescriptorType.Module.ModuleVersionId);
        sb.Append(actionDescriptorType.MetadataToken);

        sb.Append(Action.DisplayName);

        if (Action is ControllerActionDescriptor controllerAction)
        {
            sb.Append(controllerAction.ControllerName);
            sb.Append(controllerAction.ActionName);
        }

        if (Action is PageActionDescriptor pageAction)
        {
            sb.Append(pageAction.AreaName);
            sb.Append(pageAction.RelativePath);
            sb.Append(pageAction.ViewEnginePath);
        }

        return sb.ToString();
    }

    public ActionDescriptor Action { get; set; }

    public CustomRouteValueDictionary RouteValues { get; set; }

    public HashSet<string> ExcludeRouteValues { get; set; }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        if (RouteValues == null)
        {
            yield break;
        }

        if (ExcludeRouteValues.IsNullOrEmpty())
        {
            foreach (var item in RouteValues)
            {
                yield return item;
            }
        }
        else
        {
            foreach (var item in RouteValues)
            {
                if (ExcludeRouteValues.Contains(item.Key))
                {
                    continue;
                }

                yield return item;
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ExpressionRouteInfo Clone()
    {
        return new ExpressionRouteInfo(this);
    }
}