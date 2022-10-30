using QueryPack.Predicates;
using QueryPack.Models;
using System;

namespace QueryPack
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

    public class Runner
    {
        public static void Main()
        {
            IPredicateBuilder<User, SearchUser> builder = new GenericPredicateBuilder<User, SearchUser>();
            builder.With(m => e => e.Name == m.UserName)
                   .Or(b => b.With(m => e => e.Email.StartsWith(m.UserEmail))
                             .And(m => e => e.Name == "test"));

            var predicate = builder.Build(new SearchUser { UserName = "jhon" , UserEmail = "jhon@email.com"});

            Console.WriteLine(predicate);
            Console.ReadKey();
        }
    }
}
