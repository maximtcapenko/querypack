namespace QueryPack.Projections.Infrastructure
{
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal class DefaultQueryExecuter<TEntity, TProjection> : IQueryExecuter
        where TEntity : class
        where TProjection : class
    {
        private readonly Expression<Func<TEntity, TProjection>> _selector;
        private readonly Type _executerType;

        public DefaultQueryExecuter(Expression<Func<TEntity, TProjection>> selector, 
        Type executerType)
        {
            _executerType = executerType;
            _selector = selector;
        }

        public async Task<IEnumerable<object>> ExecuteCollectionAsync(IQueryExecutionContext context)
        {
            var executer = context.ServiceProvider.GetService(_executerType) as IQueryExecuter<TEntity>;
            var result = await executer?.ExecuteQueryAsync<TProjection>(_selector);
            return result;
        }
    }
}