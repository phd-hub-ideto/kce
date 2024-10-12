using KhumaloCraft.Data.Entities;
using System.Collections;
using System.Linq.Expressions;

namespace KhumaloCraft.Data.Sql.Queries;

internal class QueryContainer<TEntity, TSource> : IQueryable<TEntity>, IOrderedQueryable<TEntity>
{
    private readonly QueryProvider<TSource> _queryProvider;

    public QueryContainer(Expression expression, QueryProvider<TSource> queryProvider)
    {
        _queryProvider = queryProvider;
        Expression = expression;
    }

    public QueryContainer(Func<IDalScope, IQueryable<TSource>> queryFactory)
    {
        _queryProvider = new QueryProvider<TSource>(queryFactory);
        Expression = _queryProvider.SourceParameter;
    }

    public Type ElementType => typeof(TEntity);

    public Expression Expression { get; }

    public IQueryProvider Provider => _queryProvider;

    public IEnumerator<TEntity> GetEnumerator()
    {
        using var scope = DalScope.Begin();

        var query = _queryProvider.CreateSourceQuery(scope);

        return query.Provider
            .CreateQuery<TEntity>(_queryProvider.InjectSourceParameter(Expression, query))
            .ToList()
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}