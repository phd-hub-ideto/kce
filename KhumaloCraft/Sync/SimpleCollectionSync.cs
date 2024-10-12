namespace KhumaloCraft.Sync;

public static class SimpleCollectionSync
{
    public static void Sync<R>(IEnumerable<R> source, IEnumerable<R> destination, out IEnumerable<R> updates, out IEnumerable<R> removes, out IEnumerable<R> adds,
        IEqualityComparer<R> comparer = null)
    {
        var s = source?.Any() == true ? source : Enumerable.Empty<R>();
        var d = destination?.Any() == true ? destination : Enumerable.Empty<R>();

        updates = s.Intersect(d, comparer);
        removes = d.Except(updates, comparer);
        adds = s.Except(updates, comparer);
    }
}