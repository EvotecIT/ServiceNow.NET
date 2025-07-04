using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for starting and checking ServiceNow workflow executions.
/// </summary>
public class WorkflowApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkflowApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public WorkflowApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Starts a workflow and returns the execution ID.
    /// </summary>
    /// <param name="workflowId">Workflow sys_id or name.</param>
    /// <param name="payload">Input parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<string> StartExecutionAsync(string workflowId, object payload, CancellationToken cancellationToken = default) {
        var response = await _client.PostAsync($"/api/now/{_settings.ApiVersion}/workflow/{workflowId}/start", payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("result", out var result) &&
            result.TryGetProperty("execution_id", out var id)) {
            return id.GetString() ?? string.Empty;
        }
        return string.Empty;
    }

    /// <summary>
    /// Retrieves the status of a workflow execution.
    /// </summary>
    /// <param name="executionId">Execution identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<string> GetExecutionStatusAsync(string executionId, CancellationToken cancellationToken = default) {
        var response = await _client.GetAsync($"/api/now/{_settings.ApiVersion}/workflow/execution/{executionId}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("result", out var result) &&
            result.TryGetProperty("status", out var status)) {
            return status.GetString() ?? string.Empty;
        }
        return string.Empty;
    }
}
