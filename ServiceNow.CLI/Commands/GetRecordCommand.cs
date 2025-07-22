using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.CLI.Commands;

internal sealed class GetRecordCommand : Command
{
    public GetRecordCommand(
        Option<string> baseUrlOption,
        Option<string> usernameOption,
        Option<string> passwordOption,
        Option<string> userAgentOption,
        Option<string> apiVersionOption)
        : base("get-record", "Retrieve a record")
    {
        var tableArg = new Argument<string>("table", "Table name");
        var sysIdArg = new Argument<string>("sysId", "Record sys_id");
        var filterOpt = new Option<string[]>("--filter", "Query filters as key=value pairs")
        {
            AllowMultipleArgumentsPerToken = true
        };

        AddArgument(tableArg);
        AddArgument(sysIdArg);
        AddOption(filterOpt);

        this.SetHandler(async ctx =>
        {
            var table = ctx.ParseResult.GetValueForArgument(tableArg);
            var sysId = ctx.ParseResult.GetValueForArgument(sysIdArg);
            var filterPairs = ctx.ParseResult.GetValueForOption(filterOpt) ?? Array.Empty<string>();
            var filters = CommandHelpers.ParseFilters(filterPairs);
            var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
            var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
            var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
            var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
            var apiVersion = ctx.ParseResult.GetValueForOption(apiVersionOption)!;
            var cancellationToken = ctx.GetCancellationToken();

            var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent, ApiVersion = apiVersion };
            using var provider = CommandHelpers.BuildProvider(settings);
            var tableClient = provider.GetRequiredService<TableApiClient>();
            var record = await tableClient.GetRecordAsync<TaskRecord>(table, sysId, filters, cancellationToken).ConfigureAwait(false);
            Console.WriteLine(JsonSerializer.Serialize(record, new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));
        });
    }
}
