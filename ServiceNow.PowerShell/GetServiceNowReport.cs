using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Collections.Generic;
using System.Text.Json;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for retrieving a ServiceNow report.
/// </summary>
[Cmdlet(VerbsCommon.Get, "ServiceNowReport")]
public class GetServiceNowReport : PSCmdlet {
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
    /// Identifier of the report to retrieve.
    /// </summary>
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
