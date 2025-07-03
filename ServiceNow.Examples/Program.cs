using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net.Http;
using System.Text.Json;

var settings = new ServiceNowSettings {
    BaseUrl = "https://instance.service-now.com",
    Username = "admin",
    Password = "password"
};

using var http = new HttpClient();
var client = new ServiceNowClient(http, settings);
var tableClient = new TableApiClient(client);

Console.WriteLine("Retrieving record...");
var record = await tableClient.GetRecordAsync<TaskRecord>("incident", "example_sys_id", CancellationToken.None);
Console.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));

Console.WriteLine("Creating record...");
var payload = new Dictionary<string, string?> { ["short_description"] = "Created via example" };
await tableClient.CreateRecordAsync("incident", payload, CancellationToken.None);
Console.WriteLine("Done");