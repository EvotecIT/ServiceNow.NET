using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests;

/// <summary>
/// Test <see cref="HttpMessageHandler"/> that records requests and returns a preconfigured response.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler {
    /// <summary>
    /// The last request processed by the handler.
    /// </summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    /// <summary>
    /// Body content of the last request.
    /// </summary>
    public string? RequestContent { get; private set; }

    /// <summary>
    /// Response returned for all requests.
    /// </summary>
    public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK) {
        Content = new StringContent("{}")
    };

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        LastRequest = request;
        RequestContent = request.Content is not null
            ? await request.Content.ReadAsStringAsync()
            : null;
        return Response;
    }
}