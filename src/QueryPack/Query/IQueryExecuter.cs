namespace QueryPack.Query
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IProjectionBuilder
    {
        Task<IEnumerable<TProjection>> ExecuteQueryAsync<TEntity, TProjection>(IQueryable<TEntity> source);
    }
    
    public interface IQueryExecuter
    {
        Task<IEnumerable<IQueryResult>> ExecuteCollectionAsync();
    }
}
