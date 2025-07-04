using System.Text.Json;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving table metadata from ServiceNow.
/// </summary>
public class TableMetadataClient {
    private readonly IServiceNowClient _client;

    public TableMetadataClient(IServiceNowClient client) => _client = client;

    /// <summary>
    /// Gets metadata for the specified table.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<TableMetadata> GetMetadataAsync(string table, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableMetadata, table);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
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
        return new TableMetadata(table, list);
    }
}
