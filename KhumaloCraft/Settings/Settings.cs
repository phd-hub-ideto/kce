namespace KhumaloCraft;

public static class Settings
{
    private static ISettings _settings;

    public static ISettings Instance => _settings ?? throw new InvalidOperationException("Settings are not currently configured.");

    public static void Configure(ISettings settings)
    {
        if (_settings != null)
            throw new InvalidOperationException("Settings are already configured.");

        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
}