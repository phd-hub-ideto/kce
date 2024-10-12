using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Domain.Configuration;

namespace KhumaloCraft.Domain;

public class Setting
{
    internal Setting(DalSetting setting)
    {
        Name = setting.Name;
        Value = setting.Value;
        LastEditedBy = setting.LastEditedByUserId;
        UpdatedDate = setting.UpdatedDate;
        Metadata = SettingMetadata.FetchByName(setting.Name);
    }

    public string Name { get; }
    public string Value { get; }
    public int LastEditedBy { get; }
    public DateTime UpdatedDate { get; }
    public SettingMetadata Metadata { get; }
}