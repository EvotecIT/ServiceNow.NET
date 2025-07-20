using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving resource plan records.
/// </summary>
public class ResourcePlanClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourcePlanClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public ResourcePlanClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves a resource plan record.
    /// </summary>
    /// <param name="sysId">Plan sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ResourcePlan?> GetResourcePlanAsync(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.ResourcePlan, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ResourcePlan>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Lists resource plan records.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<ResourcePlan>> ListResourcePlansAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.ResourcePlans, _settings.ApiVersion);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ResourcePlan>>(json, ServiceNowJson.Default) ?? new();
    }
}
