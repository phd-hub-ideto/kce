using KhumaloCraft.Data.Entities;

namespace KhumaloCraft.Data.Sql.Queries;

internal static class QueryContainerFactory
{
    internal static IQueryable<T> Create<T>(Func<IDalScope, IQueryable<T>> queryFactory)
    {
        return new QueryContainer<T, T>(queryFactory);
    }
}