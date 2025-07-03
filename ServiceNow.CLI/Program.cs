using System.CommandLine;
using ServiceNow.Clients;
using System.Net.Http;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Text.Json;

var baseUrlOption = new Option<string>("--base-url", description: "ServiceNow instance base URL") { IsRequired = true };
var usernameOption = new Option<string>("--username", description: "Username") { IsRequired = true };
var passwordOption = new Option<string>("--password", description: "Password") { IsRequired = true };
var userAgentOption = new Option<string>("--user-agent", () => "ServiceNow.NET", "User agent") { IsRequired = false };

var root = new RootCommand("ServiceNow CLI");
root.AddGlobalOption(baseUrlOption);
root.AddGlobalOption(usernameOption);
root.AddGlobalOption(passwordOption);
root.AddGlobalOption(userAgentOption);

var tableArg = new Argument<string>("table", "Table name");
var sysIdArg = new Argument<string>("sysId", "Record sys_id");
var getCmd = new Command("get-record", "Retrieve a record")
{
    tableArg,
    sysIdArg
};
getCmd.SetHandler(async (string table, string sysId, string baseUrl, string username, string password, string userAgent) =>
{
    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = await tableClient.GetRecordAsync<TaskRecord>(table, sysId).ConfigureAwait(false);
    Console.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
}, tableArg, sysIdArg, baseUrlOption, usernameOption, passwordOption, userAgentOption);

var createTableArg = new Argument<string>("table", "Table name");
var createDataOpt = new Option<string>("--data", "JSON payload") { IsRequired = true };
var createCmd = new Command("create-record", "Create a record")
{
    createTableArg,
    createDataOpt
};
createCmd.SetHandler(async (string table, string data, string baseUrl, string username, string password, string userAgent) =>
{
    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data) ?? new();
    await tableClient.CreateRecordAsync(table, record).ConfigureAwait(false);
    Console.WriteLine("Record created.");
}, createTableArg, createDataOpt, baseUrlOption, usernameOption, passwordOption, userAgentOption);

var updateTableArg = new Argument<string>("table", "Table name");
var updateSysIdArg = new Argument<string>("sysId", "Record sys_id");
var updateDataOpt = new Option<string>("--data", "JSON payload") { IsRequired = true };
var updateCmd = new Command("update-record", "Update a record")
{
    updateTableArg,
    updateSysIdArg,
    updateDataOpt
};
updateCmd.SetHandler(async (string table, string sysId, string data, string baseUrl, string username, string password, string userAgent) =>
{
    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data) ?? new();
    await tableClient.UpdateRecordAsync(table, sysId, record).ConfigureAwait(false);
    Console.WriteLine("Record updated.");
}, updateTableArg, updateSysIdArg, updateDataOpt, baseUrlOption, usernameOption, passwordOption, userAgentOption);

root.AddCommand(getCmd);
root.AddCommand(createCmd);
root.AddCommand(updateCmd);

return await root.InvokeAsync(args);
