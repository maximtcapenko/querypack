namespace QueryPack.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionsExtension
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;

            var parameter = first.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.Subst[second.Parameters[0]] = parameter;

            var body = Expression.And(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;

            var parameter = first.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.Subst[second.Parameters[0]] = parameter;
            
            var body = Expression.Or(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            return memberExpression.Member as PropertyInfo;
        }
    }

    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Subst { get; } = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Subst.TryGetValue(node, out var newValue))
                return newValue;

            return node;
        }
    }
}
