namespace QueryPack.Configuration
{
    using System;
    using System.Linq.Expressions;

    public interface IProjectionBuilderConfigurer<TEntity> where TEntity : class
    {
        IProjectionBuilderConfigurer<TEntity> Property<TProperty>(Expression<Func<TEntity, TProperty>> property);
        IProjectionBuilderConfigurer<TEntity> Property<TProperty>(string name, Expression<Func<TEntity, TProperty>> property);
    }
}
