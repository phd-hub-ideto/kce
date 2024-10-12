using System.Runtime.CompilerServices;

namespace KhumaloCraft.Sync;

public abstract class AsyncSortedEnumerableSync
{
    public static async Task ProcessAsync<TSource, TTarget, TItem>(
        IAsyncEnumerable<TSource> sourceItems,
        IAsyncEnumerable<TTarget> destinationItems,
        IAsyncSyncHandler<TSource, TTarget, TItem> syncHandler,
        CancellationToken cancellationToken)
        where TSource : TItem
        where TTarget : TItem
    {
        await foreach (var item in EnumerateAsync<TSource, TTarget, TItem>(sourceItems, destinationItems, syncHandler.CompareKey, cancellationToken))
        {
            switch (item.Action)
            {
                case SyncItemMatchType.SourceOnly:
                    await syncHandler.AddAsync(item.Source, cancellationToken);
                    break;
                case SyncItemMatchType.Matched:
                    await syncHandler.ProcessAsync(item.Source, item.Target, cancellationToken);
                    break;
                case SyncItemMatchType.TargetOnly:
                    await syncHandler.DeleteAsync(item.Target, cancellationToken);
                    break;
            }
        }
    }

    public static async IAsyncEnumerable<SyncItem<TSource, TTarget>> EnumerateAsync<TSource, TTarget, TItem>(IAsyncEnumerable<TSource> sourceItems, IAsyncEnumerable<TTarget> destinationItems, Func<TItem, TItem, int> compareKey, [EnumeratorCancellation] CancellationToken cancellationToken)
        where TSource : TItem
        where TTarget : TItem
    {
        var checkedSourceItems = CheckOrderAsync(sourceItems, (item1, item2) => compareKey.Invoke(item1, item2), cancellationToken);
        var checkedDestinationItems = CheckOrderAsync(destinationItems, (item1, item2) => compareKey.Invoke(item1, item2), cancellationToken);

        await using (var destinationEnumerator = checkedDestinationItems.GetAsyncEnumerator(cancellationToken))
        {
            bool destinationHasMoreItems = await destinationEnumerator.MoveNextAsync();

            await foreach (var sourceItem in checkedSourceItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (destinationHasMoreItems && compareKey.Invoke(sourceItem, destinationEnumerator.Current) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return SyncItem<TSource, TTarget>.Delete(destinationEnumerator.Current);

                    destinationHasMoreItems = await destinationEnumerator.MoveNextAsync();
                    if (!destinationHasMoreItems) break;
                }

                if (!destinationHasMoreItems || compareKey.Invoke(sourceItem, destinationEnumerator.Current) < 0)
                {
                    yield return SyncItem<TSource, TTarget>.Add(sourceItem);
                }
                else
                {
                    yield return SyncItem<TSource, TTarget>.Update(sourceItem, destinationEnumerator.Current);
                    destinationHasMoreItems = await destinationEnumerator.MoveNextAsync();
                }
            }

            while (destinationHasMoreItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return SyncItem<TSource, TTarget>.Delete(destinationEnumerator.Current);
                destinationHasMoreItems = await destinationEnumerator.MoveNextAsync();
            }
        }
    }

    private static async IAsyncEnumerable<TItem> CheckOrderAsync<TItem>(IAsyncEnumerable<TItem> items, Func<TItem, TItem, int> compare, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        TItem lastItem = default;

        await foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Equals(lastItem, default(TItem))
                && compare(lastItem, item) > 0)
            {
                throw new InvalidOperationException("Items are not sorted in ascending order by key.");
            }

            lastItem = item;
            yield return item;
        }
    }
}
