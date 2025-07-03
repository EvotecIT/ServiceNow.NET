using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ServiceNow.Tests;

public class TableApiClientTests {
    private static (TableApiClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}")
            }
        };
        return (new TableApiClient(mock), mock);
    }

    [Fact]
    public async Task GetRecordAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var record = await client.GetRecordAsync<TaskRecord>("task", "1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/table/task/1", mock.LastRelativeUrl);
        Assert.NotNull(record);
        Assert.Equal("1", record!.SysId);
    }

    [Fact]
    public async Task GetRecordAsync_WithFilters_AppendsQueryString() {
        var (client, mock) = Create();
        var filters = new Dictionary<string, string?> { ["fields"] = "sys_id" };

        await client.GetRecordAsync<TaskRecord>("task", "1", filters, CancellationToken.None);

        Assert.Equal("/api/now/table/task/1?" + filters.ToQueryString(), mock.LastRelativeUrl);
    }

    [Fact]
    public async Task CreateRecordAsync_SendsPost() {
        var (client, mock) = Create();

        await client.CreateRecordAsync("task", new { name = "foo" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/table/task", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task UpdateRecordAsync_SendsPut() {
        var (client, mock) = Create();

        await client.UpdateRecordAsync("task", "2", new { name = "bar" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/table/task/2", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task DeleteRecordAsync_SendsDelete() {
        var (client, mock) = Create();

        await client.DeleteRecordAsync("task", "3", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, mock.LastMethod);
        Assert.Equal("/api/now/table/task/3", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task ListRecordsAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["state"] = "1" };

        var records = await client.ListRecordsAsync<TaskRecord>("task", filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/table/task?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(records);
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

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetRecordAsync<TaskRecord>("task", "1", null, cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }
}