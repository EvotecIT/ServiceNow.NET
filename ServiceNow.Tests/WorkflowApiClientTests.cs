using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class WorkflowApiClientTests {
    private static (WorkflowApiClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{}")
            }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new WorkflowApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task StartExecutionAsync_SendsPostAndReturnsId() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"result\":{\"execution_id\":\"42\"}}");

        var id = await client.StartExecutionAsync("wf1", new { x = 1 }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/v2/workflow/wf1/start", mock.LastRelativeUrl);
        Assert.Equal("42", id);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task GetExecutionStatusAsync_SendsGetAndReturnsStatus() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"result\":{\"status\":\"complete\"}}");

        var status = await client.GetExecutionStatusAsync("e1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/workflow/execution/e1", mock.LastRelativeUrl);
        Assert.Equal("complete", status);
    }

    [Fact]
    public async Task StartExecutionAsync_Error_ThrowsServiceNowException() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new WorkflowApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.StartExecutionAsync("wf", new { }, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task GetExecutionStatusAsync_Error_ThrowsServiceNowException() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new WorkflowApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.GetExecutionStatusAsync("e1", CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task StartExecutionAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var snClient = new ServiceNowClient(http, settings);
        var client = new WorkflowApiClient(snClient, settings);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.StartExecutionAsync("wf", new { }, cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }
}
