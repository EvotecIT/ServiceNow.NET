using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests;

/// <summary>
/// <see cref="HttpMessageHandler"/> that returns a predefined sequence of responses.
/// </summary>
public class SequenceMessageHandler : HttpMessageHandler {
    /// <summary>
    /// Requests processed by this handler.
    /// </summary>
    public List<HttpRequestMessage> Requests { get; } = new();

    /// <summary>
    /// Captured request bodies.
    /// </summary>
    public List<string?> RequestContents { get; } = new();

    private readonly Queue<HttpResponseMessage> _responses = new();

    /// <summary>
    /// Enqueues a response to be returned for the next request.
    /// </summary>
    public void EnqueueResponse(HttpResponseMessage response) => _responses.Enqueue(response);

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
        Requests.Add(request);
        RequestContents.Add(request.Content is not null ? await request.Content.ReadAsStringAsync() : null);
        return _responses.Dequeue();
    }
}
