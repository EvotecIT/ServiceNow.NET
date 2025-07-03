using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Management.Automation;
using System.Text.Json;
using ServiceNow.Utilities;

#if NET8_0_OR_GREATER

namespace ServiceNow.PowerShell;

[Cmdlet(VerbsCommon.New, "ServiceNowRecord")]
public class NewServiceNowRecord : PSCmdlet {
    [Parameter(Mandatory = true)]
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Table { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Data { get; set; } = string.Empty;

    protected override void ProcessRecord() {
        using var http = new HttpClient();
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var client = new ServiceNowClient(http, settings);
        var tableClient = new TableApiClient(client);
        var payload = JsonSerializer.Deserialize<Dictionary<string, string?>>(Data, ServiceNowJson.Default) ?? new();
        tableClient.CreateRecordAsync(Table, payload, CancellationToken.None).GetAwaiter().GetResult();
    }
}
#endif