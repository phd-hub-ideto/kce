using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Domain.Events;

public interface ITransactionProvider
{
    ITransaction GetTransaction();
}