namespace QueryPack.Examples.Query
{
    using Configuration;
    using Configuration.Impl;
    using QueryPack.Predicates;
    using QueryPack.Query;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class FullQueryConfigurationExample
    {
        class SearchUser : ISearchModel
        {
            public string UserName { get; set; }

            public string UserEmail { get; set; }
        }

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

        class UserSearchBuilder : GenericPredicateBuilder<User, SearchUser>
        {
            public UserSearchBuilder()
            {
                With(m => e => e.Name.StartsWith(m.UserName),
                    r => r.When(m => !string.IsNullOrEmpty(m.UserName)));
            }
        }
        class UserQueryConfiguration : IQueryConfiguration<User>
        {
            public void Configure(IQueryBuilderConfigurer<User> configurer)
            {
                configurer.WithQueryExecuter<UserQueryExecuter>("search.all",
                                p => p.Property(e => e.Name)
                                      .Property(e => e.Email)
                                      .Property("RoleName", e => e.Role.Name),
                                q => q.Predicate<SearchUser, UserSearchBuilder>());
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

            public Task<IEnumerable<TProjection>> ExecuteQueryAsync<TProjection>(Expression<Func<User, TProjection>> projection,
             Expression<Func<User, bool>> predicate)
                where TProjection : class
            {
                var results = _users.AsQueryable().Where(predicate).Select(projection).AsEnumerable();
                return Task.FromResult(results);
            }
        }

        class DefaultContext : IQueryExecutionContext
        {
            public DefaultContext(IServiceProvider serviceProvider, ISearchModel search)
            {
                ServiceProvider = serviceProvider;
                SearchModel = search;
            }

            public IServiceProvider ServiceProvider { get; }

            public ISearchModel SearchModel { get; }
        }

        public async Task Run()
        {
            var services = new ServiceCollection();
            services.AddSingleton<UserQueryExecuter>();
            services.AddSingleton<UserSearchBuilder>();

            var configuration = new UserQueryConfiguration();
            var configurer = new DeafultQueryBuilderConfigurer<User>();
            configuration.Configure(configurer);

            var serviceProvider = services.BuildServiceProvider();
            var queryExecuter = configurer.GetQueryExecuter("search.all");
            var context = new DefaultContext(serviceProvider, new SearchUser
            {
            });

            var result = await queryExecuter.ExecuteCollectionAsync(context);
        }
    }
}
