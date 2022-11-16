// See https://aka.ms/new-console-template for more information

using QueryPack.Examples.Configuration;
using QueryPack.Examples.Predicates;

var simpleExample = new SimplePredicateExample();
simpleExample.Run();

var configExample = new SearchConfigurationExample();
configExample.Run();

var projectionExample = new ProjectionConfigurationExample();
await projectionExample.Run();

Console.ReadKey();
