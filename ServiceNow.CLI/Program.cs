using System.CommandLine;
using ServiceNow.CLI.Commands;

var baseUrlOption = new Option<string>("--base-url") { Description = "ServiceNow instance base URL", Required = true, Recursive = true };
var usernameOption = new Option<string>("--username") { Description = "Username", Required = true, Recursive = true };
var passwordOption = new Option<string>("--password") { Description = "Password", Required = true, Recursive = true };
var userAgentOption = new Option<string>("--user-agent") { Description = "User agent", DefaultValueFactory = _ => "ServiceNow.NET", Recursive = true };
var apiVersionOption = new Option<string>("--api-version") { Description = "API version", DefaultValueFactory = _ => "v2", Recursive = true };

var root = new RootCommand("ServiceNow CLI");
root.Options.Add(baseUrlOption);
root.Options.Add(usernameOption);
root.Options.Add(passwordOption);
root.Options.Add(userAgentOption);
root.Options.Add(apiVersionOption);

root.Subcommands.Add(new GetRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.Subcommands.Add(new CreateRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.Subcommands.Add(new UpdateRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.Subcommands.Add(new ListRecordsCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.Subcommands.Add(new GenerateModelCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption));

return await root.Parse(args).InvokeAsync();
