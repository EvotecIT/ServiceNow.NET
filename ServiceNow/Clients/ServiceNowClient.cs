using ServiceNow.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ServiceNow.Clients;

/// <summary>
/// Basic client for interacting with the ServiceNow REST API.
/// </summary>
public class ServiceNowClient {
    private readonly HttpClient _httpClient;
    private readonly ServiceNowSettings _settings;

    public ServiceNowClient(HttpClient httpClient, ServiceNowSettings settings) {
        _httpClient = httpClient;
        _settings = settings;

        _httpClient.BaseAddress = new Uri(settings.BaseUrl);
        var authBytes = Encoding.ASCII.GetBytes($"{settings.Username}:{settings.Password}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
    }

    public async Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default)
        => await _httpClient.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);

    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
        => await _httpClient.DeleteAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
}