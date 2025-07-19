using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Utilities;
using ServiceNow.Extensions;

namespace ServiceNow.Clients;

/// <summary>
/// Client for sending email notifications and retrieving inbound emails.
/// </summary>
public class EmailApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public EmailApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Sends an email notification payload.
    /// </summary>
    /// <param name="payload">Notification payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendEmailAsync<T>(T payload, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.EmailOutbound, _settings.ApiVersion);
        var response = await _client.PostAsync(path, payload, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves details about an inbound email by identifier.
    /// </summary>
    /// <param name="emailId">Inbound email identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<T?> GetInboundEmailAsync<T>(string emailId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.EmailInbound, _settings.ApiVersion, emailId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, ServiceNowJson.Default);
    }
}
