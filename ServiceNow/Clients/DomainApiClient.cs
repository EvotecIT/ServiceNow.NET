using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for operations on the sys_domain table.
/// </summary>
public class DomainApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public DomainApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    private TableApiClient TableClient => new(_client, _settings);

    /// <summary>
    /// Lists sys_domain records.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<List<SysDomain>> ListDomainsAsync(
        Dictionary<string, string?>? filters = null,
        CancellationToken cancellationToken = default)
        => TableClient.ListRecordsAsync<SysDomain>("sys_domain", filters, cancellationToken);

    /// <summary>
    /// Updates a sys_domain record.
    /// </summary>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="domain">Domain payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task UpdateDomainAsync(
        string sysId,
        SysDomain domain,
        CancellationToken cancellationToken = default)
        => TableClient.UpdateRecordAsync("sys_domain", sysId, domain, cancellationToken);
}
