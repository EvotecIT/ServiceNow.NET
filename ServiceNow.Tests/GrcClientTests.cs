using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class GrcClientTests {
    private static (GrcClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new GrcClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetItemAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var item = await client.GetItemAsync("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_grc/v2/grc/items/1", mock.LastRelativeUrl);
        Assert.NotNull(item);
        Assert.Equal("1", item!.SysId);
    }

    [Fact]
    public async Task CreateItemAsync_SendsPost() {
        var (client, mock) = Create();

        await client.CreateItemAsync(new GrcItem { Name = "g" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/sn_grc/v1/grc/items", mock.LastRelativeUrl);
    }
}
