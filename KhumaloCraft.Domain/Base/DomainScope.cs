using System.Data;
using System.Runtime.CompilerServices;
using KhumaloCraft.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhumaloCraft.Domain;

public class DomainScope : IDisposable
{
    //https://docs.microsoft.com/en-us/sql/t-sql/statements/alter-database-transact-sql-compatibility-level
    private static readonly Dictionary<int, string> _compatibilityLevels = new Dictionary<int, string>
    {
        [80] = "SQL Server 2000",
        [90] = "SQL Server 2005",
        [100] = "SQL Server 2008, SQL Server 2008 R2",
        [110] = "SQL Server 2012",
        [120] = "SQL Server 2014",
        [130] = "SQL Server 2016, Azure SQL Database",
        [140] = "SQL Server 2017",
    };

    internal void AfterCommit(Action callback)
    {
        _dalScope.AfterCommit(callback);
    }

    public static DateTime DBServerDate()
    {
        using var scope = Begin();

        return scope.ServerDate();
    }

    public static bool InScope => DalScope.Root is not null;

    private IDalScope _dalScope;

    private DomainScope(IDalScope dalScope)
    {
        _dalScope = dalScope;
    }

    public void Commit()
    {
        _dalScope.Commit();
    }

    //TODO-LP : Convert DomainScope into a transaction and don't allow updates of on a transaction.
    //For now we will use: using var scope = new TransactionScope();
    public static DomainScope Begin()
    {
        return new DomainScope(DalScope.Begin());
    }

    public static DomainScope Begin(int timeout)
    {
        return new DomainScope(DalScope.Begin(timeout));
    }

    public void Dispose()
    {
        _dalScope.Dispose();
    }

    public DateTime ServerDate()
    {
        return _dalScope.ServerDate();
    }

    public IDbCommand CreateCommand(string commandText)
    {
        using var scope = DalScope.Begin();

        var command = scope.KhumaloCraft.Database.GetDbConnection().CreateCommand();

        command.CommandText = commandText;

        return command;
    }

    private T ExecuteScalar<T>(string commandText)
    {
        using var command = _dalScope.KhumaloCraft.Database.GetDbConnection().CreateCommand();

        command.CommandText = commandText;

        return (T)command.ExecuteScalar();
    }

    public static void EnsureInScope([CallerMemberName] string callerMemberName = null)
    {
        if (!InScope)
        {
            throw new InvalidOperationException($"A transaction is required for calls to {callerMemberName}.");
        }
    }

    public string User
    {
        get
        {
            return ExecuteScalar<string>("SELECT SUSER_NAME()");
        }
    }
    public string DatabaseName
    {
        get
        {
            return ExecuteScalar<string>("SELECT DB_NAME()");
        }
    }
    public string ServerName
    {
        get
        {
            return ExecuteScalar<string>("SELECT @@SERVERNAME");
        }
    }
    public string ServerVersion
    {
        get
        {
            return ExecuteScalar<string>("SELECT @@VERSION");
        }
    }

    public string ServerCompatibilityLevel
    {
        get
        {
            var compatibilityLevelFromDB = ExecuteScalar<byte>($"SELECT compatibility_level FROM sys.databases WHERE name = '{DatabaseName}'");

            if (_compatibilityLevels.TryGetValue(compatibilityLevelFromDB, out string compatibilityLevel))
            {
                return compatibilityLevel;
            }

            return compatibilityLevelFromDB.ToString();
        }
    }

    public TimeSpan? ServerUpTime
    {
        get
        {
            try
            {
                using (var command = CreateCommand("select create_date from sys.databases where database_id = 2")) // tempdb
                {
                    return ServerDate() - (DateTime)command.ExecuteScalar();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}