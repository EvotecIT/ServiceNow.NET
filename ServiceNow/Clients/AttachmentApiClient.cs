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

    public AttachmentApiClient(ServiceNowClient client, ServiceNowSettings settings) {
        _client = client;
        _settings = settings;
    }

    public async Task<HttpResponseMessage> GetAttachmentAsync(string sysId, CancellationToken cancellationToken = default)
        => await _client.GetAsync($"/api/now/{_settings.ApiVersion}/attachment/{sysId}", cancellationToken).ConfigureAwait(false);

    public async Task UploadAttachmentAsync(string table, string sysId, Stream file, string fileName) {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(file);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await _client.PostAsync($"/api/now/{_settings.ApiVersion}/attachment/file?table_name={table}&table_sys_id={sysId}", content, CancellationToken.None).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync($"/api/now/{_settings.ApiVersion}/attachment/{sysId}", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}