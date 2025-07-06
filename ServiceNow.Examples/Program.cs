using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Enums;
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
var tableClient = provider.GetRequiredService<TableApiClient>();

Console.WriteLine("Retrieving problem...");
var problem = await tableClient.GetRecordAsync<Problem>("problem", "example_sys_id", null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    problem,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Retrieving configuration items...");
var cis = await tableClient.ListRecordsAsync<ConfigurationItem>("cmdb_ci", null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    cis,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Retrieving report...");
var reportClient = provider.GetRequiredService<ReportApiClient>();
var report = await reportClient.GetReportAsync<Dictionary<string, object>>("daily_incidents", null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    report,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Creating record...");
var payload = new Dictionary<string, string?> { ["short_description"] = "Created via example" };
await tableClient.CreateRecordAsync("incident", payload, CancellationToken.None);

Console.WriteLine("Updating incident state...");
await tableClient.SetStateAsync("incident", "example_sys_id", IncidentState.InProgress, CancellationToken.None);

Console.WriteLine("Done");
