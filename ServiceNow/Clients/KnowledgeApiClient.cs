using System.Text.Json;
using System.Collections.Generic;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with ServiceNow knowledge articles.
/// </summary>
public class KnowledgeApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public KnowledgeApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Searches knowledge articles.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> SearchArticlesAsync<T>(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.KnowledgeSearch, _settings.ApiVersion);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Retrieves a single knowledge article.
    /// </summary>
    /// <param name="sysId">Article sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetArticleAsync<T>(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.KnowledgeArticle, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Retrieves article attachments.
    /// </summary>
    /// <param name="sysId">Article sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> GetAttachmentsAsync<T>(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.KnowledgeArticleAttachments, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Retrieves knowledge categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> GetCategoriesAsync<T>(CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.KnowledgeCategories, _settings.ApiVersion);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }
}
