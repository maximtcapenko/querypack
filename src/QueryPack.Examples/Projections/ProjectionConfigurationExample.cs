namespace QueryPack.Examples.Projections
{
    using Configuration;
    using Configuration.Impl;
    using Query;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class ProjectionConfigurationExample
    {
        class Role
        {
            public string Name { get; set; }
        }

        class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public Role Role { get; set; }
        }

        class UserQueryConfiguration : IQueryConfiguration<User>
        {
            public void Configure(IQueryBuilderConfigurer<User> configurer)
            {
                configurer.WithQueryExecuter<UserQueryExecuter>("search.all",
                                p => p.Property(e => e.Name)
                                      .Property(e => e.Email)
                                      .Property("RoleName", e => e.Role.Name));
            }
        }

        class UserQueryExecuter : IQueryExecuter<User>
        {
            private readonly List<User> _users = new List<User>
            {
                new User
                {
                      Name = "first",
                      Role = new Role
                      {
                          Name = "test role"
                      }
                }
            };

            public Task<IEnumerable<TProjection>> ExecuteQueryAsync<TProjection>(Expression<Func<User, TProjection>> projection)
                where TProjection : class
            {
                var results = _users.AsQueryable().Select(projection).AsEnumerable();
                return Task.FromResult(results);
            }
        }

        class DefaultContext : IQueryExecutionContext
        {
            public DefaultContext(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }
        }

        public async Task Run()
        {
            var services = new ServiceCollection();
            services.AddSingleton<UserQueryExecuter>();

            var configuration = new UserQueryConfiguration();
            var configurer = new DeafultQueryBuilderConfigurer<User>();
            configuration.Configure(configurer);

            var serviceProvider = services.BuildServiceProvider();
            var queryExecuter = configurer.GetQueryExecuter("search.all");
            var context = new DefaultContext(serviceProvider);

            var result = await queryExecuter.ExecuteCollectionAsync(context);
        }
    }
}
