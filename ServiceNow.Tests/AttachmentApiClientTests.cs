using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.IO;
using System.Net.Http;

namespace ServiceNow.Tests;

public class AttachmentApiClientTests {
    private static (AttachmentApiClient Client, MockHttpMessageHandler Handler) Create() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var client = new ServiceNowClient(http, settings);
        return (new AttachmentApiClient(client, settings), handler);
    }

    [Fact]
    public async Task UploadAttachmentAsync_NullFile_Throws() {
        var (client, _) = Create();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UploadAttachmentAsync("task", "1", null!, "name"));
    }

    [Fact]
    public async Task UploadAttachmentAsync_NullFileName_Throws() {
        var (client, _) = Create();
        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UploadAttachmentAsync("task", "1", stream, null!));
    }

    [Fact]
    public async Task UploadAttachmentAsync_SendsMultipart() {
        var (client, handler) = Create();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        await client.UploadAttachmentAsync("task", "1", stream, "file.txt", CancellationToken.None);

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/api/now/v2/attachment/file?table_name=task&table_sys_id=1",
            handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("multipart/form-data", handler.LastRequest?.Content?.Headers.ContentType?.MediaType);
    }
}
