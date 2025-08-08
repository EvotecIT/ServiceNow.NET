using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;

namespace ServiceNow.PowerShell;

/// <summary>
/// Retrieves a single ServiceNow record by its identifier.
/// </summary>
/// <remarks>
/// <para>Fetches the record from the specified table using the ServiceNow Table API.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Returned objects may contain null values for fields not populated in ServiceNow.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Get an incident record.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123"</code>
/// <para>Retrieves the incident with the specified sys_id.</para>
/// </example>
/// <example>
/// <summary>Get a change request.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "change_request" -SysId "def456"</code>
/// <para>Outputs the change request record for inspection.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "ServiceNowRecord")]
public class GetServiceNowRecord : PSCmdlet {
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Username used for authentication.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password used for authentication.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Name of the table containing the record.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// Sys_id of the record to retrieve.
    /// </summary>
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
