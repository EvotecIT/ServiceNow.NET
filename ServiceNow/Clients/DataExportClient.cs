using System.Net.Http;
using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Utilities;
using ServiceNow.Extensions;

namespace ServiceNow.Clients;

/// <summary>
/// Client for starting and downloading data exports.
/// </summary>
public class DataExportClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataExportClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public DataExportClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Starts an export job and returns the export identifier.
    /// </summary>
    /// <param name="payload">Export parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<string> StartExportAsync(object payload, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.DataExportStart, _settings.ApiVersion);
        var response = await _client.PostAsync(path, payload, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("result", out var result) &&
            result.TryGetProperty("export_id", out var id)) {
            return id.GetString() ?? string.Empty;
        }
        return string.Empty;
    }

    /// <summary>
    /// Downloads the export file for the given identifier.
    /// </summary>
    /// <param name="exportId">Export job identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> DownloadExportAsync(string exportId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.DataExportFile, _settings.ApiVersion, exportId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        return response;
    }
}