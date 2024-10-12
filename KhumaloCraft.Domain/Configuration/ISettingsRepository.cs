namespace KhumaloCraft.Domain.Configuration;

public interface ISettingsRepository
{
    List<Setting> Fetch();
    Setting FetchByName(string name);
    string FetchValueByName(string name);
    void Update(string name, string value);
}