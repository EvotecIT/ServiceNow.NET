using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Attachment API.
/// </summary>
public class AttachmentApiClient {
    private readonly ServiceNowClient _client;

    public AttachmentApiClient(ServiceNowClient client) => _client = client;

    public async Task<HttpResponseMessage> GetAttachmentAsync(string sysId, CancellationToken cancellationToken = default)
        => await _client.GetAsync($"/api/now/attachment/{sysId}", cancellationToken).ConfigureAwait(false);

    public async Task UploadAttachmentAsync(string table, string sysId, Stream file, string fileName) {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        using var response = await _client.PostAsync($"/api/now/attachment/file?table_name={table}&table_sys_id={sysId}", content, CancellationToken.None).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        using var response = await _client.DeleteAsync($"/api/now/attachment/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}