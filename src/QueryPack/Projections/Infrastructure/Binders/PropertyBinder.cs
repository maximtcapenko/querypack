namespace QueryPack.Projections.Infrastructure.Binders
{
    using Extensions;
    using QueryPack.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyBinder<TEntity> : IPropertyBinder<TEntity> where TEntity : class
    {
        Type _anonymousType;
        ParameterExpression _sourceParameter = Expression.Parameter(typeof(TEntity), "e");
        ParameterExpression _destinationParameter;
        Dictionary<string, IMemberValueBinder> _members = new Dictionary<string, IMemberValueBinder>();
        SubstExpressionVisitor _visitor;

        public PropertyBinder(Type anonymousType)
        {
            _anonymousType = anonymousType;
            _destinationParameter = Expression.Parameter(_anonymousType, "e");
            _visitor = new SubstExpressionVisitor(_sourceParameter);
        }

        public void Bind<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var name = property.GetMemberPath();

            // detect nested properties
            var paths = name.Split('.');

            if (paths.Length > 2) throw new NotSupportedException();

            if (paths.Length == 1 && !_members.ContainsKey(paths[0]))
            {
                var sourceExpression = _visitor.Visit(property.Body) as MemberExpression;
                var destinationExpression = Expression.PropertyOrField(_destinationParameter, sourceExpression.Member.Name);
                _members[paths[0]] = new SingleMemberValueBiner(sourceExpression, destinationExpression);
            }
            else
            {
                var sourceExpression = _visitor.Visit(property.Body) as MemberExpression;
                if (!_members.ContainsKey(paths[0]))
                {
                    var destinationExpression = Expression.PropertyOrField(_destinationParameter, paths[0]);
                    _members[paths[0]] = new MultiMemberValueBinder(destinationExpression);
                }

                var binder = _members[paths[0]] as MultiMemberValueBinder;

                binder.AddMemberExpression(sourceExpression);

            }
        }

        public IQueryExecuter GetQueryExecuter()
        {
            var @new = Expression.New(_anonymousType);
            var bindings = new List<MemberBinding>();

            foreach (var binder in _members.Values)
            {
                binder.Bind(bindings);
            }

            var init = Expression.MemberInit(@new, bindings);
            var factory = GetType().GetMethod(nameof(Create), BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = factory.MakeGenericMethod(_anonymousType);

            return (IQueryExecuter)generic.Invoke(this, new object[] { init, _sourceParameter });
        }

        private IQueryExecuter Create<TProjection>(MemberInitExpression memberInit, ParameterExpression parameter,
            IQueryable<TEntity> queryable)
            where TProjection : class, IQueryResult
        {
            var projection = Expression.Lambda<Func<TEntity, TProjection>>(memberInit, parameter);
            var executer = new DefaultQueryExecuter<TEntity, TProjection>(projection, queryable);

            return executer;
        }
    }

}
