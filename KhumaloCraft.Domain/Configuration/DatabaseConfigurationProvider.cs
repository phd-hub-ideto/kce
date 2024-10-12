using KhumaloCraft.Data.Entities;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Domain;

public class DatabaseConfigurationProvider : ConfigurationProvider, IDisposable
{
    public static DatabaseConfigurationProvider Instance = new();

    private Task _reloadTask;

    private readonly CancellationTokenSource _reloadTaskCancellationTokenSource;

    private Dictionary<string, string> _settings;

    private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

    private DatabaseConfigurationProvider()
    {
        _reloadTaskCancellationTokenSource = new CancellationTokenSource();
        _reloadTask = ReloadTask(_reloadTaskCancellationTokenSource.Token);
    }

    public override bool TryGet(string key, out string value)
    {
        if (_settings == null)
        {
            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                if (_settings == null)
                {
                    using var scope = DalScope.Begin();

                    _settings = scope.KhumaloCraft.Setting
                        .ToDictionary(item => item.Name, item => item.Value, StringComparer.InvariantCultureIgnoreCase);
                }
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        _readerWriterLockSlim.EnterReadLock();

        try
        {
            return _settings.TryGetValue(key, out value);
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }

    private async Task ReloadTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(10), token);
            try
            {
                Clear();
                Reload();
            }
            catch (Exception)
            {
                //swallow errors here
                // if we can't talk to sql for long enough for the exception to be observed and thrown
                // this task becomes faulted and we never see setting updates again
            }
        }
    }

    private void Clear()
    {
        _readerWriterLockSlim.EnterWriteLock();
        try
        {
            _settings = null;
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
    }

    public void Reload()
    {
        Clear();
        OnReload();
    }

    public void Dispose()
    {
        _reloadTaskCancellationTokenSource.Cancel();
        _reloadTask = null;
    }
}