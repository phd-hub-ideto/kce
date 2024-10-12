namespace KhumaloCraft.Application.Monitoring;

public interface IRequestStorage
{
    T Get<T>() where T : class, new();
}