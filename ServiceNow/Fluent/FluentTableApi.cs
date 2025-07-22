using ServiceNow.Clients;
using System.Collections.Generic;
using ServiceNow;

namespace ServiceNow.Fluent;

/// <summary>
/// Provides a fluent interface for working with the ServiceNow Table API.
/// </summary>
public class FluentTableApi<TRecord> {
    private readonly TableApiClient _client;
    private readonly string _table;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentTableApi{TRecord}"/> class.
    /// </summary>
    /// <param name="client">Underlying ServiceNow client.</param>
    /// <param name="table">Table name.</param>
    public FluentTableApi(ServiceNowClient client, string table) {
        _client = new TableApiClient(client, client.Settings);
        _table = table;
    }

    /// <summary>
    /// Gets the table name this instance operates on.
    /// </summary>
    public string TableName => _table;

    /// <summary>
    /// Retrieves a record from the table.
    /// </summary>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<TRecord?> GetAsync(string sysId, TableQueryOptions? options = null, CancellationToken cancellationToken = default)
        => await _client.GetRecordAsync<TRecord>(_table, sysId, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Lists records from the table.
    /// </summary>
    /// <param name="filters">Optional query filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<TRecord>> ListAsync(TableQueryOptions? options = null, CancellationToken cancellationToken = default)
        => await _client.ListRecordsAsync<TRecord>(_table, options, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Creates a record in the table.
    /// </summary>
    /// <param name="record">Record payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CreateAsync(TRecord record, CancellationToken cancellationToken = default)
        => await _client.CreateRecordAsync(_table, record, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Updates a record in the table.
    /// </summary>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="record">Record payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task UpdateAsync(string sysId, TRecord record, CancellationToken cancellationToken = default)
        => await _client.UpdateRecordAsync(_table, sysId, record, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Deletes a record from the table.
    /// </summary>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteAsync(string sysId, CancellationToken cancellationToken = default)
        => await _client.DeleteRecordAsync(_table, sysId, cancellationToken).ConfigureAwait(false);
}