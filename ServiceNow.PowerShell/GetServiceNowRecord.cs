using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for retrieving a single ServiceNow record.
/// </summary>
[Cmdlet(VerbsCommon.Get, "ServiceNowRecord")]
public class GetServiceNowRecord : PSCmdlet {
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
    /// Name of the table containing the record.
    /// </summary>
    public string Table { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    /// <summary>
    /// Sys_id of the record to retrieve.
    /// </summary>
    public string SysId { get; set; } = string.Empty;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var tableClient = provider.GetRequiredService<TableApiClient>();
        var record = tableClient.GetRecordAsync<TaskRecord>(Table, SysId, null, CancellationToken.None).GetAwaiter().GetResult();
        WriteObject(record);
    }
}
