namespace QueryPack.Projections.Infrastructure
{
    interface IBinderInvoker<TEntity> where TEntity : class
    {
        void Bind(IPropertyBinder<TEntity> propertyBinder);
    }
}
