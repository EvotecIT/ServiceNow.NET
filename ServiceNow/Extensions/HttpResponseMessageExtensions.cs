using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions {
    /// <summary>
    /// Throws an appropriate <see cref="ServiceNowException"/> if the response indicates failure.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    public static async Task EnsureServiceNowSuccessAsync(this HttpResponseMessage response) {
        if (response.IsSuccessStatusCode) {
            return;
        }

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        throw ServiceNowExceptionFactory.Create(response.StatusCode, content);
    }
}
