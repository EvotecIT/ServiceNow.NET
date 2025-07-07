using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for creating a ServiceNow record.
/// </summary>
[Cmdlet(VerbsCommon.New, "ServiceNowRecord")]
public class NewServiceNowRecord : PSCmdlet {
    [Parameter(Mandatory = true)]
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Username used for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Password used for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Name of the table in which to create the record.
    /// </summary>
    public string Table { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// JSON payload describing the record to create.
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var tableClient = provider.GetRequiredService<TableApiClient>();
        var payload = JsonSerializer.Deserialize<Dictionary<string, string?>>(Data, ServiceNowJson.Default) ?? new();
        tableClient.CreateRecordAsync(Table, payload, CancellationToken.None).GetAwaiter().GetResult();
    }
}
