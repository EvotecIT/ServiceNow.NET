using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Attachment API.
/// </summary>
/// <summary>
/// Provides operations for working with attachments in ServiceNow.
/// </summary>
public class AttachmentApiClient {
    private readonly ServiceNowClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachmentApiClient"/> class.
    /// </summary>
    /// <param name="client">The underlying ServiceNow client.</param>
    public AttachmentApiClient(ServiceNowClient client) => _client = client;

    /// <summary>
    /// Retrieves an attachment.
    /// </summary>
    /// <param name="sysId">The sys_id of the attachment.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task<HttpResponseMessage> GetAttachmentAsync(string sysId, CancellationToken cancellationToken = default)
        => await _client.GetAsync($"/api/now/attachment/{sysId}", cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Uploads a file as an attachment.
    /// </summary>
    /// <param name="table">The table name.</param>
    /// <param name="sysId">The sys_id of the record.</param>
    /// <param name="file">The file stream.</param>
    /// <param name="fileName">Name of the file.</param>
    public async Task UploadAttachmentAsync(string table, string sysId, Stream file, string fileName) {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _client.PostAsync($"/api/now/attachment/file?table_name={table}&table_sys_id={sysId}", content, CancellationToken.None).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes an attachment.
    /// </summary>
    /// <param name="sysId">The sys_id of the attachment.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/attachment/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}