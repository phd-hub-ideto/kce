using KhumaloCraft.Data.Entities;
using KhumaloCraft.Domain.Events;

namespace KhumaloCraft.Data.Sql.Transactions;

public class TransactionProvider : ITransactionProvider
{
    public ITransaction GetTransaction()
    {
        return (ITransaction)DalScope.Root;
    }
}