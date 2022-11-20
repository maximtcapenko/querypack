using QueryPack.Examples.Query;
using QueryPack.Examples.Predicates;

var simpleExample = new SimplePredicateExample();
simpleExample.Run();

var configExample = new SearchConfigurationExample();
configExample.Run();

var projectionExample = new FullQueryConfigurationExample();
await projectionExample.Run();