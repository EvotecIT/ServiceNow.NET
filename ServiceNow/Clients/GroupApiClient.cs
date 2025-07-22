using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using ServiceNow;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for CRUD operations on sys_user_group table.
/// </summary>
public class GroupApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public GroupApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    private TableApiClient TableClient => new(_client, _settings);

    /// <summary>
    /// Retrieves a sys_user_group record.
    /// </summary>
    public Task<SysUserGroup?> GetGroupAsync(string sysId, TableQueryOptions? filters = null, CancellationToken cancellationToken = default)
        => TableClient.GetRecordAsync<SysUserGroup>("sys_user_group", sysId, filters, cancellationToken);

    /// <summary>
    /// Lists sys_user_group records.
    /// </summary>
    public Task<List<SysUserGroup>> ListGroupsAsync(TableQueryOptions? filters = null, CancellationToken cancellationToken = default)
        => TableClient.ListRecordsAsync<SysUserGroup>("sys_user_group", filters, cancellationToken);

    /// <summary>
    /// Creates a sys_user_group record.
    /// </summary>
    public Task CreateGroupAsync(SysUserGroup group, CancellationToken cancellationToken = default)
        => TableClient.CreateRecordAsync("sys_user_group", group, cancellationToken);

    /// <summary>
    /// Updates a sys_user_group record.
    /// </summary>
    public Task UpdateGroupAsync(string sysId, SysUserGroup group, CancellationToken cancellationToken = default)
        => TableClient.UpdateRecordAsync("sys_user_group", sysId, group, cancellationToken);

    /// <summary>
    /// Deletes a sys_user_group record.
    /// </summary>
    public Task DeleteGroupAsync(string sysId, CancellationToken cancellationToken = default)
        => TableClient.DeleteRecordAsync("sys_user_group", sysId, cancellationToken);
}
