namespace QueryPack.Predicates
{
    using Extensions;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class GenericPredicateBuilder<TEntity, TModel> : IPredicateBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : ISearchModel
    {
        private readonly Dictionary<Operator, List<Criteria>> _criterias = new Dictionary<Operator, List<Criteria>>();

        internal enum Operator
        {
            And,
            Or
        }

        internal class BlockPredicateBuilder : IBlockPredicateBuilder<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias = new Dictionary<Operator,List<Criteria>>();

            public IBlockCriteriaBuilder<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestiction<TModel>> restriction = null)
            {
                var criteriaBuilder = new BlockCriteriaBuilder(_criterias);
                criteriaBuilder.And(predicateFactory, restriction);
                return criteriaBuilder;
            }

            public Expression<Func<TEntity, bool>> Build(TModel model) 
                => GenericPredicateBuilder<TEntity, TModel>.Build(_criterias, model);
        }

        internal class BlockCriteriaBuilder : IBlockCriteriaBuilder<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias;

            public BlockCriteriaBuilder(Dictionary<Operator, List<Criteria>> criterias)
            {
                _criterias = criterias;
            }

            public IBlockCriteriaBuilder<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestiction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public IBlockCriteriaBuilder<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, 
                Action<IRestiction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }
        }

        internal class CriteriaBuilder : ICriteriaBuilder<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias;

            public CriteriaBuilder(Dictionary<Operator, List<Criteria>> criterias)
            {
                _criterias = criterias;
            }

            public ICriteriaBuilder<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestiction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public ICriteriaBuilder<TEntity, TModel> And(Action<IBlockPredicateBuilder<TEntity, TModel>> block)
            {
                var builder = new BlockPredicateBuilder();
                block(builder);
                var criteria = new Criteria(m => builder.Build(m));
                if (_criterias.TryGetValue(Operator.And, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                return this;
            }

            public ICriteriaBuilder<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestiction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.Or, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public ICriteriaBuilder<TEntity, TModel> Or(Action<IBlockPredicateBuilder<TEntity, TModel>> block)
            {
                var builder = new BlockPredicateBuilder();
                block(builder);
                var criteria = new Criteria(m => builder.Build(m));
                if (_criterias.TryGetValue(Operator.Or, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                return this;
            }
        }

        internal class Criteria : IRestiction<TModel>
        {
            private Func<TModel, bool> _restriction;
            private readonly Func<TModel, Expression<Func<TEntity, bool>>> _predicateFactory;

            public Criteria(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory)
            {
                _predicateFactory = predicateFactory;
            }

            public void When(Func<TModel, bool> restriction)
            {
                _restriction = restriction;
            }

            public Expression<Func<TEntity, bool>> GetWhenValid(TModel model)
            {
                if (_restriction != null && _restriction(model))
                    return _predicateFactory(model);

                else if (_restriction == null)
                    return _predicateFactory(model);

                return null;
            }
        }

        public ICriteriaBuilder<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestiction<TModel>> restriction = default)
        {
            var criteriaBuilder = new CriteriaBuilder(_criterias);
            criteriaBuilder.And(predicateFactory, restriction);

            return criteriaBuilder;
        }

        internal static Expression<Func<TEntity, bool>> Build(IDictionary<Operator, List<Criteria>> criterias, TModel model)
        {
            if (!criterias.Any())
                return e => true;

            var validCriterias = new Dictionary<Operator, List<Expression<Func<TEntity, bool>>>>();

            foreach (var key in criterias.Keys)
            {
                var validPredicates = new List<Expression<Func<TEntity, bool>>>();

                foreach (var criteria in criterias[key])
                {
                    var validPredicate = criteria.GetWhenValid(model);
                    if (validPredicate != null)
                        validPredicates.Add(validPredicate);
                }

                validCriterias.Add(key, validPredicates);
            }

            if (!validCriterias.Any())
                return e => true;

            Expression<Func<TEntity, bool>> first = null;

            foreach (var key in validCriterias.Keys)
                foreach (var predicate in validCriterias[key])
                {
                    first = key == Operator.And
                        ? first.And(predicate) : first.Or(predicate);
                }

            if (first == null)
                return e => true;

            return first;
        }

        public Expression<Func<TEntity, bool>> Build(TModel model) 
            => Build(_criterias, model);
    }
}
