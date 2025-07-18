using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Clients;

namespace ServiceNow.Tests;

/// <summary>
/// Mock client that counts GET requests.
/// </summary>
public class CountingServiceNowClient : MockServiceNowClient, IServiceNowClient {
    public int GetCount { get; private set; }

    public new Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default)
        => GetAsyncImpl(relativeUrl, cancellationToken);

    Task<HttpResponseMessage> IServiceNowClient.GetAsync(string relativeUrl, CancellationToken cancellationToken)
        => GetAsyncImpl(relativeUrl, cancellationToken);

    private Task<HttpResponseMessage> GetAsyncImpl(string relativeUrl, CancellationToken cancellationToken) {
        GetCount++;
        return base.GetAsync(relativeUrl, cancellationToken);
    }
}
