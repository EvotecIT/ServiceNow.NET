using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using ServiceNow;
using System.Net.Http;

namespace ServiceNow.Tests;

public class RoleApiClientTests {
    private static (RoleApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new RoleApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetRoleAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var role = await client.GetRoleAsync("1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/sys_user_role/1", mock.LastRelativeUrl);
        Assert.NotNull(role);
        Assert.Equal("1", role!.SysId);
    }

    [Fact]
    public async Task ListRolesAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var options = new TableQueryOptions { Query = "name=admin" };

        var list = await client.ListRolesAsync(options, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_role?" + options.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }
}
