using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving and creating GRC items.
/// </summary>
public class GrcClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="GrcClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public GrcClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves a GRC item record.
    /// </summary>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<GrcItem?> GetItemAsync(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.GrcItem, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<GrcItem>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Creates a GRC item record.
    /// </summary>
    /// <param name="item">Item payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CreateItemAsync(GrcItem item, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.GrcItems, _settings.ApiVersion);
        var response = await _client.PostAsync(path, item, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}
