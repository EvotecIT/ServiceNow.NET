using ServiceNow.Clients;
using System.Collections.Generic;
using ServiceNow.Enums;

namespace ServiceNow.Extensions;

/// <summary>
/// Extension methods for changing task states via the Table API.
/// </summary>
public static class TableApiClientTaskExtensions {
    /// <summary>
    /// Updates the state field of a task record.
    /// </summary>
    /// <param name="client">The table API client.</param>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="state">New state value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task SetStateAsync<TState>(this TableApiClient client, string table, string sysId, TState state, CancellationToken cancellationToken = default)
        where TState : struct, Enum {
        if (!Enum.IsDefined(typeof(TState), state)) {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        var payload = new Dictionary<string, TState> { ["state"] = state };
        return client.UpdateRecordAsync(table, sysId, payload, cancellationToken);
    }

    /// <summary>
    /// Updates the state of an incident record.
    /// </summary>
    /// <param name="client">The table API client.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="state">New incident state.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task SetStateAsync(this TableApiClient client, string sysId, IncidentState state, CancellationToken cancellationToken = default)
        => client.SetStateAsync("incident", sysId, state, cancellationToken);
}
