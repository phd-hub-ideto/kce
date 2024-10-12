namespace KhumaloCraft.Application.Session;

public interface IUniqueUserTracker
{
    void EnsureCookie();
    string GetUniqueUserId();
    void IssueCookie();
}