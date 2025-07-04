using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests;

public class SequenceMessageHandler : HttpMessageHandler {
    public List<HttpRequestMessage> Requests { get; } = new();
    public List<string?> RequestContents { get; } = new();
    private readonly Queue<HttpResponseMessage> _responses = new();

    public void EnqueueResponse(HttpResponseMessage response) => _responses.Enqueue(response);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        Requests.Add(request);
        RequestContents.Add(request.Content is not null ? await request.Content.ReadAsStringAsync() : null);
        return _responses.Dequeue();
    }
}
