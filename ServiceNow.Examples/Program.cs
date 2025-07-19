using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Enums;
using ServiceNow.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Text.Json;
using ServiceNow.Utilities;
using System.Linq;

var settings = new ServiceNowSettings {
    BaseUrl = "https://instance.service-now.com",
    Username = "admin",
    Password = "password",
    MetadataCacheDuration = TimeSpan.FromMinutes(5)
};

var services = new ServiceCollection();
services.AddServiceNow(settings);
using var provider = services.BuildServiceProvider();
var tableClient = provider.GetRequiredService<TableApiClient>();
var groupClient = provider.GetRequiredService<GroupApiClient>();
var metaClient = provider.GetRequiredService<TableMetadataClient>();

Console.WriteLine("Retrieving table metadata...");
var meta = await metaClient.GetMetadataAsync("incident", CancellationToken.None);
Console.WriteLine($"Fields: {string.Join(",", meta.Fields.Select(f => f.Name))}");

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

Console.WriteLine("Retrieving user groups...");
var groups = await groupClient.ListGroupsAsync(null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    groups,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Streaming incidents...");
await foreach (var inc in tableClient.StreamRecordsAsync<Incident>("incident", 100, CancellationToken.None)) {
    Console.WriteLine(inc.SysId);
}

Console.WriteLine("Retrieving report...");
var reportClient = provider.GetRequiredService<ReportApiClient>();
var report = await reportClient.GetReportAsync<Dictionary<string, object>>("daily_incidents", null, CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    report,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Calling scripted REST API...");
var scriptClient = provider.GetRequiredService<ScriptedRestClient>();
var scriptResult = await scriptClient.GetAsync<Dictionary<string, object>>("/api/x_my_app/endpoint", CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(
    scriptResult,
    new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));

Console.WriteLine("Creating record...");
var payload = new Dictionary<string, string?> { ["short_description"] = "Created via example" };
await tableClient.CreateRecordAsync("incident", payload, CancellationToken.None);

Console.WriteLine("Updating incident state...");
await tableClient.SetStateAsync("incident", "example_sys_id", IncidentState.InProgress, CancellationToken.None);

Console.WriteLine("Starting data export...");
var exportClient = provider.GetRequiredService<DataExportClient>();
var exportId = await exportClient.StartExportAsync(new { table = "incident" }, CancellationToken.None);
var exportResponse = await exportClient.DownloadExportAsync(exportId, CancellationToken.None);
Console.WriteLine($"Export {exportId} status: {exportResponse.StatusCode}");

Console.WriteLine("Done");
