namespace QueryPack.Configuration.Impl
{
    using Projections.Infrastructure.Binders;
    using System;

    public class DeafultQueryBuilderConfigurer<TEntity> : IQueryBuilderConfigurer<TEntity>
        where TEntity : class
    {
        public IQueryBuilderConfigurer<TEntity> WithId(string id)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilderConfigurer<TEntity> WithProjection(Action<IProjectionBuilderConfigurer<TEntity>> configurer)
        {
            var instance = new DefaultProjectionBuilderConfigurer<TEntity>(new AnonymousPropertyBinder<TEntity>());
            configurer?.Invoke(instance);

            return this;
        }
    }
}
