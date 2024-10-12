using KhumaloCraft.Data.Entities;
using System.Linq.Expressions;

namespace KhumaloCraft.Data.Sql.Queries;

internal class QueryProvider<TSource> : IQueryProvider
{
    private readonly Func<IDalScope, IQueryable<TSource>> _queryFactory;

    public ConstantExpression SourceParameter { get; } = Expression.Constant(null, typeof(IQueryable<TSource>));

    public QueryProvider(Func<IDalScope, IQueryable<TSource>> queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new QueryContainer<TElement, TSource>(expression, this);
    }

    public object Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        using var scope = DalScope.Begin();

        var query = CreateSourceQuery(scope);

        Expression expression1 = InjectSourceParameter(expression, query);

        return query.Provider.Execute<TResult>(expression1);
    }

    internal Expression InjectSourceParameter(Expression expression, IQueryable<TSource> query)
    {
        var expressionVistor = new SourceParamaterInjectorExpressionVisitor(SourceParameter, query.Expression);

        return expressionVistor.Visit(expression);
    }

    internal IQueryable<TSource> CreateSourceQuery(IDalScope scope)
    {
        return _queryFactory(scope);
    }
}
