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
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
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
