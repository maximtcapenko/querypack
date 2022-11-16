namespace QueryPack.Projections
{
    using System;
    using System.Linq.Expressions;

    public interface IPropertyBinder<TEntity> where TEntity : class
    {
        void Bind<TProperty>(Expression<Func<TEntity, TProperty>> property);
    }
}
