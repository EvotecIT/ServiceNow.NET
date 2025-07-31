using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
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
        var tableArg = new Argument<string>("table", "Table name");
        var sysIdArg = new Argument<string>("sysId", "Record sys_id");
        var dataOpt = new Option<string>("--data", "JSON payload") { IsRequired = true };

        AddArgument(tableArg);
        AddArgument(sysIdArg);
        AddOption(dataOpt);

        this.SetHandler(async ctx =>
        {
            var table = ctx.ParseResult.GetValueForArgument(tableArg);
            var sysId = ctx.ParseResult.GetValueForArgument(sysIdArg);
            var data = ctx.ParseResult.GetValueForOption(dataOpt)!;
            var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
            var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
            var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
            var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
            var apiVersion = ctx.ParseResult.GetValueForOption(apiVersionOption)!;
            var cancellationToken = ctx.GetCancellationToken();

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
                StandardStreamWriter.WriteLine(ctx.Console.Error, $"Invalid JSON payload: {ex.Message}");
                ctx.ExitCode = 1;
                return;
            }

            await tableClient.UpdateRecordAsync(table, sysId, record, cancellationToken).ConfigureAwait(false);
            Console.WriteLine("Record updated.");
        });
    }
}
