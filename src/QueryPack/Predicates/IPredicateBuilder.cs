namespace QueryPack.Predicates
{
    using Models;
    using System;
    using System.Linq.Expressions;

    public interface IPredicate<TEntity> where TEntity : class
    {
         Expression<Func<TEntity, bool>> Get(ISearchModel search);
    }

    public interface IPredicateBuilder<TEntity> where TEntity : class
    {
        IPredicate<TEntity> Build();
    }

    public interface IPredicateBuilder<TEntity, TModel> : IPredicateBuilder<TEntity>
        where TEntity : class
        where TModel : ISearchModel
    {
        ICriteriaBuilder<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }

    public interface IRestriction<TModel>
         where TModel : ISearchModel
    {
        void When(Func<TModel, bool> validation);
    }

    public interface ICriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : ISearchModel
    {
        ICriteriaBuilder<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
        ICriteriaBuilder<TEntity, TModel> And(Action<IBlockPredicateBuilder<TEntity, TModel>> block);

        ICriteriaBuilder<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
        ICriteriaBuilder<TEntity, TModel> Or(Action<IBlockPredicateBuilder<TEntity, TModel>> block);
    }

    public interface IBlockPredicateBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : ISearchModel
    {
        IBlockCriteriaBuilder<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }

    public interface IBlockCriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : ISearchModel
    {
        IBlockCriteriaBuilder<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
        IBlockCriteriaBuilder<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }
}
