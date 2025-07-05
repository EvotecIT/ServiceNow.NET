using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for posting events to ServiceNow Event Management.
/// </summary>
public class EventApiClient {
    private readonly IServiceNowClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    public EventApiClient(IServiceNowClient client) => _client = client;

    /// <summary>
    /// Sends an event payload to ServiceNow.
    /// </summary>
    /// <param name="payload">Event parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendEventAsync<T>(T payload, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync(ServiceNowApiPaths.EmEvent, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}
