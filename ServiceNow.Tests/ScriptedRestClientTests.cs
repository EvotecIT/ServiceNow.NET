using ServiceNow.Clients;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class ScriptedRestClientTests {
    private static (ScriptedRestClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"value\":1}") }
        };
        var client = new ScriptedRestClient(mock);
        return (client, mock);
    }

    [Fact]
    public async Task GetAsync_SendsGetAndReturnsData() {
        var (client, mock) = Create();

        var result = await client.GetAsync<TestResponse>("/api/x/do", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/x/do", mock.LastRelativeUrl);
        Assert.NotNull(result);
        Assert.Equal(1, result!.Value);
    }

    [Fact]
    public async Task PostAsync_SendsPostAndReturnsData() {
        var (client, mock) = Create();

        var result = await client.PostAsync<TestResponse>("/api/x/do", new { x = 1 }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/x/do", mock.LastRelativeUrl);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task PutAsync_SendsPutAndReturnsData() {
        var (client, mock) = Create();

        var result = await client.PutAsync<TestResponse>("/api/x/do", new { x = 1 }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/x/do", mock.LastRelativeUrl);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_SendsDelete() {
        var (client, mock) = Create();

        await client.DeleteAsync("/api/x/do", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, mock.LastMethod);
        Assert.Equal("/api/x/do", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task ErrorResponse_ThrowsServiceNowException() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new ScriptedRestClient(mock);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.GetAsync<TestResponse>("/x", CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    private class TestResponse {
        public int Value { get; set; }
    }
}
