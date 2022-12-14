namespace QueryPack.Projections.Infrastructure.Binders
{
    using Builders;
    using Extensions;
    using QueryPack.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class AnonymousPropertyBinder<TEntity> : IPropertyBinder<TEntity> where TEntity : class
    {
        private readonly List<IBinderInvoker<TEntity>> _binderInvokers = new List<IBinderInvoker<TEntity>>();
        private readonly Dictionary<string, IMemberBuilder> _members = new Dictionary<string, IMemberBuilder>();
        private readonly SubstExpressionVisitor _visitor = new SubstExpressionVisitor(Expression.Parameter(typeof(TEntity), "e"));
        private readonly Type _queryExecuterType;
        private List<Type> _predicateBuilderTypes = new List<Type>();

        public AnonymousPropertyBinder(Type queryExecuterType)
        {
            _queryExecuterType = queryExecuterType;
        }

        #region invokers
        class InternalBinderInvoker<TProperty> : IBinderInvoker<TEntity>
        {
            private readonly Expression<Func<TEntity, TProperty>> _property;

            public InternalBinderInvoker(Expression<Func<TEntity, TProperty>> property)
            {
                _property = property;
            }

            public void Bind(IPropertyBinder<TEntity> propertyBinder)
                => propertyBinder.Bind(_property);
        }

        class InternalProjectionBinderInvoker<TProperty> : IBinderInvoker<TEntity>
        {
            private readonly string _name;
            private readonly Expression<Func<TEntity, TProperty>> _property;

            public InternalProjectionBinderInvoker(string name, Expression<Func<TEntity, TProperty>> property)
            {
                _name = name;
                _property = property;
            }

            public void Bind(IPropertyBinder<TEntity> propertyBinder)
                 => propertyBinder.Bind(_name, _property);
        }
        #endregion

        public void Bind<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var path = property.GetMemberPath();

            // detect nested properties
            var paths = path.Split('.');

            if (paths.Length > 2) throw new NotSupportedException();

            // store property for future usage
            _binderInvokers.Add(new InternalBinderInvoker<TProperty>(property));

            if (paths.Length == 1 && !_members.ContainsKey(paths[0]))
                _members[paths[0]] = new SingleMemberBuilder(_visitor.Visit(property.Body) as MemberExpression);
            else
            {
                if (!_members.ContainsKey(paths[0]))
                {
                    var expression = _visitor.Visit(property.Body) as MemberExpression;
                    MemberExpression rootExpression = null;
                    do
                    {
                        rootExpression = expression;
                        expression = expression.Expression as MemberExpression;
                    }
                    while (expression != null);

                    _members[paths[0]] = new MultiMemberBuilder(rootExpression);
                }

                var builder = _members[paths[0]] as MultiMemberBuilder;
                builder.AddMemberExpression(_visitor.Visit(property.Body) as MemberExpression);

            }
        }

        public void Bind<TProperty>(string name, Expression<Func<TEntity, TProperty>> property)
        {
            // should be primitive or string
            if (!(typeof(TProperty).IsPrimitive || typeof(TProperty) == typeof(string)))
                throw new NotSupportedException();

            _binderInvokers.Add(new InternalProjectionBinderInvoker<TProperty>(name, property));
            var path = property.GetMemberPath();

            _members[name] = new RuntimeMemberBuilder(name, typeof(TProperty));

        }

        public void AddPredicates(IEnumerable<Type> predicates)
        {
             _predicateBuilderTypes.AddRange(predicates);
        }

        public IQueryExecuter GetQueryExecuter()
        {
            var typeBuilder = AnonymousTypeBuilder.GetBuilder();
            foreach (var bilder in _members.Values)
            {
                bilder.Build(typeBuilder);
            }

            var type = typeBuilder.CreateType();
            var _internalBinder = new PropertyBinder<TEntity>(type, _queryExecuterType, _predicateBuilderTypes);

            foreach (var invoker in _binderInvokers)
                invoker.Bind(_internalBinder);

            return _internalBinder.GetQueryExecuter();
        }
    }
}
