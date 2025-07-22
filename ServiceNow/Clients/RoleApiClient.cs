using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Models;
using ServiceNow;

namespace ServiceNow.Clients;

/// <summary>
/// Client for retrieving sys_user_role records.
/// </summary>
public class RoleApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public RoleApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    private TableApiClient TableClient => new(_client, _settings);

    /// <summary>
    /// Retrieves a sys_user_role record.
    /// </summary>
    public Task<SysUserRole?> GetRoleAsync(string sysId, TableQueryOptions? options = null, CancellationToken cancellationToken = default)
        => TableClient.GetRecordAsync<SysUserRole>("sys_user_role", sysId, options, cancellationToken);

    /// <summary>
    /// Lists sys_user_role records.
    /// </summary>
    public Task<List<SysUserRole>> ListRolesAsync(TableQueryOptions? options = null, CancellationToken cancellationToken = default)
        => TableClient.ListRecordsAsync<SysUserRole>("sys_user_role", options, cancellationToken);
}
