using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Text.Json;

namespace ServiceNow.Tests;

public class TableMetadataClientTests {
    private static (TableMetadataClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{\"result\":[{\"element\":\"sys_id\",\"internal_type\":\"string\"}]}")
            }
        };
        var settings = new ServiceNowSettings();
        return (new TableMetadataClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetMetadataAsync_ReturnsFields() {
        var (client, mock) = Create();

        var meta = await client.GetMetadataAsync("task", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/table/sys_dictionary?sysparm_query=name=task&sysparm_fields=element,internal_type", mock.LastRelativeUrl);
        Assert.Single(meta.Fields);
        Assert.Equal("sys_id", meta.Fields[0].Name);
        Assert.Equal("string", meta.Fields[0].Type);
    }

    [Fact]
    public async Task GetMetadataAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new TableMetadataClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.GetMetadataAsync("task", CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task GetMetadataAsync_InvalidJson_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{invalid")
            }
        };
        var client = new TableMetadataClient(mock, new ServiceNowSettings());

        await Assert.ThrowsAnyAsync<JsonException>(() => client.GetMetadataAsync("task", CancellationToken.None));
    }

    [Fact]
    public async Task GetMetadataAsync_CancelledRequest_Throws() {
        var handler = new CancelMessageHandler();
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
        var client = provider.GetRequiredService<TableMetadataClient>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetMetadataAsync("task", cts.Token));
    }

    [Fact]
    public async Task GetMetadataAsync_UsesCache() {
        var mock = new CountingServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{\"result\":[{\"element\":\"id\",\"internal_type\":\"string\"}]}")
            }
        };
        var settings = new ServiceNowSettings { MetadataCacheDuration = TimeSpan.FromMinutes(5) };
        var client = new TableMetadataClient(mock, settings);

        var first = await client.GetMetadataAsync("task", CancellationToken.None);
        mock.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var second = await client.GetMetadataAsync("task", CancellationToken.None);

        Assert.Equal(1, mock.GetCount);
        Assert.Same(first, second);
    }
}
