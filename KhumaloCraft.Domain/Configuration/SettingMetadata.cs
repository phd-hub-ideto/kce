using KhumaloCraft;

namespace KhumaloCraft.Domain.Configuration;

public class SettingMetadata
{
    private static readonly List<SettingMetadata> _settingMetadatas;
    private static readonly Dictionary<string, SettingMetadata> _settingMetadataDictionary;

    private SettingMetadata(string name, bool isUserEditable, string description, Type type, bool hasDefault, object defaultValue)
    {
        Name = name;
        IsUserEditable = isUserEditable;
        Description = description;
        Type = type;
        HasDefault = hasDefault;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public bool IsUserEditable { get; }
    public string Description { get; }
    public Type Type { get; }
    public bool HasDefault { get; set; }
    public object DefaultValue { get; }

    static SettingMetadata()
    {
        var settingMetadatas = new List<SettingMetadata>();

        var settings = typeof(ISettings).GetProperties();

        foreach (var setting in settings)
        {
            var settingAttribute = setting.GetCustomAttributes(typeof(UserEditableSettingAttribute), false).Cast<UserEditableSettingAttribute>().FirstOrDefault();
            var defaultAttribute = setting.GetCustomAttributes(typeof(DefaultAttribute), false).Cast<DefaultAttribute>().FirstOrDefault();

            var isUserEditable = settingAttribute != null;
            var description = settingAttribute?.Description;
            var type = setting?.PropertyType ?? typeof(bool);
            var hasDefault = defaultAttribute != null;
            var defaultValue = defaultAttribute?.Value;

            settingMetadatas.Add(new SettingMetadata(setting.Name, isUserEditable, description, type, hasDefault, defaultValue));
        }

        _settingMetadatas = settingMetadatas;
        _settingMetadataDictionary = settingMetadatas.ToDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase);
    }

    public static bool ExistsByName(string name)
    {
        return _settingMetadataDictionary.Keys.Contains(name);
    }

    public static IEnumerable<SettingMetadata> Fetch()
    {
        return _settingMetadatas;
    }

    public static SettingMetadata FetchByName(string name)
    {
        return _settingMetadataDictionary[name];
    }
}