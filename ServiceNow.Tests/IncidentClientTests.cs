using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Enums;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class IncidentClientTests {
    private static (IncidentClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new IncidentClient(mock, settings), mock);
    }

    [Fact]
    public async Task AssignAsync_SendsPut() {
        var (client, mock) = Create();

        await client.AssignAsync("1", "u1", IncidentState.New, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/incident/1", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task ResolveAsync_SendsPut() {
        var (client, mock) = Create();

        await client.ResolveAsync("2", IncidentState.InProgress, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/incident/2", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task CloseAsync_SendsPut() {
        var (client, mock) = Create();

        await client.CloseAsync("3", IncidentState.Resolved, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/incident/3", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task AssignAsync_InvalidState_Throws() {
        var (client, _) = Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.AssignAsync("1", "u", IncidentState.Closed, CancellationToken.None));
    }

    [Fact]
    public async Task ResolveAsync_InvalidState_Throws() {
        var (client, _) = Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.ResolveAsync("1", IncidentState.New, CancellationToken.None));
    }

    [Fact]
    public async Task CloseAsync_InvalidState_Throws() {
        var (client, _) = Create();

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.CloseAsync("1", IncidentState.New, CancellationToken.None));
    }

    [Fact]
    public async Task AssignAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings { BaseUrl = "https://example.com", Username = "u", Password = "p" };
        var snClient = new ServiceNowClient(http, settings);
        var client = new IncidentClient(snClient, settings);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.AssignAsync("1", "u", IncidentState.New, cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }
}
