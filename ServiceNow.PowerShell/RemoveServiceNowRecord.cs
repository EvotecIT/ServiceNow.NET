using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;

namespace ServiceNow.PowerShell;

/// <summary>
/// Deletes a record from a ServiceNow table.
/// </summary>
/// <remarks>
/// <para>Calls the ServiceNow Table API to remove the specified record.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Deletion is permanent and cannot be undone.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Remove an incident.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Remove-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123"</code>
/// <para>Deletes the specified incident after confirmation.</para>
/// </example>
/// <example>
/// <summary>Force removal.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Remove-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -SysId "abc123" -Force</code>
/// <para>Deletes the incident without prompting for confirmation.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Remove, "ServiceNowRecord")]
public class RemoveServiceNowRecord : PSCmdlet {
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
    /// Sys_id of the record to remove.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string SysId { get; set; } = string.Empty;

    /// <summary>
    /// Suppress confirmation prompts when deleting.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        if (!Force && !ShouldContinue($"Delete record {SysId} from {Table}?", "Confirm")) {
            return;
        }

        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var tableClient = provider.GetRequiredService<TableApiClient>();
        tableClient.DeleteRecordAsync(Table, SysId, CancellationToken.None).GetAwaiter().GetResult();
    }
}
