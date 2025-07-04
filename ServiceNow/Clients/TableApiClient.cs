using System.Text.Json;
using System.Collections.Generic;
using ServiceNow.Extensions;
using ServiceNow.Utilities;
using ServiceNow.Configuration;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Table API.
/// </summary>
public class TableApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    public TableApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    public async Task<T?> GetRecordAsync<T>(string table, string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var response = await _client.GetAsync($"/api/now/{_settings.ApiVersion}/table/{table}/{sysId}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    public async Task<List<T>> ListRecordsAsync<T>(string table, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var response = await _client.GetAsync($"/api/now/{_settings.ApiVersion}/table/{table}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync($"/api/now/{_settings.ApiVersion}/table/{table}", record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PutAsync($"/api/now/{_settings.ApiVersion}/table/{table}/{sysId}", record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/{_settings.ApiVersion}/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}