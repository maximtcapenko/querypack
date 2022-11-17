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
        private readonly Type _anonymousType;
        private readonly ParameterExpression _sourceParameter = Expression.Parameter(typeof(TEntity), "e");
        private readonly ParameterExpression _destinationParameter;
        private readonly Dictionary<string, IMemberValueBinder> _members = new Dictionary<string, IMemberValueBinder>();
        private readonly SubstExpressionVisitor _visitor;
        private readonly Type _queryExecuterType;

        public PropertyBinder(Type anonymousType, Type queryExecuterType)
        {
            _anonymousType = anonymousType;
            _destinationParameter = Expression.Parameter(_anonymousType, "e");
            _visitor = new SubstExpressionVisitor(_sourceParameter);
            _queryExecuterType = queryExecuterType;
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

        public void Bind<TProperty>(string name, Expression<Func<TEntity, TProperty>> property)
        {
            var sourceExpression = _visitor.Visit(property.Body) as MemberExpression;
            var destinationExpression = Expression.PropertyOrField(_destinationParameter, name);
            _members[name] = new ProjectionMemberValueBinder(sourceExpression, destinationExpression);
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

            return (IQueryExecuter)generic.Invoke(this, new object[] { init, _sourceParameter, _queryExecuterType });
        }

        private IQueryExecuter Create<TProjection>(MemberInitExpression memberInit, ParameterExpression parameter,
            Type queryExecuter)
            where TProjection : class
        {
            var projection = Expression.Lambda<Func<TEntity, TProjection>>(memberInit, parameter);
            return new DefaultQueryExecuter<TEntity, TProjection>(projection, queryExecuter);
        }
    }
}
