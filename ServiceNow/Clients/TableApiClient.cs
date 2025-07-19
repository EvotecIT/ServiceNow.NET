using System.Text.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="TableApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public TableApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves a record from a table.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetRecordAsync<T>(string table, string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Lists records from a table.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> ListRecordsAsync<T>(string table, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Retrieves a page of records from a table.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="limit">Maximum records to return.</param>
    /// <param name="offset">Record offset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IReadOnlyList<T>> GetRecordsAsync<T>(string table, int limit, int offset, CancellationToken cancellationToken = default) {
        var query = $"?sysparm_limit={limit}&sysparm_offset={offset}";
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new List<T>();
    }

    /// <summary>
    /// Streams records from a table in batches.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="batchSize">Number of records per batch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async IAsyncEnumerable<T> StreamRecordsAsync<T>(string table, int batchSize = 100, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        var offset = 0;
        while (true) {
            var records = await GetRecordsAsync<T>(table, batchSize, offset, cancellationToken).ConfigureAwait(false);
            if (records.Count == 0) {
                yield break;
            }
            foreach (var record in records) {
                yield return record;
            }
            offset += records.Count;
        }
    }

    /// <summary>
    /// Retrieves all records from a table using paging.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="batchSize">Number of records per request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> ListAllRecordsAsync<T>(string table, int batchSize = 100, CancellationToken cancellationToken = default) {
        var list = new List<T>();
        await foreach (var record in StreamRecordsAsync<T>(table, batchSize, cancellationToken)) {
            list.Add(record);
        }
        return list;
    }

    /// <summary>
    /// Creates a record.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="record">Record payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.PostAsync(path, record, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a record.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="record">Record payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.PutAsync(path, record, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a record.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.DeleteAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }
}