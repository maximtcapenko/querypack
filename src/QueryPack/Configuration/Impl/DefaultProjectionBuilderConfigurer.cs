namespace QueryPack.Configuration.Impl
{
    using Projections;
    using System;
    using System.Linq.Expressions;

    internal class DefaultProjectionBuilderConfigurer<TEntity> : IProjectionBuilderConfigurer<TEntity>
        where TEntity : class
    {
        private readonly IPropertyBinder<TEntity> _propertyBinder;

        public DefaultProjectionBuilderConfigurer(IPropertyBinder<TEntity> propertyBinder)
        {
            _propertyBinder = propertyBinder;
        }

        public IProjectionBuilderConfigurer<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            _propertyBinder.Bind(property);
            return this;
        }

        public IProjectionBuilderConfigurer<TEntity> Property<TProperty>(string name, Expression<Func<TEntity, TProperty>> property)
        {
            _propertyBinder.Bind(name, property);
            return this;
        }
    }
}
