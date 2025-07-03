using ServiceNow.Clients;

namespace ServiceNow.Fluent;

/// <summary>
/// Provides a fluent interface for working with the ServiceNow Table API.
/// </summary>
public class FluentTableApi {
    private readonly TableApiClient _client;
    private readonly string _table;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentTableApi"/> class.
    /// </summary>
    /// <param name="client">Underlying ServiceNow client.</param>
    /// <param name="table">Table name.</param>
    public FluentTableApi(ServiceNowClient client, string table) {
        _client = new TableApiClient(client);
        _table = table;
    }

    /// <summary>
    /// Gets the table name this instance operates on.
    /// </summary>
    public string TableName => _table;

    /// <summary>
    /// Retrieves a record from the table.
    /// </summary>
    public async Task<T?> GetAsync<T>(string sysId)
        => await _client.GetRecordAsync<T>(_table, sysId).ConfigureAwait(false);

    /// <summary>
    /// Creates a record in the table.
    /// </summary>
    public async Task CreateAsync<T>(T record)
        => await _client.CreateRecordAsync(_table, record).ConfigureAwait(false);

    /// <summary>
    /// Updates a record in the table.
    /// </summary>
    public async Task UpdateAsync<T>(string sysId, T record)
        => await _client.UpdateRecordAsync(_table, sysId, record).ConfigureAwait(false);

    /// <summary>
    /// Deletes a record from the table.
    /// </summary>
    public async Task DeleteAsync(string sysId)
        => await _client.DeleteRecordAsync(_table, sysId).ConfigureAwait(false);
}