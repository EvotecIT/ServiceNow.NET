using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class ReportApiClientTests {
    private static (ReportApiClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new ReportApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetReportAsync_SendsCorrectRequest() {
        var (client, mock) = Create();

        await client.GetReportAsync<object>("weekly", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/report/weekly", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task ListReportsAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["type"] = "analytics" };

        var list = await client.ListReportsAsync<object>(filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/report?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task UsesCustomApiVersionInUrls() {
        var (client, mock) = Create("v1");

        await client.GetReportAsync<object>("r1", null, CancellationToken.None);

        Assert.Equal("/api/now/v1/report/r1", mock.LastRelativeUrl);
    }
}
