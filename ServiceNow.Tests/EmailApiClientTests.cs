using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class EmailApiClientTests {
    private static (EmailApiClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new EmailApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task SendEmailAsync_SendsPost() {
        var (client, mock) = Create();

        await client.SendEmailAsync(new { subject = "hi" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal($"/api/now/v2/email/outbound", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task SendEmailAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new EmailApiClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.SendEmailAsync(new { }, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task GetInboundEmailAsync_SendsGet() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"id\":\"1\"}");

        var result = await client.GetInboundEmailAsync<InboundEmail>("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal($"/api/now/v2/email/inbound/1", mock.LastRelativeUrl);
        Assert.NotNull(result);
        Assert.Equal("1", result!.Id);
    }

    private class InboundEmail {
        public string? Id { get; set; }
    }
}
