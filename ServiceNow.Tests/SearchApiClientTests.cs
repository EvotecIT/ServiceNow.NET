using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;
using Xunit;

namespace ServiceNow.Tests;

public class SearchApiClientTests {
    private static (SearchApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new SearchApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task SearchKnowledgeAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        await client.SearchKnowledgeAsync<object>("test", CancellationToken.None);
        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/search?sysparm_search=test&sysparm_target=kb_knowledge", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task SearchCatalogAsync_SendsCorrectRequest() {
        var (client, mock) = Create();
        await client.SearchCatalogAsync<object>("item", CancellationToken.None);
        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/search?sysparm_search=item&sysparm_target=sc_cat_item", mock.LastRelativeUrl);
    }
}
