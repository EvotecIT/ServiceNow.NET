using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Clients;

namespace ServiceNow.Tests;

/// <summary>
/// Simple mock implementation of <see cref="IServiceNowClient"/> for unit tests.
/// </summary>
public class MockServiceNowClient : IServiceNowClient {
    public HttpMethod? LastMethod { get; private set; }
    public string? LastRelativeUrl { get; private set; }
    public object? LastPayload { get; private set; }
    public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK) { Content = new StringContent("{}") };

    public Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        LastMethod = HttpMethod.Get;
        LastRelativeUrl = relativeUrl;
        return Task.FromResult(Response);
    }

    public Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        LastMethod = HttpMethod.Post;
        LastRelativeUrl = relativeUrl;
        LastPayload = payload;
        return Task.FromResult(Response);
    }

    public Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        LastMethod = HttpMethod.Put;
        LastRelativeUrl = relativeUrl;
        LastPayload = payload;
        return Task.FromResult(Response);
    }

    public Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        LastMethod = HttpMethod.Delete;
        LastRelativeUrl = relativeUrl;
        return Task.FromResult(Response);
    }
}
