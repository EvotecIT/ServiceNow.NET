using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.PowerShell;

/// <summary>
/// Updates an existing ServiceNow record.
/// </summary>
/// <remarks>
/// <para>Applies the provided JSON payload to the record through the ServiceNow Table API.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Updating records may trigger workflows or business rules on the server.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Update an incident description.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Set-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123" -Data '{"short_description":"Updated"}'</code>
/// <para>Changes the short description of the incident.</para>
/// </example>
/// <example>
/// <summary>Adjust change request priority.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Set-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "change_request" -SysId "def456" -Data '{"priority":"2"}'</code>
/// <para>Sets the priority field on the change request.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Set, "ServiceNowRecord")]
public class SetServiceNowRecord : PSCmdlet {
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
    /// Sys_id of the record to update.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string SysId { get; set; } = string.Empty;

    /// <summary>
    /// JSON payload describing the updates to apply.
    /// </summary>
    [Parameter(Mandatory = true)]
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
        tableClient.UpdateRecordAsync(Table, SysId, payload, CancellationToken.None).GetAwaiter().GetResult();
    }
}
