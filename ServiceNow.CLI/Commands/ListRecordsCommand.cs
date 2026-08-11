using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.CLI.Commands;

internal sealed class ListRecordsCommand : Command
{
    public ListRecordsCommand(
        Option<string> baseUrlOption,
        Option<string> usernameOption,
        Option<string> passwordOption,
        Option<string> userAgentOption,
        Option<string> apiVersionOption)
        : base("list-records", "List records")
    {
        var tableArg = new Argument<string>("table") { Description = "Table name" };
        var filterOpt = new Option<string[]>("--filter")
        {
            Description = "Query filters as key=value pairs",
            AllowMultipleArgumentsPerToken = true
        };

        Arguments.Add(tableArg);
        Options.Add(filterOpt);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var table = parseResult.GetRequiredValue(tableArg);
            var filterPairs = parseResult.GetValue(filterOpt) ?? Array.Empty<string>();
            var options = CommandHelpers.ParseQueryOptions(filterPairs);
            var baseUrl = parseResult.GetRequiredValue(baseUrlOption);
            var username = parseResult.GetRequiredValue(usernameOption);
            var password = parseResult.GetRequiredValue(passwordOption);
            var userAgent = parseResult.GetRequiredValue(userAgentOption);
            var apiVersion = parseResult.GetRequiredValue(apiVersionOption);

            var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent, ApiVersion = apiVersion };
            using var provider = CommandHelpers.BuildProvider(settings);
            var tableClient = provider.GetRequiredService<TableApiClient>();
            var records = await tableClient.ListRecordsAsync<TaskRecord>(table, options, cancellationToken).ConfigureAwait(false);
            Console.WriteLine(JsonSerializer.Serialize(records, new JsonSerializerOptions(ServiceNowJson.Default) { WriteIndented = true }));
            return 0;
        });
    }
}
