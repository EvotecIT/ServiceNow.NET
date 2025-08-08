using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.PowerShell;

/// <summary>
/// Creates a new record in a ServiceNow table.
/// </summary>
/// <remarks>
/// <para>Sends a JSON payload to the ServiceNow Table API to create the record.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Creating records can trigger business rules or workflows on the ServiceNow instance.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Create an incident.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>New-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident" -Data '{"short_description":"Test"}'</code>
/// <para>Creates an incident with a short description.</para>
/// </example>
/// <example>
/// <summary>Create a problem record.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>New-ServiceNowRecord -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "problem" -Data '{"short_description":"Example"}'</code>
/// <para>Submits a problem record to the ServiceNow instance.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.New, "ServiceNowRecord")]
public class NewServiceNowRecord : PSCmdlet {
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
    /// Name of the table in which to create the record.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// JSON payload describing the record to create.
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
        tableClient.CreateRecordAsync(Table, payload, CancellationToken.None).GetAwaiter().GetResult();
    }
}
