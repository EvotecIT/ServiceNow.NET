using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ServiceNow.Utilities;
using System.Collections.Generic;

namespace ServiceNow.CLI.Commands;

internal sealed class UpdateRecordCommand : Command
{
    public UpdateRecordCommand(
        Option<string> baseUrlOption,
        Option<string> usernameOption,
        Option<string> passwordOption,
        Option<string> userAgentOption,
        Option<string> apiVersionOption)
        : base("update-record", "Update a record")
    {
        var tableArg = new Argument<string>("table") { Description = "Table name" };
        var sysIdArg = new Argument<string>("sysId") { Description = "Record sys_id" };
        var dataOpt = new Option<string>("--data") { Description = "JSON payload", Required = true };

        Arguments.Add(tableArg);
        Arguments.Add(sysIdArg);
        Options.Add(dataOpt);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var table = parseResult.GetRequiredValue(tableArg);
            var sysId = parseResult.GetRequiredValue(sysIdArg);
            var data = parseResult.GetRequiredValue(dataOpt);
            var baseUrl = parseResult.GetRequiredValue(baseUrlOption);
            var username = parseResult.GetRequiredValue(usernameOption);
            var password = parseResult.GetRequiredValue(passwordOption);
            var userAgent = parseResult.GetRequiredValue(userAgentOption);
            var apiVersion = parseResult.GetRequiredValue(apiVersionOption);

            var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent, ApiVersion = apiVersion };
            using var provider = CommandHelpers.BuildProvider(settings);
            var tableClient = provider.GetRequiredService<TableApiClient>();

            Dictionary<string, string?> record;
            try
            {
                record = JsonSerializer.Deserialize<Dictionary<string, string?>>(data, ServiceNowJson.Default) ?? new();
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Invalid JSON payload: {ex.Message}");
                return 1;
            }

            await tableClient.UpdateRecordAsync(table, sysId, record, cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Record updated.");
            return 0;
        });
    }
}
