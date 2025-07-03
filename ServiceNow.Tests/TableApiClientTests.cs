using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ServiceNow.Tests;

public class TableApiClientTests {
    private static (TableApiClient Client, MockHttpMessageHandler Handler) Create() {
        var handler = new MockHttpMessageHandler {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}")
            }
        };
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var snClient = new ServiceNowClient(http, settings);
        return (new TableApiClient(snClient), handler);
    }

    [Fact]
    public async Task GetRecordAsync_SendsCorrectRequest() {
        var (client, handler) = Create();
        handler.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var record = await client.GetRecordAsync<TaskRecord>("task", "1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/api/now/table/task/1", handler.LastRequest?.RequestUri?.ToString());
        Assert.NotNull(record);
        Assert.Equal("1", record!.SysId);
    }

    [Fact]
    public async Task CreateRecordAsync_SendsPost() {
        var (client, handler) = Create();

        await client.CreateRecordAsync("task", new { name = "foo" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/api/now/table/task", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task UpdateRecordAsync_SendsPut() {
        var (client, handler) = Create();

        await client.UpdateRecordAsync("task", "2", new { name = "bar" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/api/now/table/task/2", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task DeleteRecordAsync_SendsDelete() {
        var (client, handler) = Create();

        await client.DeleteRecordAsync("task", "3", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/api/now/table/task/3", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetRecordAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var snClient = new ServiceNowClient(http, settings);
        var client = new TableApiClient(snClient);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetRecordAsync<TaskRecord>("task", "1", cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }
}