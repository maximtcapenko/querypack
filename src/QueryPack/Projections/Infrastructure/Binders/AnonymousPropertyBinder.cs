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
        private List<IBinderInvoker<TEntity>> _binderInvokers = new List<IBinderInvoker<TEntity>>();
        private Dictionary<string, IMemberBuilder> _members = new Dictionary<string, IMemberBuilder>();
        private SubstExpressionVisitor _visitor = new SubstExpressionVisitor(Expression.Parameter(typeof(TEntity), "e"));

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

        public void Bind<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            // store property for future usage
            _binderInvokers.Add(new InternalBinderInvoker<TProperty>(property));

            var path = property.GetMemberPath();

            // detect nested properties
            var paths = path.Split('.');

            if (paths.Length > 2) throw new NotSupportedException();

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

        public IQueryExecuter GetQueryExecuter()
        {
            var typeBuilder = AnonymousTypeBuilder.GetBuilder();
            foreach (var bilder in _members.Values)
            {
                bilder.Build(typeBuilder);
            }

            var type = typeBuilder.CreateType();
            var _internalBinder = new PropertyBinder<TEntity>(type);

            foreach (var invoker in _binderInvokers) 
                invoker.Bind(_internalBinder);

            return _internalBinder.GetQueryExecuter();
        }
    }
}
