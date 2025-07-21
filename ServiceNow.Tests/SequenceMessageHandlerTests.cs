using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using Xunit;

namespace ServiceNow.Tests;

public class SequenceMessageHandlerTests {
    [Fact]
    public async Task PostAsync_RecordsJsonBodyAndHeaders() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
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
        var client = provider.GetRequiredService<IServiceNowClient>();

        await client.PostAsync("/api", new { Name = "foo" }, CancellationToken.None);

        var request = handler.Requests.Single();
        var content = handler.RequestContents.Single();
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("https://example.com/api", request.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"foo\"}", content);
        Assert.Equal("application/json", request.Content!.Headers.ContentType?.MediaType);
        var expectedAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes("user:pass"));
        Assert.Equal("Basic", request.Headers.Authorization?.Scheme);
        Assert.Equal(expectedAuth, request.Headers.Authorization?.Parameter);
    }
}
