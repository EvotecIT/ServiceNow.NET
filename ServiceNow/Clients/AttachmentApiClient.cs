using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using ServiceNow.Configuration;

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
        => await _client.GetAsync($"/api/now/{_settings.ApiVersion}/attachment/{sysId}", cancellationToken).ConfigureAwait(false);

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

        var response = await _client.PostAsync($"/api/now/{_settings.ApiVersion}/attachment/file?table_name={table}&table_sys_id={sysId}", content, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes an attachment.
    /// </summary>
    /// <param name="sysId">Attachment sys_id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/{_settings.ApiVersion}/attachment/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}