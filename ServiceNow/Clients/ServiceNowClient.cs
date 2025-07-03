using ServiceNow.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ServiceNow.Clients;

/// <summary>
/// Basic client for interacting with the ServiceNow REST API.
/// </summary>
/// <summary>
/// Provides basic HTTP operations for interacting with the ServiceNow REST API.
/// </summary>
public class ServiceNowClient {
    private readonly HttpClient _httpClient;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying <see cref="HttpClient"/> used for requests.</param>
    /// <param name="settings">Configuration settings for the ServiceNow instance.</param>
    public ServiceNowClient(HttpClient httpClient, ServiceNowSettings settings) {
        _httpClient = httpClient;
        _settings = settings;

        _httpClient.BaseAddress = new Uri(settings.BaseUrl);
        var authBytes = Encoding.ASCII.GetBytes($"{settings.Username}:{settings.Password}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
    }

    /// <summary>
    /// Sends an HTTP GET request.
    /// </summary>
    /// <param name="relativeUrl">The relative URL to request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default)
        => await _httpClient.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends an HTTP POST request with a JSON payload.
    /// </summary>
    /// <typeparam name="T">Type of the payload.</typeparam>
    /// <param name="relativeUrl">The relative URL to request.</param>
    /// <param name="payload">The payload to serialize to JSON.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP PUT request with a JSON payload.
    /// </summary>
    /// <typeparam name="T">Type of the payload.</typeparam>
    /// <param name="relativeUrl">The relative URL to request.</param>
    /// <param name="payload">The payload to serialize to JSON.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP DELETE request.
    /// </summary>
    /// <param name="relativeUrl">The relative URL to request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
        => await _httpClient.DeleteAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
}