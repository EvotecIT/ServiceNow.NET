using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net.Http;
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
getCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(tableArg);
    var sysId = ctx.ParseResult.GetValueForArgument(sysIdArg);
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = await tableClient.GetRecordAsync<TaskRecord>(table, sysId, cancellationToken).ConfigureAwait(false);
    Console.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true }));
});

var createTableArg = new Argument<string>("table", "Table name");
var createDataOpt = new Option<string>("--data", "JSON payload") { IsRequired = true };
var createCmd = new Command("create-record", "Create a record")
{
    createTableArg,
    createDataOpt
};
createCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(createTableArg);
    var data = ctx.ParseResult.GetValueForOption(createDataOpt)!;
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data) ?? new();
    await tableClient.CreateRecordAsync(table, record, cancellationToken).ConfigureAwait(false);
    Console.WriteLine("Record created.");
});

var updateTableArg = new Argument<string>("table", "Table name");
var updateSysIdArg = new Argument<string>("sysId", "Record sys_id");
var updateDataOpt = new Option<string>("--data", "JSON payload") { IsRequired = true };
var updateCmd = new Command("update-record", "Update a record")
{
    updateTableArg,
    updateSysIdArg,
    updateDataOpt
};
updateCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(updateTableArg);
    var sysId = ctx.ParseResult.GetValueForArgument(updateSysIdArg);
    var data = ctx.ParseResult.GetValueForOption(updateDataOpt)!;
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    using var http = new HttpClient();
    var client = new ServiceNowClient(http, settings);
    var tableClient = new TableApiClient(client);
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data) ?? new();
    await tableClient.UpdateRecordAsync(table, sysId, record, cancellationToken).ConfigureAwait(false);
    Console.WriteLine("Record updated.");
});

root.AddCommand(getCmd);
root.AddCommand(createCmd);
root.AddCommand(updateCmd);

return await root.InvokeAsync(args);