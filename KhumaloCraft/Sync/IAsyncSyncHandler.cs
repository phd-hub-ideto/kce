namespace KhumaloCraft.Sync;

public interface IAsyncSyncHandler<TSource, TTarget, TItem>
    where TSource : TItem
    where TTarget : TItem
{
    int CompareKey(TItem item1, TItem item2);
    Task ProcessAsync(TSource sourceItem, TTarget targetItem, CancellationToken cancellationToken);
    Task AddAsync(TSource sourceItem, CancellationToken cancellationToken);
    Task DeleteAsync(TTarget targetItem, CancellationToken cancellationToken);
}