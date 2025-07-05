using ServiceNow.Clients;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class ImportSetApiClientTests {
    private static (ImportSetApiClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent("{}") }
        };
        return (new ImportSetApiClient(mock), mock);
    }

    [Fact]
    public async Task ImportAsync_SendsPost() {
        var (client, mock) = Create();

        await client.ImportAsync("u_table", new { name = "foo" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/import/u_table", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task ImportAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new ImportSetApiClient(mock);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.ImportAsync("u_table", new { }, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }
}
