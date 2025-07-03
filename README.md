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

## CLI Usage

The command-line tool is in the `ServiceNow.CLI` project. Example:

```bash
# Build and run the CLI
dotnet run --project ServiceNow.CLI -- --base-url https://instance.service-now.com \
    --username admin --password password get-record incident abc123
```

## PowerShell Module Usage

PowerShell cmdlets are located in the `ServiceNow.PowerShell` project. After building the project, import the module and use the cmdlets:

```powershell
# Import the module (path to DLL output from build)
Import-Module ./ServiceNow.PowerShell/bin/Debug/net8.0/ServiceNow.PowerShell.dll

# Retrieve a record
Get-ServiceNowRecord -BaseUrl https://instance.service-now.com -Username admin -Password password -Table incident -SysId abc123
```
