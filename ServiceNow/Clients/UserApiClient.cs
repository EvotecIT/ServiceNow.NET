using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for CRUD operations on sys_user and sys_user_group tables.
/// </summary>
public class UserApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public UserApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    private TableApiClient TableClient => new(_client, _settings);

    /// <summary>
    /// Retrieves a sys_user record.
    /// </summary>
    public Task<SysUser?> GetUserAsync(string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => TableClient.GetRecordAsync<SysUser>("sys_user", sysId, filters, cancellationToken);

    /// <summary>
    /// Lists sys_user records.
    /// </summary>
    public Task<List<SysUser>> ListUsersAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => TableClient.ListRecordsAsync<SysUser>("sys_user", filters, cancellationToken);

    /// <summary>
    /// Creates a sys_user record.
    /// </summary>
    public Task CreateUserAsync(SysUser user, CancellationToken cancellationToken = default)
        => TableClient.CreateRecordAsync("sys_user", user, cancellationToken);

    /// <summary>
    /// Updates a sys_user record.
    /// </summary>
    public Task UpdateUserAsync(string sysId, SysUser user, CancellationToken cancellationToken = default)
        => TableClient.UpdateRecordAsync("sys_user", sysId, user, cancellationToken);

    /// <summary>
    /// Deletes a sys_user record.
    /// </summary>
    public Task DeleteUserAsync(string sysId, CancellationToken cancellationToken = default)
        => TableClient.DeleteRecordAsync("sys_user", sysId, cancellationToken);

    /// <summary>
    /// Retrieves a sys_user_group record.
    /// </summary>
    public Task<SysUserGroup?> GetGroupAsync(string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => TableClient.GetRecordAsync<SysUserGroup>("sys_user_group", sysId, filters, cancellationToken);

    /// <summary>
    /// Lists sys_user_group records.
    /// </summary>
    public Task<List<SysUserGroup>> ListGroupsAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
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
