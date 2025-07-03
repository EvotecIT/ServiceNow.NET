using System.Net.Http;
using System.Threading;

namespace ServiceNow.Clients;

/// <summary>
/// Defines basic HTTP methods for interacting with the ServiceNow REST API.
/// </summary>
public interface IServiceNowClient {
    Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
