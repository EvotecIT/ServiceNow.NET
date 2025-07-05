using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for posting records via the Import Set API.
/// </summary>
public class ImportSetApiClient {
    private readonly IServiceNowClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportSetApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    public ImportSetApiClient(IServiceNowClient client) => _client = client;

    /// <summary>
    /// Posts a payload to an import set table.
    /// </summary>
    /// <param name="table">Import set table name.</param>
    /// <param name="payload">Payload object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ImportAsync<T>(string table, T payload, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.ImportSet, table);
        var response = await _client.PostAsync(path, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
    }
}
