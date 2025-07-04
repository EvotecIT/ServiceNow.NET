using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Text.Json;
using ServiceNow.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
var filterOpt = new Option<string[]>("--filter", "Query filters as key=value pairs") { AllowMultipleArgumentsPerToken = true };
var getCmd = new Command("get-record", "Retrieve a record")
{
    tableArg,
    sysIdArg,
    filterOpt
};
getCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(tableArg);
    var sysId = ctx.ParseResult.GetValueForArgument(sysIdArg);
    var filterPairs = ctx.ParseResult.GetValueForOption(filterOpt) ?? Array.Empty<string>();
    var filters = ParseFilters(filterPairs);
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    var services = new ServiceCollection();
    services.AddServiceNow(settings);
    using var provider = services.BuildServiceProvider();
    var tableClient = provider.GetRequiredService<TableApiClient>();
    var record = await tableClient.GetRecordAsync<TaskRecord>(table, sysId, filters, cancellationToken).ConfigureAwait(false);
    Console.WriteLine(JsonSerializer.Serialize(
        record,
        new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));
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
    var services = new ServiceCollection();
    services.AddServiceNow(settings);
    using var provider = services.BuildServiceProvider();
    var tableClient = provider.GetRequiredService<TableApiClient>();
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data, ServiceNowJson.Default) ?? new();
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
    var services = new ServiceCollection();
    services.AddServiceNow(settings);
    using var provider = services.BuildServiceProvider();
    var tableClient = provider.GetRequiredService<TableApiClient>();
    var record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data, ServiceNowJson.Default) ?? new();
    await tableClient.UpdateRecordAsync(table, sysId, record, cancellationToken).ConfigureAwait(false);
    Console.WriteLine("Record updated.");
});

root.AddCommand(getCmd);
root.AddCommand(createCmd);
root.AddCommand(updateCmd);

var listTableArg = new Argument<string>("table", "Table name");
var listCmd = new Command("list-records", "List records")
{
    listTableArg,
    filterOpt
};
listCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(listTableArg);
    var filterPairs = ctx.ParseResult.GetValueForOption(filterOpt) ?? Array.Empty<string>();
    var filters = ParseFilters(filterPairs);
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    var services = new ServiceCollection();
    services.AddServiceNow(settings);
    using var provider = services.BuildServiceProvider();
    var tableClient = provider.GetRequiredService<TableApiClient>();
    var records = await tableClient.ListRecordsAsync<TaskRecord>(table, filters, cancellationToken).ConfigureAwait(false);
    Console.WriteLine(JsonSerializer.Serialize(
        records,
        new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));
});

root.AddCommand(listCmd);

var genTableArg = new Argument<string>("table", "Table name");
var outputOpt = new Option<string>("--output", "Output file") { IsRequired = true };
var genCmd = new Command("generate-model", "Generate C# model for a table") {
    genTableArg,
    outputOpt
};
genCmd.SetHandler(async (InvocationContext ctx) => {
    var table = ctx.ParseResult.GetValueForArgument(genTableArg);
    var output = ctx.ParseResult.GetValueForOption(outputOpt)!;
    var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
    var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
    var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
    var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
    var cancellationToken = ctx.GetCancellationToken();

    var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
    var services = new ServiceCollection();
    services.AddServiceNow(settings);
    using var provider = services.BuildServiceProvider();
    var metaClient = provider.GetRequiredService<TableMetadataClient>();
    var metadata = await metaClient.GetMetadataAsync(table, cancellationToken).ConfigureAwait(false);
    var code = GenerateClass(metadata);
    File.WriteAllText(output, code);
    Console.WriteLine($"Model written to {output}");
});

root.AddCommand(genCmd);

return await root.InvokeAsync(args);

static Dictionary<string, string?> ParseFilters(IEnumerable<string> pairs) {
    var dict = new Dictionary<string, string?>();
    foreach (var pair in pairs) {
        if (string.IsNullOrEmpty(pair)) {
            continue;
        }

        var idx = pair.IndexOf('=');
        if (idx < 0) {
            dict[pair] = null;
            continue;
        }

        var key = pair.Substring(0, idx);
        var value = pair.Substring(idx + 1);
        dict[key] = value;
    }
    return dict;
}

static string GenerateClass(TableMetadata metadata) {
    var className = ToPascal(metadata.Table);
    var sb = new StringBuilder();
    sb.AppendLine("namespace ServiceNow.Models;");
    sb.AppendLine();
    sb.AppendLine($"public class {className} {{");
    foreach (var field in metadata.Fields) {
        var type = MapType(field.Type);
        var name = ToPascal(field.Name);
        sb.AppendLine($"    public {type} {name} {{ get; set; }}");
    }
    sb.AppendLine("}");
    return sb.ToString();
}

static string ToPascal(string value) {
    if (string.IsNullOrEmpty(value)) return string.Empty;
    var parts = value.Split('_');
    var sb = new StringBuilder();
    foreach (var p in parts) {
        if (p.Length == 0) continue;
        sb.Append(char.ToUpperInvariant(p[0]));
        if (p.Length > 1) sb.Append(p.Substring(1));
    }
    return sb.ToString();
}

static string MapType(string type) => type switch {
    "integer" => "int?",
    "number" => "decimal?",
    "float" => "float?",
    "double" => "double?",
    "boolean" => "bool?",
    "glide_date_time" => "DateTime?",
    "glide_date" => "DateTime?",
    _ => "string?"
};
