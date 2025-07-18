using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;
using ServiceNow.Extensions;

namespace ServiceNow.Tests;

public class CatalogItemClientTests {
    private static (CatalogItemClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new CatalogItemClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetItemsAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var list = await client.GetItemsAsync<CatalogItem>(null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sc/v1/catalog/items", mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetItemAsync_SendsGet() {
        var (client, mock) = Create();
        await client.GetItemAsync<CatalogItem>("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sc/v1/catalog/items/1", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task OrderItemAsync_SendsPost() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"result\":{\"request_number\":\"R1\"}}");

        var number = await client.OrderItemAsync("2", new { qty = 1 }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/sn_sc/v1/catalog/items/2/order_now", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
        Assert.Equal("R1", number);
    }
}
