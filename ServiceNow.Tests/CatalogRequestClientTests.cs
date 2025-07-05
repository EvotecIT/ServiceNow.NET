using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class CatalogRequestClientTests {
    private static (CatalogRequestClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings();
        return (new CatalogRequestClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetRequestAsync_SendsGet() {
        var (client, mock) = Create();
        await client.GetRequestAsync<CatalogRequest>("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sc/v2/request/1", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task GetApprovalsAsync_SendsGet() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var approvals = await client.GetApprovalsAsync<CatalogApproval>("2", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_sc/v2/request/2/approvals", mock.LastRelativeUrl);
        Assert.NotNull(approvals);
    }
}
