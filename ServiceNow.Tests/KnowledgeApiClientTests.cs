using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Models;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class KnowledgeApiClientTests {
    private static (KnowledgeApiClient Client, MockServiceNowClient Mock) Create(string version = "v1") {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings { ApiVersion = version };
        return (new KnowledgeApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task SearchArticlesAsync_SendsGet() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var list = await client.SearchArticlesAsync<KnowledgeArticle>(null, CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/knowledge", mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task SearchArticlesAsync_WithFilters_AppendsQuery() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");
        var filters = new Dictionary<string, string?> { ["category"] = "1" };

        await client.SearchArticlesAsync<KnowledgeArticle>(filters, CancellationToken.None);

        Assert.Equal("/api/now/v1/knowledge?" + filters.ToQueryString(), mock.LastRelativeUrl);
    }

    [Fact]
    public async Task GetArticleAsync_SendsGet() {
        var (client, mock) = Create();
        await client.GetArticleAsync<KnowledgeArticle>("2", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/knowledge/2", mock.LastRelativeUrl);
    }

    [Fact]
    public async Task GetCategoriesAsync_SendsGet() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var list = await client.GetCategoriesAsync<KnowledgeCategory>(CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/knowledge/categories", mock.LastRelativeUrl);
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetAttachmentsAsync_SendsGet() {
        var (client, mock) = Create();
        mock.Response.Content = new StringContent("[]");

        var list = await client.GetAttachmentsAsync<object>("3", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/v1/knowledge/3/attachments", mock.LastRelativeUrl);
        Assert.NotNull(list);
    }
}
