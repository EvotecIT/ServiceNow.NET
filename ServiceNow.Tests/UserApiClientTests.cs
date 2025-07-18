using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ServiceNow.Tests;

public class UserApiClientTests {
    private static (UserApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new UserApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task GetUserAsync_SendsGet() {
        var (client, mock) = Create("v2");
        mock.Response.Content = new StringContent("{\"SysId\":\"1\"}");

        var user = await client.GetUserAsync("1", null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/sys_user/1", mock.LastRelativeUrl);
        Assert.NotNull(user);
        Assert.Equal("1", user!.SysId);
    }

    [Fact]
    public async Task CreateUserAsync_SendsPost() {
        var (client, mock) = Create();

        await client.CreateUserAsync(new SysUser { UserName = "u" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task UpdateGroupAsync_SendsPut() {
        var (client, mock) = Create();

        await client.UpdateGroupAsync("2", new SysUserGroup { Name = "g" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_group/2", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task DeleteGroupAsync_SendsDelete() {
        var (client, mock) = Create();

        await client.DeleteGroupAsync("3", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_user_group/3", mock.LastRelativeUrl);
    }
}
