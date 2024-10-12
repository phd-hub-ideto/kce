using System.Linq.Expressions;

namespace KhumaloCraft.Data.Sql.Queries;

internal class SourceParamaterInjectorExpressionVisitor(
    ConstantExpression sourceParameter,
    Expression expression) : ExpressionVisitor
{
    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Equals(sourceParameter))
        {
            return expression;
        }

        return node;
    }
}