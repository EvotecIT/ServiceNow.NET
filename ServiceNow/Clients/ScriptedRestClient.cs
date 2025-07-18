using System.Net.Http;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for invoking custom Scripted REST API endpoints.
/// </summary>
public class ScriptedRestClient {
    private readonly IServiceNowClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptedRestClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    public ScriptedRestClient(IServiceNowClient client)
        => _client = client;

    /// <summary>
    /// Sends a GET request to the specified path and returns the deserialized result.
    /// </summary>
    /// <param name="path">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken = default) {
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Sends a POST request with a payload and returns the deserialized result.
    /// </summary>
    /// <param name="path">Relative request URL.</param>
    /// <param name="payload">Request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> PostAsync<T>(string path, object payload, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync(path, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Sends a PUT request with a payload and returns the deserialized result.
    /// </summary>
    /// <param name="path">Relative request URL.</param>
    /// <param name="payload">Request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> PutAsync<T>(string path, object payload, CancellationToken cancellationToken = default) {
        var response = await _client.PutAsync(path, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }

    /// <summary>
    /// Sends a DELETE request to the specified path.
    /// </summary>
    /// <param name="path">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync(path, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}
