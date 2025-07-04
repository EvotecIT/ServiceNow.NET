using ServiceNow.Clients;
using ServiceNow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;
using System.Text.Json;
using ServiceNow.Utilities;

#if NET8_0_OR_GREATER

namespace ServiceNow.PowerShell;

[Cmdlet(VerbsData.Update, "ServiceNowRecord")]
public class UpdateServiceNowRecord : PSCmdlet {
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

    [Parameter(Mandatory = true)]
    public string Data { get; set; } = string.Empty;

    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var tableClient = provider.GetRequiredService<TableApiClient>();
        var payload = JsonSerializer.Deserialize<Dictionary<string, string?>>(Data, ServiceNowJson.Default) ?? new();
        tableClient.PatchRecordAsync(Table, SysId, payload, CancellationToken.None).GetAwaiter().GetResult();
    }
}
#endif
