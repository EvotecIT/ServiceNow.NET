using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Management.Automation;

namespace ServiceNow.PowerShell;

/// <summary>
/// PowerShell cmdlet for searching knowledge articles.
/// </summary>
[Cmdlet(VerbsCommon.Find, "ServiceNowKnowledge")]
public class FindServiceNowKnowledge : PSCmdlet {
    [Parameter(Mandatory = true)]
    public string BaseUrl { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Username { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Password { get; set; } = string.Empty;

    [Parameter(Mandatory = true)]
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Executes the cmdlet.
    /// </summary>
    protected override void ProcessRecord() {
        var settings = new ServiceNowSettings { BaseUrl = BaseUrl, Username = Username, Password = Password };
        var services = new ServiceCollection();
        services.AddServiceNow(settings);
        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SearchApiClient>();
        var results = client.SearchKnowledgeAsync<KnowledgeSearchResult>(Query, CancellationToken.None).GetAwaiter().GetResult();
        WriteObject(results, true);
    }
}

