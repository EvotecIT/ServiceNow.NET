using ServiceNow.Builders;

Console.WriteLine("Query builder example:");
var query = new QueryBuilder()
    .Add("sysparm_limit", "10")
    .AddIfNotNull("sysparm_query", "active=true")
    .Build();
Console.WriteLine(query);
