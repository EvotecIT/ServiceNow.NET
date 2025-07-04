using System.Net.Http;
using System.Threading;

namespace ServiceNow.Clients;

/// <summary>
/// Defines basic HTTP methods for interacting with the ServiceNow REST API.
/// </summary>
public interface IServiceNowClient {
    /// <summary>
    /// Sends a GET request.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request with a payload.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="payload">Request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PUT request with a payload.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="payload">Request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
