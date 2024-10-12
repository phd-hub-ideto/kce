using KhumaloCraft.Application.DisplayModes;
using KhumaloCraft.Application.Platforms;
using KhumaloCraft.Domain.Platforms;

namespace KhumaloCraft.Application.Portal.PlatformDetermination;

public class PortalPlatformDeterminator : IPlatformDeterminator
{
    private static readonly Dictionary<DisplayModeType, Platform> _displayModeTypeToPlatformMap = new Dictionary<DisplayModeType, Platform>
    {
        [DisplayModeType.Desktop] = Platform.Desktop,
        [DisplayModeType.SmartPhone] = Platform.SmartPhone
    };

    private readonly IDisplayModeSelector _displayModeSelector;

    public PortalPlatformDeterminator(
        IDisplayModeSelector displayModeSelector
    )
    {
        _displayModeSelector = displayModeSelector;
    }

    public Platform Determine()
    {
        return _displayModeTypeToPlatformMap.TryGetValue(_displayModeSelector.GetSelected(), out var platform)
            ? platform : Platform.Unknown;
    }
}
