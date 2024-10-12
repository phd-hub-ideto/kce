namespace KhumaloCraft.Application.DisplayModes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DesktopDisplayModeOnlyAttribute : SupportedDisplayModesAttribute
{
    public DesktopDisplayModeOnlyAttribute()
        : base(DisplayModeType.Desktop)
    {
    }
}