using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class DomainApiClientTests {
    private static (DomainApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new DomainApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task ListDomainsAsync_SendsGetWithFilters() {
        var (client, mock) = Create();
        var filters = new Dictionary<string, string?> { ["name"] = "core" };

        var list = await client.ListDomainsAsync(filters, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_domain?" + filters.ToQueryString(), mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task UpdateDomainAsync_SendsPut() {
        var (client, mock) = Create();

        await client.UpdateDomainAsync("1", new SysDomain { Name = "core" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v1/table/sys_domain/1", mock.LastRelativeUrl);
    }
}
