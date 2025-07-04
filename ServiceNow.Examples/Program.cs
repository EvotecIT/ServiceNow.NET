using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Text.Json;
using ServiceNow.Utilities;

var settings = new ServiceNowSettings {
    BaseUrl = "https://instance.service-now.com",
    Username = "admin",
    Password = "password"
};

var services = new ServiceCollection();
services.AddServiceNow(settings);
using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<ServiceNowClient>();

Console.WriteLine("Retrieving problem...");
var problem = await client.Table<Problem>("problem").GetAsync("example_sys_id", null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    problem,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Retrieving configuration items...");
var cis = await client.Table<ConfigurationItem>("cmdb_ci").ListAsync(null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    cis,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Creating record...");
var payload = new Dictionary<string, string?> { ["short_description"] = "Created via example" };
await client.Table<Dictionary<string, string?>>("incident").CreateAsync(payload, CancellationToken.None);
Console.WriteLine("Done");