using ServiceNow.Clients;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;

namespace ServiceNow.Tests;

public class EventApiClientTests {
    private static (EventApiClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        return (new EventApiClient(mock), mock);
    }

    [Fact]
    public async Task SendEventAsync_SendsPost() {
        var (client, mock) = Create();

        await client.SendEventAsync(new { source = "test" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/global/em_event", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task SendEventAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new EventApiClient(mock);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.SendEventAsync(new { }, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task SendEventAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNow.Configuration.ServiceNowSettings { BaseUrl = "https://example.com", Username = "u", Password = "p" };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNow.Clients.ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<EventApiClient>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.SendEventAsync(new { }, cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }
}
