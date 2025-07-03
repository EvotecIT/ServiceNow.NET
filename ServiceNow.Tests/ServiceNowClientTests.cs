using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using ServiceNow.Clients;
using ServiceNow.Configuration;

namespace ServiceNow.Tests;

public class ServiceNowClientTests
{
    private static ServiceNowClient CreateClient(MockHttpMessageHandler handler)
    {
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings
        {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            UserAgent = "TestAgent"
        };
        return new ServiceNowClient(http, settings);
    }

    [Fact]
    public void Constructor_SetsBaseAddressAndHeaders()
    {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings
        {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            UserAgent = "TestAgent"
        };

        var client = new ServiceNowClient(http, settings);

        Assert.Equal(new Uri("https://example.com"), http.BaseAddress);
        Assert.Equal("Basic", http.DefaultRequestHeaders.Authorization?.Scheme);
        var param = http.DefaultRequestHeaders.Authorization?.Parameter;
        Assert.Equal(Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("user:pass")), param);
        Assert.Contains(http.DefaultRequestHeaders.UserAgent, h => h.Product?.Name == "TestAgent");
    }

    [Fact]
    public async Task GetAsync_SendsGetRequest()
    {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.GetAsync("/path");

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task PostAsync_SendsPostRequestWithJson()
    {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.PostAsync("/path", new { Name = "foo" });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"foo\"}", handler.RequestContent);
        Assert.Equal("application/json", handler.LastRequest!.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PutAsync_SendsPutRequestWithJson()
    {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.PutAsync("/path", new { Name = "bar" });

        Assert.Equal(HttpMethod.Put, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"bar\"}", handler.RequestContent);
        Assert.Equal("application/json", handler.LastRequest!.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteRequest()
    {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.DeleteAsync("/path");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
    }
}
