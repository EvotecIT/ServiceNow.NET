using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
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
        var tableArg = new Argument<string>("table", "Table name");
        var outputOpt = new Option<string>("--output", "Output file") { IsRequired = true };

        AddArgument(tableArg);
        AddOption(outputOpt);

        this.SetHandler(async ctx =>
        {
            var table = ctx.ParseResult.GetValueForArgument(tableArg);
            var output = ctx.ParseResult.GetValueForOption(outputOpt)!;
            var baseUrl = ctx.ParseResult.GetValueForOption(baseUrlOption)!;
            var username = ctx.ParseResult.GetValueForOption(usernameOption)!;
            var password = ctx.ParseResult.GetValueForOption(passwordOption)!;
            var userAgent = ctx.ParseResult.GetValueForOption(userAgentOption)!;
            var cancellationToken = ctx.GetCancellationToken();

            var settings = new ServiceNowSettings { BaseUrl = baseUrl, Username = username, Password = password, UserAgent = userAgent };
            using var provider = CommandHelpers.BuildProvider(settings);
            var metaClient = provider.GetRequiredService<TableMetadataClient>();
            var metadata = await metaClient.GetMetadataAsync(table, cancellationToken).ConfigureAwait(false);
            var code = CommandHelpers.GenerateClass(metadata);
            File.WriteAllText(output, code);
            Console.WriteLine($"Model written to {output}");
        });
    }
}
