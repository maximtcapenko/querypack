namespace QueryPack.Configuration
{
    using Query;
    using System;

    public interface IQueryBuilderConfigurer<TEntity> where TEntity : class
    {
        IQueryBuilderConfigurer<TEntity> WithQueryExecuter<TSource>(string queryId, 
        Action<IProjectionBuilderConfigurer<TEntity>> configurer) where TSource : class, 
            IQueryExecuter<TEntity>;
    }
}
