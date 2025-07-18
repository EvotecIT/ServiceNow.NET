using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class DataExportClientTests {
    private static (DataExportClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new DataExportClient(mock, settings), mock);
    }

    [Fact]
    public async Task StartExportAsync_SendsPostAndReturnsId() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"result\":{\"export_id\":\"e1\"}}");

        var id = await client.StartExportAsync(new { table = "x" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/v2/export", mock.LastRelativeUrl);
        Assert.Equal("e1", id);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task DownloadExportAsync_SendsGet() {
        var (client, mock) = Create();

        var response = await client.DownloadExportAsync("e1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/export/e1/file", mock.LastRelativeUrl);
        Assert.Equal(mock.Response, response);
    }

    [Fact]
    public async Task StartExportAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new DataExportClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.StartExportAsync(new { }, CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }

    [Fact]
    public async Task DownloadExportAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") }
        };
        var client = new DataExportClient(mock, new ServiceNowSettings());

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.DownloadExportAsync("e1", CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }
}