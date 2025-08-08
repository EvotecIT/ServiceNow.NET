using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using ServiceNow.Models;
using System.Management.Automation;
using System.Collections.Generic;

namespace ServiceNow.PowerShell;

/// <summary>
/// Streams records from a ServiceNow table.
/// </summary>
/// <remarks>
/// <para>Retrieves records in batches using the ServiceNow Table API.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Multiple requests are made until all records are returned.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Stream incident records.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowRecordList -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "incident"</code>
/// <para>The cmdlet writes each incident record to the pipeline.</para>
/// </example>
/// <example>
/// <summary>Retrieve configuration items with custom batch size.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowRecordList -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Table "cmdb_ci" -BatchSize 50</code>
/// <para>Streams configuration item records fifty at a time.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "ServiceNowRecordList")]
public class GetServiceNowRecordList : PSCmdlet {
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
    /// Name of the table to query.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// Number of records to retrieve per batch.
    /// </summary>
    [Parameter]
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var tableClient = provider.GetRequiredService<TableApiClient>();
        var enumerator = tableClient.StreamRecordsAsync<TaskRecord>(Table, BatchSize, CancellationToken.None).GetAsyncEnumerator();
        try {
            while (enumerator.MoveNextAsync().GetAwaiter().GetResult()) {
                WriteObject(enumerator.Current, true);
            }
        } finally {
            enumerator.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
