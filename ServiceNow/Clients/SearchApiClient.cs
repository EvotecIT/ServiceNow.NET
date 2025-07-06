using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for the ServiceNow search API.
/// </summary>
public class SearchApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public SearchApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Searches knowledge articles.
    /// </summary>
    /// <param name="query">Search text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> SearchKnowledgeAsync<T>(string query, CancellationToken cancellationToken = default) {
        var term = Uri.EscapeDataString(query ?? string.Empty);
        var path = string.Format(ServiceNowApiPaths.Search, _settings.ApiVersion);
        var url = $"{path}?sysparm_search={term}&sysparm_target=kb_knowledge";
        var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind == JsonValueKind.Array) {
            return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
        }
        if (doc.RootElement.TryGetProperty("result", out var result) && result.TryGetProperty("search_results", out var arr)) {
            return JsonSerializer.Deserialize<List<T>>(arr.GetRawText(), ServiceNowJson.Default) ?? new();
        }
        return new();
    }

    /// <summary>
    /// Searches catalog items.
    /// </summary>
    /// <param name="query">Search text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> SearchCatalogAsync<T>(string query, CancellationToken cancellationToken = default) {
        var term = Uri.EscapeDataString(query ?? string.Empty);
        var path = string.Format(ServiceNowApiPaths.Search, _settings.ApiVersion);
        var url = $"{path}?sysparm_search={term}&sysparm_target=sc_cat_item";
        var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind == JsonValueKind.Array) {
            return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
        }
        if (doc.RootElement.TryGetProperty("result", out var result) && result.TryGetProperty("search_results", out var arr)) {
            return JsonSerializer.Deserialize<List<T>>(arr.GetRawText(), ServiceNowJson.Default) ?? new();
        }
        return new();
    }
}
