namespace QueryPack.Projections.Infrastructure
{
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal class DefaultQueryExecuter<TEntity, TProjection> : IQueryExecuter
        where TEntity : class
        where TProjection : class, IQueryResult
    {
        private readonly Expression<Func<TEntity, TProjection>> _selector;
        private readonly IQueryable<TEntity> _queriable;

        public DefaultQueryExecuter(Expression<Func<TEntity, TProjection>> selector, IQueryable<TEntity> queriable)
        {
            _queriable = queriable;
            _selector = selector;
        }

        public async Task<IEnumerable<IQueryResult>> ExecuteCollectionAsync()
        {
            return await Task.FromResult(_queriable.Select(_selector).AsEnumerable());
        }
    }
}
