using Microsoft.AspNetCore.Mvc.Filters;

namespace KhumaloCraft.Application.Mvc;

public class IgnoreRouteValueFilterAttribute : ActionFilterAttribute
{
    public string[] Keys { get; set; }

    public IgnoreRouteValueFilterAttribute(params string[] keys)
    {
        Keys = keys;
    }
}
