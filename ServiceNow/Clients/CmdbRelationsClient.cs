using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving CMDB relationships.
/// </summary>
public class CmdbRelationsClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmdbRelationsClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public CmdbRelationsClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Lists relationships for a configuration item.
    /// </summary>
    /// <param name="table">Configuration item table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> ListRelationshipsAsync<T>(string table, string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.CmdbRelationships, _settings.ApiVersion, table, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }
}
