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

    public TableApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    public async Task<T?> GetRecordAsync<T>(string table, string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    public async Task<List<T>> ListRecordsAsync<T>(string table, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    public async Task<IReadOnlyList<T>> GetRecordsAsync<T>(string table, int limit, int offset, CancellationToken cancellationToken = default) {
        var query = $"?sysparm_limit={limit}&sysparm_offset={offset}";
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new List<T>();
    }

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

    public async Task CreateRecordAsync<T>(string table, T record, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.Table, _settings.ApiVersion, table);
        var response = await _client.PostAsync(path, record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task UpdateRecordAsync<T>(string table, string sysId, T record, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.PutAsync(path, record, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }

    public async Task DeleteRecordAsync(string table, string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, table, sysId);
        var response = await _client.DeleteAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}