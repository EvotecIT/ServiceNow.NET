using System.Text.Json;
using System.Collections.Generic;
using ServiceNow.Configuration;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with ServiceNow catalog requests.
/// </summary>
public class CatalogRequestClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogRequestClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public CatalogRequestClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves a catalog request by sys_id.
    /// </summary>
    /// <param name="requestId">Request sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetRequestAsync<T>(string requestId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.CatalogRequest, _settings.ApiVersion, requestId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Retrieves approvals for a catalog request.
    /// </summary>
    /// <param name="requestId">Request sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> GetApprovalsAsync<T>(string requestId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.CatalogRequestApprovals, _settings.ApiVersion, requestId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }
}
