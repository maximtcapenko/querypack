namespace QueryPack.Configuration.Impl
{
    using Models;
    using Predicates;
    using System;
    using System.Collections.Generic;

    internal class DefaultPredicateBuilderConfigurer<TEntity> : IPredicateBuilderConfigurer<TEntity>
        where TEntity : class
    {
        private readonly List<Type> _predicates = new List<Type>();

        public IPredicateBuilderConfigurer<TEntity> Predicate<TSearch, TBuilder>()
            where TSearch : ISearchModel
            where TBuilder : IPredicateBuilder<TEntity, TSearch>
        {
            _predicates.Add(typeof(TBuilder));
            return this;
        }

        public IEnumerable<Type> Predicates => _predicates;
    }
}