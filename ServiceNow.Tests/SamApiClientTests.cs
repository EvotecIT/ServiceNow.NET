using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class SamApiClientTests {
    private static (SamApiClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new SamApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetSoftwareAssetAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var asset = await client.GetSoftwareAssetAsync("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sam/v2/software/1", mock.LastRelativeUrl);
        Assert.NotNull(asset);
        Assert.Equal("1", asset!.SysId);
    }

    [Fact]
    public async Task ListSoftwareAssetsAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["name"] = "asset" };

        var list = await client.ListSoftwareAssetsAsync(filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sam/v2/software?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task UsesCustomApiVersionInUrls() {
        var (client, mock) = Create("v1");

        await client.GetSoftwareAssetAsync("42", CancellationToken.None);

        Assert.Equal("/api/sn_sam/v1/software/42", mock.LastRelativeUrl);
    }
}
