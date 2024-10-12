using KhumaloCraft.Domain.Configuration;
using KhumaloCraft.Networking;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace KhumaloCraft.Domain;

// Don't give your settings setters unless they should be changed from code.
// Updating them via Manage is handled via the standard domain object pattern.
public class SettingsImpl
{
    private readonly IConfiguration _configuration;

    private SettingsImpl(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private object ConvertValue(Type settingType, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            if (settingType.IsValueType)
            {
                return Activator.CreateInstance(settingType);
            }
            else
            {
                return null;
            }
        }

        var underlyingType = Nullable.GetUnderlyingType(settingType) ?? settingType;

        if (underlyingType == typeof(NetworkAddress[]))
        {
            return value.Split(',').Select(NetworkAddress.Parse).ToArray();
        }
        else if (underlyingType == typeof(Uri))
        {
            return new Uri(value);
        }
        else if (underlyingType == typeof(TimeSpan))
        {
            return TimeSpan.Parse(value);
        }
        else
        {
            return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
        }
    }

    public object Get(Type settingType, string name)
    {
        var value = _configuration[name];

        if (value is null)
        {
            var settingMetadata = SettingMetadata.FetchByName(name);

            if (settingMetadata.HasDefault)
            {
                return settingMetadata.DefaultValue;
            }

            throw new KeyNotFoundException($"Settings '{name}' not available.");
        }

        return ConvertValue(settingType, value);
    }

    public static ISettings Configure(IConfiguration configuration)
    {
        var settingImpl = new SettingsImpl(configuration);

        var settings = SettingsDispatchProxy.Create(settingImpl);

        Settings.Configure(settings);

        return settings;
    }
}