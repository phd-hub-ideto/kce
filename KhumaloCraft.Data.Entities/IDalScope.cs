namespace KhumaloCraft.Data.Entities;

public interface IDalScope : IDisposable
{
    KhumaloCraftDbContext KhumaloCraft { get; }
    DateTime ServerDate();
    void Commit();
    void AfterCommit(Action callback);
}