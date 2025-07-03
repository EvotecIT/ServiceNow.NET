using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace ServiceNow.Clients;

/// <summary>
/// Client for interacting with the ServiceNow Attachment API.
/// </summary>
public class AttachmentApiClient
{
    private readonly ServiceNowClient _client;

    public AttachmentApiClient(ServiceNowClient client) => _client = client;

    public async Task<HttpResponseMessage> GetAttachmentAsync(string sysId)
        => await _client.GetAsync($"/api/now/attachment/{sysId}").ConfigureAwait(false);

    public async Task UploadAttachmentAsync(string table, string sysId, Stream file, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _client.PostAsync($"/api/now/attachment/file?table_name={table}&table_sys_id={sysId}", content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAttachmentAsync(string sysId)
    {
        var response = await _client.DeleteAsync($"/api/now/attachment/{sysId}").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
