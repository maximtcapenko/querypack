namespace QueryPack.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IQueryExecutionContext
    {
        IServiceProvider ServiceProvider { get; }
    }

    public interface IQueryExecuter
    {
        Task<IEnumerable<object>> ExecuteCollectionAsync(IQueryExecutionContext context);
    }

    public interface IQueryExecuter<TEntity>
    {
        Task<IEnumerable<TProjection>> ExecuteQueryAsync<TProjection>(Expression<Func<TEntity, TProjection>> projection)
         where TProjection : class;
    }
}
