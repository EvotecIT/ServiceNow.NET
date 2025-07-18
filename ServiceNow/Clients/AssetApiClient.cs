using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using ServiceNow.Models;

namespace ServiceNow.Clients;

/// <summary>
/// Client for CRUD operations on the alm_asset table.
/// </summary>
public class AssetApiClient {
    private readonly IServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public AssetApiClient(IServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    private TableApiClient TableClient => new(_client, _settings);

    /// <summary>
    /// Retrieves an asset record.
    /// </summary>
    public Task<Asset?> GetAssetAsync(string sysId, Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => TableClient.GetRecordAsync<Asset>("alm_asset", sysId, filters, cancellationToken);

    /// <summary>
    /// Lists asset records.
    /// </summary>
    public Task<List<Asset>> ListAssetsAsync(Dictionary<string, string?>? filters = null, CancellationToken cancellationToken = default)
        => TableClient.ListRecordsAsync<Asset>("alm_asset", filters, cancellationToken);

    /// <summary>
    /// Creates an asset record.
    /// </summary>
    public Task CreateAssetAsync(Asset asset, CancellationToken cancellationToken = default)
        => TableClient.CreateRecordAsync("alm_asset", asset, cancellationToken);

    /// <summary>
    /// Updates an asset record.
    /// </summary>
    public Task UpdateAssetAsync(string sysId, Asset asset, CancellationToken cancellationToken = default)
        => TableClient.UpdateRecordAsync("alm_asset", sysId, asset, cancellationToken);

    /// <summary>
    /// Deletes an asset record.
    /// </summary>
    public Task DeleteAssetAsync(string sysId, CancellationToken cancellationToken = default)
        => TableClient.DeleteRecordAsync("alm_asset", sysId, cancellationToken);
}
