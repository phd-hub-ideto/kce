using System.Reflection;

namespace KhumaloCraft.Domain.Configuration;

public class SettingsDispatchProxy : DispatchProxy
{
    private SettingsImpl _settings;

    public static ISettings Create(SettingsImpl settingsImpl)
    {
        var settings = Create<ISettings, SettingsDispatchProxy>();

        var proxy = (SettingsDispatchProxy)settings;

        proxy._settings = settingsImpl;

        return settings;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        string settingName;

        lock (_mappings)
        {
            if (!_mappings.TryGetValue(targetMethod, out settingName))
            {
                var getProperty = Array.Find(targetMethod.DeclaringType.GetProperties(), prop => prop.GetGetMethod().Name == targetMethod.Name);

                if (getProperty is null)
                {
                    throw new NotSupportedException($"Unable to determine property for method {targetMethod.Name}.");
                }

                settingName = getProperty.Name;

                _mappings[targetMethod] = settingName;
            }
        }

        var returnType = targetMethod.ReturnType;
        return _settings.Get(returnType, settingName);
    }

    private readonly Dictionary<MethodInfo, string> _mappings = new();
}