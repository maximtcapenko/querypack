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
        Action<IProjectionBuilderConfigurer<TEntity>> configurer) where TSource : class,
            IQueryExecuter<TEntity>
        {
            var projectionBuilder = new AnonymousPropertyBinder<TEntity>(typeof(TSource));
            var instance = new DefaultProjectionBuilderConfigurer<TEntity>(projectionBuilder);
            configurer?.Invoke(instance);

            _queryProviders[queryId] = projectionBuilder;
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
