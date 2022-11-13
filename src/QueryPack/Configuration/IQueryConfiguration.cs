namespace QueryPack.Configuration
{
    public interface IQueryConfiguration<TEntity> where TEntity : class
    {
        void Configure(IQueryBuilderConfigurer<TEntity> configurer);
    }
}
