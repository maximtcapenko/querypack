namespace QueryPack.Query
{
    using System.Linq;

    public interface IQueryDataSource<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetQuerySource();
    }
}
