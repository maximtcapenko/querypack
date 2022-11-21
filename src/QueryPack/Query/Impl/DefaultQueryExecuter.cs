namespace QueryPack.Query.Impl
{
    using Query;
    using Extensions;
    using Predicates;
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
        private readonly IEnumerable<Type> _predicateBuilderTypes;

        public DefaultQueryExecuter(Expression<Func<TEntity, TProjection>> selector,
            Type executerType, IEnumerable<Type> predicateBuilderTypes)
        {
            _executerType = executerType;
            _predicateBuilderTypes = predicateBuilderTypes;
            _selector = selector;
        }

        public async Task<IEnumerable<object>> ExecuteCollectionAsync(IQueryExecutionContext context)
        {
            Expression<Func<TEntity, bool>> where = e => true;

            if (_predicateBuilderTypes != null && context.SearchModel != null)
            {
                foreach (var type in _predicateBuilderTypes)
                {
                    var predicateBuilder = context.ServiceProvider.GetService(type) as IPredicateBuilder<TEntity>;
                    var predicate = predicateBuilder.Build();
                    where = where.And(predicate.Get(context.SearchModel));
                }
            }
            var executer = context.ServiceProvider.GetService(_executerType) as IQueryExecuter<TEntity>;
            var result = await executer?.ExecuteQueryAsync<TProjection>(_selector, where);

            return result;
        }
    }
}
