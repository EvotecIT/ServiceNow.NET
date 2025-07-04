using System.Text.Json;
using System.Collections.Generic;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Table API.
/// </summary>
public class TableApiClient {
    private readonly IServiceNowClient _client;

    public TableApiClient(IServiceNowClient client) => _client = client;

    public async Task<T?> GetRecordAsync<T>(string table, string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var response = await _client.GetAsync($"/api/now/table/{table}/{sysId}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    public async Task<List<T>> ListRecordsAsync<T>(string table, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var response = await _client.GetAsync($"/api/now/table/{table}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync($"/api/now/table/{table}", record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PutAsync($"/api/now/table/{table}/{sysId}", record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task PatchRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PatchAsync($"/api/now/table/{table}/{sysId}", record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}