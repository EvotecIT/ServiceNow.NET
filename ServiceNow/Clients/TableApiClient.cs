using System.Net.Http;
using System.Text.Json;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Table API.
/// </summary>
/// <summary>
/// Provides CRUD operations for ServiceNow tables.
/// </summary>
public class TableApiClient {
    private readonly ServiceNowClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableApiClient"/> class.
    /// </summary>
    /// <param name="client">The underlying ServiceNow client.</param>
    public TableApiClient(ServiceNowClient client) => _client = client;

    /// <summary>
    /// Retrieves a record from the specified table.
    /// </summary>
    /// <typeparam name="T">Type of the record to deserialize.</typeparam>
    /// <param name="table">The table name.</param>
    /// <param name="sysId">The sys_id of the record.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<T?> GetRecordAsync<T>(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.GetAsync($"/api/now/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Creates a record in the specified table.
    /// </summary>
    /// <typeparam name="T">Type of the record payload.</typeparam>
    /// <param name="table">The table name.</param>
    /// <param name="record">The record payload.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync($"/api/now/table/{table}", record, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Updates a record in the specified table.
    /// </summary>
    /// <typeparam name="T">Type of the record payload.</typeparam>
    /// <param name="table">The table name.</param>
    /// <param name="sysId">The sys_id of the record.</param>
    /// <param name="record">The record payload.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var response = await _client.PutAsync($"/api/now/table/{table}/{sysId}", record, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes a record from the specified table.
    /// </summary>
    /// <param name="table">The table name.</param>
    /// <param name="sysId">The sys_id of the record.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/table/{table}/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}