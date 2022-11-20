namespace QueryPack.Examples.Predicates
{
    using Models;
    using QueryPack.Predicates;

    internal class SearchConfigurationExample
    {
        class SearchUser : ISearchModel
        {
            public string UserName { get; set; }

            public string UserEmail { get; set; }
        }

        class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        class UserSearchBuilder : GenericPredicateBuilder<User, SearchUser>
        {
            public UserSearchBuilder()
            {
                With(m => e => e.Name.StartsWith(m.UserName),
                    r => r.When(m => !string.IsNullOrEmpty(m.UserName)));
            }
        }

        public void Run()
        {
            var searchBuilder = new UserSearchBuilder();
            var predicate = searchBuilder.Build();
            var expression = predicate.Get(new SearchUser
            {
                UserName = "jhon"
            });

            Console.WriteLine($"Run example of {nameof(SearchConfigurationExample)}");
            Console.WriteLine(expression);
            Console.WriteLine("end");
        }
    }
}
