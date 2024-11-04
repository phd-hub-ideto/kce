namespace KhumaloCraft.Data.Entities;

public class DalScope : IDalScope, ITransaction
{
    public static bool UseDatabaseServerDate { get; set; } = true;

    private static readonly AsyncLocal<DalScope> _rootDalScope = new AsyncLocal<DalScope>();

    private KhumaloCraftDbContext _dbContext;
    private List<Action> _afterCommit;

    protected bool HasCurrentScopeCommitted { get; set; }

    private bool _disposed = false;

    private static DalScope RootDalScope
    {
        get
        {
            return _rootDalScope.Value;
        }
        set
        {
            _rootDalScope.Value = value;
        }
    }

    public static IDalScope Root => RootDalScope;

    private DalScope(int commandTimeout = 30)
    {
        _dbContext = new KhumaloCraftDbContext(commandTimeout);
    }

    public static IDalScope Begin()
    {
        return BeginInternal();
    }

    public static IDalScope Begin(int commandTimeout = 30)
    {
        return BeginInternal(commandTimeout);
    }

    private static DalScope BeginInternal(int commandTimeout = 30)
    {
        if (_rootDalScope.Value == null)
        {
            var scope = new DalScope(commandTimeout);

            _rootDalScope.Value = scope;

            return scope;
        }
        else
        {
            return _rootDalScope.Value;
        }
    }

    public DateTime ServerDate()
    {
        //TODO-LP : Better way for determining time. For now, we will use South African Time (UTC + 2 hours)
        return UseDatabaseServerDate ? DateTime.UtcNow.AddHours(2) : HighResolutionDateTime.Now;
    }

    public void Commit()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DalScope));

        HasCurrentScopeCommitted = true;

        _dbContext.SaveChanges();
    }

    public void AfterCommit(Action callback)
    {
        if (HasCurrentScopeCommitted) throw new InvalidOperationException($"{nameof(AfterCommit)} may not be run after the first commit in a scope");

        _afterCommit ??= [];

        _afterCommit.Add(callback);
    }

    public KhumaloCraftDbContext KhumaloCraft
    {
        get
        {
            return _disposed ? throw new ObjectDisposedException(nameof(DalScope)) : _dbContext;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (HasCurrentScopeCommitted)
            {
                if (_afterCommit != null)
                {
                    var afterCommit = _afterCommit;

                    _afterCommit = null;

                    foreach (var callback in afterCommit)
                    {
                        callback();
                    }
                }
            }

            _dbContext?.Dispose();

            RootDalScope = null;

            _disposed = true;
        }
    }
}