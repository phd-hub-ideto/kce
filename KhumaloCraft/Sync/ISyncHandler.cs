namespace KhumaloCraft.Sync;

public interface ISyncHandler<TSource, TTarget, TItem>
    where TSource : TItem
    where TTarget : TItem
{
    int CompareKey(TItem item1, TItem item2);
    void Process(TSource sourceItem, TTarget targetItem);
    void Add(TSource sourceItem);
    void Delete(TTarget targetItem);
}