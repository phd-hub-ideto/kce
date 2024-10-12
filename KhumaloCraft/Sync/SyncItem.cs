namespace KhumaloCraft.Sync;

public class SyncItem<TSource, TTarget>
{
    public SyncItemMatchType Action { get; }

    public TSource Source { get; }
    public TTarget Target { get; }

    public SyncItem(SyncItemMatchType add, TSource source, TTarget target)
    {
        this.Action = add;
        this.Source = source;
        this.Target = target;
    }

    public static SyncItem<TSource, TTarget> Add(TSource source) => new SyncItem<TSource, TTarget>(SyncItemMatchType.SourceOnly, source, default);
    public static SyncItem<TSource, TTarget> Update(TSource source, TTarget target) => new SyncItem<TSource, TTarget>(SyncItemMatchType.Matched, source, target);
    public static SyncItem<TSource, TTarget> Delete(TTarget target) => new SyncItem<TSource, TTarget>(SyncItemMatchType.TargetOnly, default, target);
}