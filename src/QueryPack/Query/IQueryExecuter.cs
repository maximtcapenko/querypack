namespace QueryPack.Query
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IQueryExecutionContext
    {
        IServiceProvider ServiceProvider { get; }
	    ISearchModel SearchModel { get; }
    }

    public interface IQueryExecuter
    {
        Task<IEnumerable<object>> ExecuteCollectionAsync(IQueryExecutionContext context);
    }

    public interface IQueryExecuter<TEntity>
    {
        Task<IEnumerable<TProjection>> ExecuteQueryAsync<TProjection>(Expression<Func<TEntity, TProjection>> projection,
            Expression<Func<TEntity, bool>> predicate)
         where TProjection : class;
    }
}
