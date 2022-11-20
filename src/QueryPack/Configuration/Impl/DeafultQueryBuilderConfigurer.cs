namespace QueryPack.Configuration.Impl
{
    using Projections.Infrastructure;
    using Projections.Infrastructure.Binders;
    using Query;
    using System;
    using System.Collections.Generic;

    public class DeafultQueryBuilderConfigurer<TEntity> : IQueryBuilderConfigurer<TEntity>, 
        IQueryExecuterProvider
        where TEntity : class
    {
        private readonly Dictionary<string, AnonymousPropertyBinder<TEntity>> _queryProviders =
        new Dictionary<string, AnonymousPropertyBinder<TEntity>>();

        public IQueryBuilderConfigurer<TEntity> WithQueryExecuter<TSource>(string queryId, 
        Action<IProjectionBuilderConfigurer<TEntity>> projectioncConfigurer,
        Action<IPredicateBuilderConfigurer<TEntity>> predicateConfigurer = null) where TSource : class,
            IQueryExecuter<TEntity>
        {
            var projectionBinder = new AnonymousPropertyBinder<TEntity>(typeof(TSource));

            var projectionConfigurerInstance = new DefaultProjectionBuilderConfigurer<TEntity>(projectionBinder);
            projectioncConfigurer?.Invoke(projectionConfigurerInstance);

            var predicateConfigurerInstance = new DefaultPredicateBuilderConfigurer<TEntity>();
            predicateConfigurer?.Invoke(predicateConfigurerInstance);
            projectionBinder.AddPredicates(predicateConfigurerInstance.Predicates);

            _queryProviders[queryId] = projectionBinder;
            return this;
        }

        public IQueryExecuter GetQueryExecuter(string id)
        {
            if(_queryProviders.TryGetValue(id, out var provider))
                return provider.GetQueryExecuter();

            return null;
        }
    }
}
