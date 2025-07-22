using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;

using ServiceNow;
using System.Linq;
using ServiceNow.Extensions;
namespace ServiceNow.Tests;

public class ApplicationServiceClientTests {
    private static (ApplicationServiceClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new ApplicationServiceClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetServiceAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var svc = await client.GetServiceAsync("1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/cmdb_ci_service/1", mock.LastRelativeUrl);
        Assert.NotNull(svc);
        Assert.Equal("1", svc!.SysId);
    }

    [Fact]
    public async Task ListServicesAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var options = new TableQueryOptions { Query = "name=svc" };

        var list = await client.ListServicesAsync(options, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/cmdb_ci_service?" + options.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetServiceMapAsync_UsesConstantPath() {
        var (client, mock) = Create();
        await client.GetServiceMapAsync("123", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/cmdb_ci_service/123/service-map", mock.LastRelativeUrl);
    }

    [Fact]
    public void ServiceMap_Traverse_VisitsDependencies() {
        var map = new ServiceMap {
            Nodes = [ new ServiceMapNode { Id = "1" }, new ServiceMapNode { Id = "2" }, new ServiceMapNode { Id = "3" } ],
            Dependencies = [ new ServiceMapDependency { Parent = "1", Child = "2" }, new ServiceMapDependency { Parent = "2", Child = "3" } ]
        };

        var visited = map.Traverse("1").Select(n => n.Id).ToList();

        Assert.Equal(new[] { "1", "2", "3" }, visited);
    }
}
