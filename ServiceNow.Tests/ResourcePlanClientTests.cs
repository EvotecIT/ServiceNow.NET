using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class ResourcePlanClientTests {
    private static (ResourcePlanClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new ResourcePlanClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetResourcePlanAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var plan = await client.GetResourcePlanAsync("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_rm/v2/resource/plan/1", mock.LastRelativeUrl);
        Assert.NotNull(plan);
        Assert.Equal("1", plan!.SysId);
    }

    [Fact]
    public async Task ListResourcePlansAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["name"] = "test" };

        var list = await client.ListResourcePlansAsync(filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/sn_rm/v1/resource/plan?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }
}
