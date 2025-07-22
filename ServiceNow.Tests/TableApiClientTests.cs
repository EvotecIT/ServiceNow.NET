using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using ServiceNow;
using ServiceNow.Queries;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ServiceNow.Tests;

public class TableApiClientTests {
    private static (TableApiClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}")
            }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new TableApiClient(mock, settings), mock);
    }

    private static TableApiClient CreateReal(HttpMessageHandler handler) {
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
        return provider.GetRequiredService<TableApiClient>();
    }

    [Fact]
    public async Task GetRecordAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var record = await client.GetRecordAsync<TaskRecord>("task", "1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task/1", mock.LastRelativeUrl);
        Assert.NotNull(record);
        Assert.Equal("1", record!.SysId);
    }

    [Fact]
    public async Task GetRecordAsync_WithOptions_AppendsQueryString() {
        var (client, mock) = Create();
        var opts = new TableQueryOptions { Fields = ["sys_id"] };

        await client.GetRecordAsync<TaskRecord>("task", "1", opts, CancellationToken.None);

        Assert.Equal("/api/now/v2/table/task/1?sysparm_fields=sys_id", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task CreateRecordAsync_SendsPost() {
        var (client, mock) = Create();

        await client.CreateRecordAsync("task", new { name = "foo" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task UpdateRecordAsync_SendsPut() {
        var (client, mock) = Create();

        await client.UpdateRecordAsync("task", "2", new { name = "bar" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task/2", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task DeleteRecordAsync_SendsDelete() {
        var (client, mock) = Create();

        await client.DeleteRecordAsync("task", "3", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task/3", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task ListRecordsAsync_SendsGetWithOptions() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var opts = new TableQueryOptions { AdditionalParameters = { ["state"] = "1" } };

        var records = await client.ListRecordsAsync<TaskRecord>("task", opts, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task?state=1", mock.LastRelativeUrl);
        Assert.NotNull(records);
    }

    [Fact]
    public async Task GetRecordAsync_WithMultipleOptions_BuildsQueryString() {
        var (client, mock) = Create();
        var opts = new TableQueryOptions {
            Fields = ["sys_id", "number"],
            Query = new QueryBuilder().And("active=true"),
            DisplayValue = "true",
            ExcludeReferenceLinks = true,
            AdditionalParameters = { ["foo"] = "bar" }
        };

        await client.GetRecordAsync<TaskRecord>("task", "1", opts, CancellationToken.None);

        var expectedDict = new Dictionary<string, object?> {
            ["sysparm_fields"] = new[] { "sys_id", "number" },
            ["sysparm_query"] = "active=true",
            ["sysparm_display_value"] = "true",
            ["sysparm_exclude_reference_link"] = "true",
            ["foo"] = "bar"
        };
        Assert.Equal($"/api/now/v2/table/task/1?{expectedDict.ToQueryString()}", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task UsesCustomApiVersionInUrls() {
        var (client, mock) = Create("v1");

        await client.GetRecordAsync<TaskRecord>("task", "42", null, CancellationToken.None);

        Assert.Equal("/api/now/v1/table/task/42", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task GetRecordAsync_ErrorResponse_ThrowsServiceNowException() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new TableApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.GetRecordAsync<TaskRecord>("task", "1", null, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task GetRecordAsync_InvalidJson_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{invalid")
            }
        };
        var client = new TableApiClient(mock, new ServiceNowSettings());

        await Assert.ThrowsAnyAsync<JsonException>(() => client.GetRecordAsync<TaskRecord>("task", "1", null, CancellationToken.None));
    }

    [Fact]
    public async Task GetRecordAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var client = CreateReal(handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetRecordAsync<TaskRecord>("task", "1", null, cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }

    [Fact]
    public async Task PageRecordsAsync_SendsLimitAndOffset() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var records = await client.PageRecordsAsync<TaskRecord>("task", 5, 10, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/task?sysparm_limit=5&sysparm_offset=10", mock.LastRelativeUrl);
        Assert.NotNull(records);
    }

    [Fact]
    public async Task PageRecordsAsync_ErrorResponse_ThrowsServiceNowException() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new TableApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.PageRecordsAsync<TaskRecord>("task", 1, 0, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }
    [Fact]
    public async Task StreamRecordsAsync_YieldsRecordsAcrossPages() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("[{\"SysId\":\"1\"}]")
        });
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("[]")
        });
        var client = CreateReal(handler);

        var list = new List<TaskRecord>();
        await foreach (var r in client.StreamRecordsAsync<TaskRecord>("task", 1, CancellationToken.None)) {
            list.Add(r);
        }

        Assert.Single(list);
        Assert.Equal("1", list[0].SysId);
        Assert.Equal("/api/now/v2/table/task?sysparm_limit=1&sysparm_offset=0", handler.Requests[0].RequestUri?.PathAndQuery);
        Assert.Equal("/api/now/v2/table/task?sysparm_limit=1&sysparm_offset=1", handler.Requests[1].RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task StreamRecordsAsync_Error_ThrowsServiceNowException() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") });
        var client = CreateReal(handler);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(async () => {
            await foreach (var _ in client.StreamRecordsAsync<TaskRecord>("task", 1, CancellationToken.None)) { }
        });
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task ListAllRecordsAsync_ReturnsRecordsAcrossPages() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("[{\"SysId\":\"1\"}]")
        });
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("[]")
        });
        var client = CreateReal(handler);

        var list = await client.ListAllRecordsAsync<TaskRecord>("task", 1, CancellationToken.None);

        Assert.Single(list);
        Assert.Equal("1", list[0].SysId);
        Assert.Equal("/api/now/v2/table/task?sysparm_limit=1&sysparm_offset=0", handler.Requests[0].RequestUri?.PathAndQuery);
        Assert.Equal("/api/now/v2/table/task?sysparm_limit=1&sysparm_offset=1", handler.Requests[1].RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ListAllRecordsAsync_Error_ThrowsServiceNowException() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") });
        var client = CreateReal(handler);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.ListAllRecordsAsync<TaskRecord>("task", 1, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, typeof(ServiceNowAuthorizationException))]
    [InlineData(HttpStatusCode.Forbidden, typeof(ServiceNowAuthorizationException))]
    [InlineData(HttpStatusCode.NotFound, typeof(ServiceNowNotFoundException))]
    [InlineData((HttpStatusCode)429, typeof(ServiceNowRateLimitException))]
    [InlineData(HttpStatusCode.InternalServerError, typeof(ServiceNowServerException))]
    public async Task GetRecordAsync_ThrowsSpecificExceptions(HttpStatusCode status, Type expected) {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(status) { Content = new StringContent("bad") }
        };
        var client = new TableApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync(expected, () => client.GetRecordAsync<TaskRecord>("task", "1", null, CancellationToken.None));
        var snEx = Assert.IsAssignableFrom<ServiceNowException>(ex);
        Assert.Equal(status, snEx.StatusCode);
    }
}
