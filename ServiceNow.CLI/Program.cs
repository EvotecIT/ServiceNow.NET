using ServiceNow.Builders;

Console.WriteLine("Query builder example from CLI:");
var query = new QueryBuilder()
    .Add("sysparm_limit", "5")
    .AddIfNotNull("sysparm_query", "active=true")
    .Build();
Console.WriteLine(query);
