using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Attachment API.
/// </summary>
public class AttachmentApiClient {
    private readonly ServiceNowClient _client;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachmentApiClient"/> class.
    /// </summary>
    /// <param name="client">Underlying ServiceNow client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public AttachmentApiClient(ServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    /// <summary>
    /// Retrieves an attachment.
    /// </summary>
    /// <param name="sysId">Attachment sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> GetAttachmentAsync(string sysId, CancellationToken cancellationToken = default)
        => await _client.GetAsync(string.Format(ServiceNowApiPaths.Attachment, _settings.ApiVersion, sysId), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Searches attachments for a record.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<List<T>> SearchAttachmentsAsync<T>(string table, string sysId, CancellationToken cancellationToken = default) {
        var path = string.Format(ServiceNowApiPaths.AttachmentSearch, _settings.ApiVersion, table, sysId);
        var response = await _client.GetAsync(path, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, ServiceNowJson.Default) ?? new();
    }

    /// <summary>
    /// Uploads an attachment to a record.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="sysId">Record sys_id.</param>
    /// <param name="file">File stream.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task UploadAttachmentAsync(string table, string sysId, Stream file, string fileName, CancellationToken cancellationToken = default) {
        if (file is null) {
            throw new ArgumentNullException(nameof(file));
        }

        if (fileName is null) {
            throw new ArgumentNullException(nameof(fileName));
        }

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _client.PostAsync(string.Format(ServiceNowApiPaths.AttachmentFile, _settings.ApiVersion, table, sysId), content, cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an attachment.
    /// </summary>
    /// <param name="sysId">Attachment sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync(string.Format(ServiceNowApiPaths.Attachment, _settings.ApiVersion, sysId), cancellationToken).ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);
    }
}