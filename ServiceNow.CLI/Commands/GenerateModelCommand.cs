using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ServiceNow.CLI.Commands;

internal sealed class GenerateModelCommand : Command
{
    public GenerateModelCommand(
        Option<string> baseUrlOption,
        Option<string> usernameOption,
        Option<string> passwordOption,
        Option<string> userAgentOption)
        : base("generate-model", "Generate C# model for a table")
    {
        var tableArg = new Argument<string>("table") { Description = "Table name" };
        var outputOpt = new Option<string>("--output") { Description = "Output file", Required = true };

        Arguments.Add(tableArg);
        Options.Add(outputOpt);

        SetAction(async (parseResult, cancellationToken) =>
        {
            var table = parseResult.GetRequiredValue(tableArg);
            var output = parseResult.GetRequiredValue(outputOpt);
            var baseUrl = parseResult.GetRequiredValue(baseUrlOption);
            var username = parseResult.GetRequiredValue(usernameOption);
            var password = parseResult.GetRequiredValue(passwordOption);
            var userAgent = parseResult.GetRequiredValue(userAgentOption);

            var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
            using var provider = CommandHelpers.BuildProvider(settings);
            var metaClient = provider.GetRequiredService<TableMetadataClient>();
            var metadata = await metaClient.GetMetadataAsync(table, cancellationToken).ConfigureAwait(false);
            var code = CommandHelpers.GenerateClass(metadata);
            File.WriteAllText(output, code);
            Console.WriteLine($"Model written to {output}");
            return 0;
        });
    }
}
