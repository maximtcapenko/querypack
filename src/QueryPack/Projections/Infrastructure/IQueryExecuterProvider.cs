using QueryPack.Query;

namespace QueryPack.Projections.Infrastructure
{
    public interface IQueryExecuterProvider
    {
        IQueryExecuter GetQueryExecuter(string id);
    }
}