using System.CommandLine;
using ServiceNow.CLI.Commands;

var baseUrlOption = new Option<string>("--base-url", description: "ServiceNow instance base URL") { IsRequired = true };
var usernameOption = new Option<string>("--username", description: "Username") { IsRequired = true };
var passwordOption = new Option<string>("--password", description: "Password") { IsRequired = true };
var userAgentOption = new Option<string>("--user-agent", () => "ServiceNow.NET", "User agent") { IsRequired = false };
var apiVersionOption = new Option<string>("--api-version", () => "v2", "API version") { IsRequired = false };

var root = new RootCommand("ServiceNow CLI");
root.AddGlobalOption(baseUrlOption);
root.AddGlobalOption(usernameOption);
root.AddGlobalOption(passwordOption);
root.AddGlobalOption(userAgentOption);
root.AddGlobalOption(apiVersionOption);

root.AddCommand(new GetRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.AddCommand(new CreateRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.AddCommand(new UpdateRecordCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.AddCommand(new ListRecordsCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption, apiVersionOption));
root.AddCommand(new GenerateModelCommand(baseUrlOption, usernameOption, passwordOption, userAgentOption));

return await root.InvokeAsync(args);
