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
        => await _client.GetAsync(string.Format(ServiceNowApiPaths.Attachment, _settings.ApiVersion, sysId), cancellationToken).ConfigureAwait(false);

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
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAttachmentAsync(string sysId, CancellationToken cancellationToken = default) {
        var response = await _client.DeleteAsync(string.Format(ServiceNowApiPaths.Attachment, _settings.ApiVersion, sysId), cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}