using Microsoft.AspNetCore.Mvc.Filters;

namespace KhumaloCraft.Application.DisplayModes;

public class SupportedDisplayModesAttribute : ActionFilterAttribute
{
    public SupportedDisplayModesAttribute(params DisplayModeType[] displayModes)
    {
        Order = 0;
        SupportedDisplayModes = displayModes ?? Default;
    }

    public static IEnumerable<DisplayModeType> Default { get; } = [DisplayModeType.Desktop, DisplayModeType.SmartPhone];

    public IEnumerable<DisplayModeType> SupportedDisplayModes { get; }

    private readonly static object _supportedDisplayModes = new object();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[_supportedDisplayModes] = SupportedDisplayModes;

        base.OnActionExecuting(context);
    }

    public static bool ShouldRender(IEnumerable<DisplayModeType> supportedDisplayModes, DisplayModeType displayModeType, DisplayModeType selectedDisplayModeType)
    {
        // This action doesn't support this display mode
        if (!supportedDisplayModes.Contains(displayModeType)) return false;

        // If the users choice is for this displaymode and it is supported
        if (selectedDisplayModeType == displayModeType) return true;

        // Fall back to desktop if no other display modes are available, and the action specifically ONLY supports Desktop
        if (displayModeType == DisplayModeType.Desktop
            && supportedDisplayModes.Count() == 1
            && supportedDisplayModes.First() == DisplayModeType.Desktop) return true;

        return false;
    }
}