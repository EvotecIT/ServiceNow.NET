using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;

namespace ServiceNow.PowerShell;

[Cmdlet(VerbsCommon.Remove, "ServiceNowRecord")]
/// <summary>
/// PowerShell cmdlet for removing a ServiceNow record.
/// </summary>
public class RemoveServiceNowRecord : PSCmdlet {
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
