namespace QueryPack.Examples.Predicates
{
    using Models;
    using QueryPack.Predicates;

    internal class SimplePredicateExample
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


        public void Run()
        {
            var builder = new GenericPredicateBuilder<User, SearchUser>();
            builder.With(m => e => e.Name == m.UserName)
                   .Or(b => b.With(m => e => e.Email.StartsWith(m.UserEmail))
                             .And(m => e => e.Name == "test"));

            var predicate = builder.Build();
            var expression = predicate.Get(new SearchUser { UserName = "jhon", UserEmail = "jhon@email.com" });

            Console.WriteLine($"Run example of {nameof(SimplePredicateExample)}");
            Console.WriteLine(expression);
            Console.WriteLine("end");
        }
    }
}
