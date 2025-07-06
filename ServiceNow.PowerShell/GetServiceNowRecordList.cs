using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using ServiceNow.Models;
using System.Management.Automation;
using System.Collections.Generic;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for streaming ServiceNow records.
/// </summary>
[Cmdlet(VerbsCommon.Get, "ServiceNowRecordList")]
public class GetServiceNowRecordList : PSCmdlet {
    [Parameter(Mandatory = true)]
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

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

