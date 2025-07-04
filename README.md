# ServiceNow.NET

## Build Instructions

1. Restore and build all projects:
   ```bash
   dotnet build ServiceNow.NET.sln
   ```
2. Run tests:
   ```bash
   dotnet test ServiceNow.NET.sln
   ```

## Supported Frameworks

This repository targets the following frameworks:
- .NET 8.0
- .NET 9.0 (preview)
- .NET Framework 4.7.2
- .NET Standard 2.0 (library only)

## Models

The library provides simple classes for common tables like `Incident`, `Problem`,
`ChangeRequest`, and `ConfigurationItem`. Each class includes basic properties such as
`SysId` and `Number` (or `Name`).

## Configuration

`ServiceNowSettings` requires `BaseUrl`, `Username` and `Password` to be provided when creating a `ServiceNowClient`.
An `ArgumentException` is thrown if `BaseUrl` is missing.

### Using Dependency Injection

You can register the clients with `IServiceCollection`:

```csharp
var services = new ServiceCollection();
services.AddServiceNow(new ServiceNowSettings {
    BaseUrl = "https://instance.service-now.com",
    Username = "admin",
    Password = "password"
});
var provider = services.BuildServiceProvider();
var tableClient = provider.GetRequiredService<TableApiClient>();
var problem = await tableClient.GetRecordAsync<Problem>("problem", "abc123", null, CancellationToken.None);
var recent = await tableClient.GetRecordsAsync<TaskRecord>("task", 10, 0, CancellationToken.None);
```

## CLI Usage

The command-line tool is in the `ServiceNow.CLI` project. Example:

```bash
# Build and run the CLI
dotnet run --project ServiceNow.CLI -- --base-url https://instance.service-now.com \
    --username admin --password password get-record incident abc123
```

To generate a model class from table metadata:

```bash
dotnet run --project ServiceNow.CLI -- --base-url https://instance.service-now.com \
    --username admin --password password generate-model incident Incident.cs
```

The CLI builds a service provider and registers typed clients via `AddHttpClient`.
Failed requests throw `ServiceNowException` containing the status code and body.

## PowerShell Module Usage

PowerShell cmdlets are located in the `ServiceNow.PowerShell` project. After building the project, import the module and use the cmdlets:

```powershell
# Import the module (path to DLL output from build)
Import-Module ./ServiceNow.PowerShell/bin/Debug/net8.0/ServiceNow.PowerShell.dll

# Retrieve a record
Get-ServiceNowRecord -BaseUrl https://instance.service-now.com -Username admin -Password password -Table incident -SysId abc123
```

Like the CLI, the PowerShell module uses dependency injection with `AddHttpClient` and throws `ServiceNowException` on failure.
