using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Collections.Generic;
using System.Text.Json;

namespace ServiceNow.PowerShell;

/// <summary>
/// Retrieves a ServiceNow report.
/// </summary>
/// <remarks>
/// <para>Calls the ServiceNow Report API and returns the report data.</para>
/// <list type="alertSet">
/// <item>
/// <term>Note</term>
/// <description>Large reports may consume significant memory when loaded.</description>
/// </item>
/// </list>
/// </remarks>
/// <example>
/// <summary>Retrieve an incident metrics report.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowReport -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Report "incident_metrics"</code>
/// <para>Outputs the data for the specified report.</para>
/// </example>
/// <example>
/// <summary>Retrieve an assets summary.</summary>
/// <prefix>PS&gt; </prefix>
/// <code>Get-ServiceNowReport -BaseUrl "https://instance.service-now.com" -Username "user" -Password "pass" -Report "asset_summary"</code>
/// <para>Gets a summary of assets defined in the instance.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/">PowerShell Documentation</seealso>
/// <seealso href="https://github.com/ServiceNowNET/ServiceNow.NET">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "ServiceNowReport")]
public class GetServiceNowReport : PSCmdlet {
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
    /// Identifier of the report to retrieve.
    /// </summary>
    [Parameter(Mandatory = true)]
    public string Report { get; set; } = string.Empty;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var reportClient = provider.GetRequiredService<ReportApiClient>();
        var result = reportClient.GetReportAsync<Dictionary<string, object>>(Report, null, CancellationToken.None).GetAwaiter().GetResult();
        WriteObject(result);
    }
}
