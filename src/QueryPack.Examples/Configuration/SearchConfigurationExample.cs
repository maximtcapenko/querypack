namespace QueryPack.Examples.Configuration
{
    using QueryPack.Configuration;
    using QueryPack.Configuration.Impl;

    internal class SearchConfigurationExample
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
                configurer.WithId("search.all")
                          .WithProjection(p => p.Property(e => e.Name)
                                                .Property(e => e.Email)
                                                .Property(e => e.Role.Name));
            }
        }

        public void Run()
        {
            var configuration = new UserQueryConfiguration();
            var configurer = new DeafultQueryBuilderConfigurer<User>();

            configuration.Configure(configurer);
        }
    }
}
