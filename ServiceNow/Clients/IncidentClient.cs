using ServiceNow.Configuration;
using ServiceNow.Enums;
using ServiceNow.Extensions;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for common incident operations.
/// </summary>
public class IncidentClient {
    private readonly TableApiClient _tableClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncidentClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public IncidentClient(IServiceNowClient client, ServiceNowSettings settings)
        => _tableClient = new TableApiClient(client, settings);

    /// <summary>
    /// Retrieves an incident record.
    /// </summary>
    /// <param name="sysId">Incident sys_id.</param>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<Incident?> GetIncidentAsync(string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => _tableClient.GetRecordAsync<Incident>("incident", sysId, filters, cancellationToken);

    /// <summary>
    /// Lists incident records.
    /// </summary>
    public Task<List<Incident>> ListIncidentsAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => _tableClient.ListRecordsAsync<Incident>("incident", filters, cancellationToken);

    /// <summary>
    /// Creates an incident record.
    /// </summary>
    public Task CreateIncidentAsync(Incident incident, CancellationToken cancellationToken = default)
        => _tableClient.CreateRecordAsync("incident", incident, cancellationToken);

    /// <summary>
    /// Updates an incident record.
    /// </summary>
    public Task UpdateIncidentAsync(string sysId, Incident incident, CancellationToken cancellationToken = default)
        => _tableClient.UpdateRecordAsync("incident", sysId, incident, cancellationToken);

    /// <summary>
    /// Deletes an incident record.
    /// </summary>
    public Task DeleteIncidentAsync(string sysId, CancellationToken cancellationToken = default)
        => _tableClient.DeleteRecordAsync("incident", sysId, cancellationToken);

    /// <summary>
    /// Assigns an incident to a user and moves it to In Progress state.
    /// </summary>
    /// <param name="sysId">Incident sys_id.</param>
    /// <param name="assignee">User sys_id.</param>
    /// <param name="currentState">Current incident state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task AssignAsync(string sysId, string assignee, IncidentState currentState, CancellationToken cancellationToken = default) {
        if (currentState is not IncidentState.New and not IncidentState.OnHold and not IncidentState.InProgress) {
            throw new InvalidOperationException($"Cannot assign incident in {currentState} state.");
        }

        var payload = new Dictionary<string, object?> {
            ["assigned_to"] = assignee,
            ["state"] = IncidentState.InProgress
        };

        return _tableClient.UpdateRecordAsync("incident", sysId, payload, cancellationToken);
    }

    /// <summary>
    /// Resolves an incident.
    /// </summary>
    /// <param name="sysId">Incident sys_id.</param>
    /// <param name="currentState">Current incident state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task ResolveAsync(string sysId, IncidentState currentState, CancellationToken cancellationToken = default) {
        if (currentState != IncidentState.InProgress && currentState != IncidentState.OnHold) {
            throw new InvalidOperationException($"Cannot resolve incident in {currentState} state.");
        }

        var payload = new Dictionary<string, object?> {
            ["state"] = IncidentState.Resolved
        };

        return _tableClient.UpdateRecordAsync("incident", sysId, payload, cancellationToken);
    }

    /// <summary>
    /// Closes an incident.
    /// </summary>
    /// <param name="sysId">Incident sys_id.</param>
    /// <param name="currentState">Current incident state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task CloseAsync(string sysId, IncidentState currentState, CancellationToken cancellationToken = default) {
        if (currentState != IncidentState.Resolved) {
            throw new InvalidOperationException($"Cannot close incident in {currentState} state.");
        }

        var payload = new Dictionary<string, object?> {
            ["state"] = IncidentState.Closed
        };

        return _tableClient.UpdateRecordAsync("incident", sysId, payload, cancellationToken);
    }
}
