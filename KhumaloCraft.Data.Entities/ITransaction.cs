namespace KhumaloCraft.Data.Entities;

public interface ITransaction
{
    void AfterCommit(Action callback);
}