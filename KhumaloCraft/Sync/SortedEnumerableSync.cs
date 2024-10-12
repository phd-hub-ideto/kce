namespace KhumaloCraft.Sync;

public abstract class SortedEnumerableSync
{
    public static void Process<TSource, TTarget, TItem>(
        IEnumerable<TSource> sourceItems,
        IEnumerable<TTarget> destinationItems,
        ISyncHandler<TSource, TTarget, TItem> syncHandler,
        CancellationToken cancellationToken)
        where TSource : TItem
        where TTarget : TItem
    {
        foreach (var item in Enumerate<TSource, TTarget, TItem>(sourceItems, destinationItems, syncHandler.CompareKey, cancellationToken))
        {
            switch (item.Action)
            {
                case SyncItemMatchType.SourceOnly:
                    syncHandler.Add(item.Source);
                    break;
                case SyncItemMatchType.Matched:
                    syncHandler.Process(item.Source, item.Target);
                    break;
                case SyncItemMatchType.TargetOnly:
                    syncHandler.Delete(item.Target);
                    break;
            }
        }
    }

    public static IEnumerable<SyncItem<TSource, TTarget>> Enumerate<TSource, TTarget, TItem>(IEnumerable<TSource> sourceItems, IEnumerable<TTarget> destinationItems, Func<TItem, TItem, int> compareKey, CancellationToken cancellationToken)
        where TSource : TItem
        where TTarget : TItem
    {
        var checkedSourceItems = CheckOrder(sourceItems, (item1, item2) => compareKey.Invoke(item1, item2));
        var checkedDestinationItems = CheckOrder(destinationItems, (item1, item2) => compareKey.Invoke(item1, item2));

        using (var destinationEnumerator = checkedDestinationItems.GetEnumerator())
        {
            bool destinationHasMoreItems = destinationEnumerator.MoveNext();

            foreach (var sourceItem in checkedSourceItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (destinationHasMoreItems && compareKey.Invoke(sourceItem, destinationEnumerator.Current) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return SyncItem<TSource, TTarget>.Delete(destinationEnumerator.Current);

                    destinationHasMoreItems = destinationEnumerator.MoveNext();
                    if (!destinationHasMoreItems) break;
                }

                if (!destinationHasMoreItems || compareKey.Invoke(sourceItem, destinationEnumerator.Current) < 0)
                {
                    yield return SyncItem<TSource, TTarget>.Add(sourceItem);
                }
                else
                {
                    yield return SyncItem<TSource, TTarget>.Update(sourceItem, destinationEnumerator.Current);
                    destinationHasMoreItems = destinationEnumerator.MoveNext();
                }
            }

            while (destinationHasMoreItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return SyncItem<TSource, TTarget>.Delete(destinationEnumerator.Current);
                destinationHasMoreItems = destinationEnumerator.MoveNext();
            }
        }
    }

    private static IEnumerable<TItem> CheckOrder<TItem>(IEnumerable<TItem> items, Func<TItem, TItem, int> compare)
    {
        TItem lastItem = default;

        foreach (var item in items)
        {
            if (!Equals(lastItem, default(TItem))
                && compare(lastItem, item) > 0)
            {
                throw new InvalidOperationException($"Items are not sorted in ascending order by key. (Previous={lastItem}, Current={item})");
            }

            lastItem = item;
            yield return item;
        }
    }
}