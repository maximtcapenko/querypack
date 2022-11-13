namespace QueryPack.Configuration
{
    using Query;
    using System;

    public interface IQueryBuilderConfigurer<TEntity> where TEntity : class
    {
        IQueryBuilderConfigurer<TEntity> WithSource<TSource>() where TSource : class, 
            IQueryDataSource<TEntity>;
        IQueryBuilderConfigurer<TEntity> WithId(string id);
        IQueryBuilderConfigurer<TEntity> WithProjection(Action<IProjectionBuilderConfigurer<TEntity>> configurer);
    }
}
