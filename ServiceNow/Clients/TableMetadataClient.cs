using System.Collections.Concurrent;
using System.Text.Json;
using ServiceNow.Models;
using ServiceNow.Configuration;
using ServiceNow.Extensions;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving table metadata from ServiceNow.
/// </summary>
public class TableMetadataClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;
    private readonly ConcurrentDictionary<string, (TableMetadata Metadata, DateTimeOffset Expires)> _cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableMetadataClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Configuration settings.</param>
    public TableMetadataClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Gets metadata for the specified table.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<TableMetadata> GetMetadataAsync(string table, CancellationToken cancellationToken = default) {
        if (_cache.TryGetValue(table, out var entry) && entry.Expires > DateTimeOffset.UtcNow) {
            return entry.Metadata;
        }

        var path = string.Format(ServiceNowApiPaths.TableMetadata, table);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var doc = JsonDocument.Parse(json);
        var list = new List<TableField>();
        if (doc.RootElement.TryGetProperty("result", out var result)) {
            foreach (var element in result.EnumerateArray()) {
                if (element.TryGetProperty("element", out var nameProp) &&
                    element.TryGetProperty("internal_type", out var typeProp)) {
                    var name = nameProp.GetString() ?? string.Empty;
                    var type = typeProp.GetString() ?? "string";
                    if (!string.IsNullOrEmpty(name)) {
                        list.Add(new TableField(name, type));
                    }
                }
            }
        }
        var meta = new TableMetadata(table, list);
        var expires = DateTimeOffset.UtcNow.Add(_settings.MetadataCacheDuration);
        _cache[table] = (meta, expires);
        return meta;
    }
}
