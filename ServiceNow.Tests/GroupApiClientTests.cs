using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ServiceNow.Tests;

public class GroupApiClientTests {
    private static (GroupApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new GroupApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetGroupAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var group = await client.GetGroupAsync("1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/sys_user_group/1", mock.LastRelativeUrl);
        Assert.NotNull(group);
        Assert.Equal("1", group!.SysId);
    }

    [Fact]
    public async Task CreateGroupAsync_SendsPost() {
        var (client, mock) = Create();

        await client.CreateGroupAsync(new SysUserGroup { Name = "g" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_group", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task UpdateGroupAsync_SendsPut() {
        var (client, mock) = Create();

        await client.UpdateGroupAsync("2", new SysUserGroup { Name = "g" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_group/2", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task DeleteGroupAsync_SendsDelete() {
        var (client, mock) = Create();

        await client.DeleteGroupAsync("3", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_group/3", mock.LastRelativeUrl);
    }
}
