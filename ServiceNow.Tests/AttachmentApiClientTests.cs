using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.IO;
using System.Net.Http;

namespace ServiceNow.Tests;

public class AttachmentApiClientTests {
    private static AttachmentApiClient Create() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var client = new ServiceNowClient(http, settings);
        return new AttachmentApiClient(client, settings);
    }

    [Fact]
    public async Task UploadAttachmentAsync_NullFile_Throws() {
        var client = Create();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UploadAttachmentAsync("task", "1", null!, "name"));
    }

    [Fact]
    public async Task UploadAttachmentAsync_NullFileName_Throws() {
        var client = Create();
        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UploadAttachmentAsync("task", "1", stream, null!));
    }
}
