namespace QueryPack.Configuration
{
    using Query;
    using System;

    public interface IQueryBuilderConfigurer<TEntity> where TEntity : class
    {
        IQueryBuilderConfigurer<TEntity> WithQueryExecuter<TSource>(string queryId, 
        Action<IProjectionBuilderConfigurer<TEntity>> projectionConfigurer,
        Action<IPredicateBuilderConfigurer<TEntity>> predicateConfigurer = null) where TSource : class, 
            IQueryExecuter<TEntity>;
    }
}
