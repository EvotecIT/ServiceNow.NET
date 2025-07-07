using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Tests;

/// <summary>
/// <see cref="HttpMessageHandler"/> that always returns a canceled task.
/// </summary>
public class CancelMessageHandler : HttpMessageHandler
{
    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromCanceled<HttpResponseMessage>(cancellationToken);
}
