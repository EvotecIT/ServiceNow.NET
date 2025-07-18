using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Threading;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class CmdbRelationsClientTests {
    private static (CmdbRelationsClient Client, MockServiceNowClient Mock) Create(string version = "v2") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new CmdbRelationsClient(mock, settings), mock);
    }

    [Fact]
    public async Task ListRelationshipsAsync_SendsCorrectRequest() {
        var (client, mock) = Create();

        await client.ListRelationshipsAsync<object>("cmdb_ci", "1", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v2/cmdb/instance/cmdb_ci/1/relationships", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task UsesCustomApiVersionInUrls() {
        var (client, mock) = Create("v1");

        await client.ListRelationshipsAsync<object>("cmdb_ci", "2", CancellationToken.None);

        Assert.Equal("/api/now/v1/cmdb/instance/cmdb_ci/2/relationships", mock.LastRelativeUrl);
    }
}
