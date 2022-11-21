using QueryPack.Models;
using QueryPack.Predicates;

namespace QueryPack.Configuration
{
    public interface IPredicateBuilderConfigurer<TEntity>
        where TEntity : class
    {
        IPredicateBuilderConfigurer<TEntity> Predicate<TSearch, TBuilder>() 
            where TSearch : ISearchModel
            where TBuilder : IPredicateBuilder<TEntity, TSearch>;
    }
}