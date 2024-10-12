// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using KhumaloCraft.Application.DisplayModes;
using Microsoft.AspNetCore.Mvc.Razor;

namespace KhumaloCraft.Application.Portal;

internal class CustomViewLocationExpander : IViewLocationExpander
{
    private readonly IDisplayModeSelector _displayModeSelector;

    public CustomViewLocationExpander(IDisplayModeSelector displayModeSelector)
    {
        _displayModeSelector = displayModeSelector;
    }

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        context.Values["DisplayMode"] = _displayModeSelector.GetSelected().ToString();
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        foreach (var viewLocation in viewLocations)
        {
            yield return string.Format(viewLocation, context.ViewName, context.ControllerName, context.Values["DisplayMode"]);
        }
    }
}