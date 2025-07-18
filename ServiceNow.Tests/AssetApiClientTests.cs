using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;

namespace ServiceNow.Tests;

public class AssetApiClientTests {
    private static (AssetApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new AssetApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetAssetAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var asset = await client.GetAssetAsync("1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/alm_asset/1", mock.LastRelativeUrl);
        Assert.NotNull(asset);
        Assert.Equal("1", asset!.SysId);
    }

    [Fact]
    public async Task ListAssetsAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["active"] = "true" };

        var list = await client.ListAssetsAsync(filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/alm_asset?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }
}
