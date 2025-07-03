using System.Net.Http;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Table API.
/// </summary>
public class TableApiClient {
    private readonly ServiceNowClient _client;

    public TableApiClient(ServiceNowClient client) => _client = client;

    public async Task<T?> GetRecordAsync<T>(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.GetAsync($"/api/now/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync($"/api/now/table/{table}", record, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PutAsync($"/api/now/table/{table}/{sysId}", record, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}