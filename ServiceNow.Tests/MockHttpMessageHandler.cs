using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests;

public class MockHttpMessageHandler : HttpMessageHandler {
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? RequestContent { get; private set; }
    public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK) {
        Content = new StringContent("{}")
    };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        LastRequest = request;
        RequestContent = request.Content is not null
            ? await request.Content.ReadAsStringAsync()
            : null;
        return Response;
    }
}
