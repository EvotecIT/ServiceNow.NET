using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Utilities;
using ServiceNow.Extensions;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving application services and service maps.
/// </summary>
public class ApplicationServiceClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationServiceClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public ApplicationServiceClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves an application service record.
    /// </summary>
    /// <param name="sysId">Service sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ApplicationService?> GetServiceAsync(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.TableRecord, _settings.ApiVersion, "cmdb_ci_service", sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ApplicationService>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Lists application services.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<ApplicationService>> ListServicesAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default) {
        var query = filters is { Count: > 0 } ? $"?{filters.ToQueryString()}" : string.Empty;
        var path = string.Format(ServiceNowApiPaths.ApplicationService, _settings.ApiVersion);
        var response = await _client.GetAsync($"{path}{query}", cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<ApplicationService>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Retrieves the service map for an application service.
    /// </summary>
    /// <param name="sysId">Service sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ServiceMap?> GetServiceMapAsync(string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.ServiceMap, _settings.ApiVersion, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<ServiceMap>(json, ServiceNowJson.Default);
    }
}
