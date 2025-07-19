using System.Text.Json;
using System.Collections.Generic;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with ServiceNow catalog items.
/// </summary>
public class CatalogItemClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogItemClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public CatalogItemClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves a list of catalog items.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> GetItemsAsync<T>(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.CatalogItems, _settings.ApiVersion);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Retrieves a single catalog item by sys_id.
    /// </summary>
    /// <param name="sysId">Item sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetItemAsync<T>(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.CatalogItem, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Submits a request for a catalog item.
    /// </summary>
    /// <param name="sysId">Item sys_id.</param>
    /// <param name="payload">Request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<string> OrderItemAsync<T>(string sysId, T payload, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.CatalogItemOrder, _settings.ApiVersion, sysId);
        var response = await _client.PostAsync(path, payload, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("result", out var result)) {
            if (result.TryGetProperty("request_number", out var num)) {
                return num.GetString() ?? string.Empty;
            }
            if (result.TryGetProperty("request_id", out var id)) {
                return id.GetString() ?? string.Empty;
            }
        }
        return string.Empty;
    }
}
