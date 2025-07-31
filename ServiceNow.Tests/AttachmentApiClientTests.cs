using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;

namespace ServiceNow.Tests;

public class AttachmentApiClientTests {
    private static AttachmentApiClient Create() {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = (ServiceNowClient)provider.GetRequiredService<IServiceNowClient>();
        return new AttachmentApiClient(client, settings);
    }

    private static (AttachmentApiClient Client, MockHttpMessageHandler Handler) CreateWithHandler(string version = "v2") {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            ApiVersion = version
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = (ServiceNowClient)provider.GetRequiredService<IServiceNowClient>();
        return (new AttachmentApiClient(client, settings), handler);
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

    [Fact]
    public async Task SearchAttachmentsAsync_SendsGet() {
        var (client, handler) = CreateWithHandler();
        handler.Response.Content = new StringContent("[]");

        var list = await client.SearchAttachmentsAsync<object>("task", "1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/now/v2/attachment?table_name=task&table_sys_id=1", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task SearchAttachmentsAsync_UsesCustomApiVersion() {
        var (client, handler) = CreateWithHandler("v1");
        handler.Response.Content = new StringContent("[]");

        await client.SearchAttachmentsAsync<object>("task", "2", CancellationToken.None);

        Assert.Equal("/api/now/v1/attachment?table_name=task&table_sys_id=2", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}
