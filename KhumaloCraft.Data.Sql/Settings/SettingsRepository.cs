using KhumaloCraft.Data.Entities;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Configuration;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Data.Sql.Settings;

public class SettingsRepository(
    IPrincipalResolver principalResolver) : ISettingsRepository
{
    public List<Setting> Fetch()
    {
        using var scope = DalScope.Begin();

        return scope.KhumaloCraft.Setting
            .Where(dalSetting => SettingMetadata.ExistsByName(dalSetting.Name))
            .Select(dalSetting => new Setting(dalSetting))
            .ToList();
    }

    public Setting FetchByName(string name)
    {
        using var scope = DalScope.Begin();

        return new Setting(scope.KhumaloCraft.Setting.Single(i => i.Name == name));
    }

    public string FetchValueByName(string name)
    {
        using var scope = DalScope.Begin();

        var dalSetting = scope.KhumaloCraft.Setting.Single(i => i.Name == name);

        return dalSetting.Value;
    }

    public void Update(string name, string value)
    {
        using var scope = DalScope.Begin();

        var dalSetting = scope.KhumaloCraft.Setting.SingleOrDefault(i => i.Name == name);

        if (dalSetting == null) throw new InvalidOperationException("Specified setting does not exist");

        var previousValue = dalSetting.Value;

        dalSetting.Value = value;
        dalSetting.LastEditedByUserId = principalResolver.GetRequiredUserId();
        dalSetting.UpdatedDate = scope.ServerDate();

        scope.KhumaloCraft.Setting.Update(dalSetting);

        scope.Commit();
    }
}