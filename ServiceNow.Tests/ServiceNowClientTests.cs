using ServiceNow.Clients;
using ServiceNow.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ServiceNow.Tests;

public class ServiceNowClientTests {
    private static ServiceNowClient CreateClient(MockHttpMessageHandler handler) {
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            UserAgent = "TestAgent"
        };
        return new ServiceNowClient(http, settings);
    }

    [Fact]
    public void Constructor_SetsBaseAddressAndHeaders() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
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
    public void Constructor_NullBaseUrl_Throws() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings { Username = "user", Password = "pass" };

        Assert.Throws<ArgumentException>(() => new ServiceNowClient(http, settings));
    }

    [Fact]
    public void Constructor_EmptyBaseUrl_Throws() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings { BaseUrl = string.Empty, Username = "user", Password = "pass" };

        Assert.Throws<ArgumentException>(() => new ServiceNowClient(http, settings));
    }

    [Fact]
    public async Task GetAsync_SendsGetRequest() {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.GetAsync("/path", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task PostAsync_SendsPostRequestWithJson() {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.PostAsync("/path", new { Name = "foo" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"foo\"}", handler.RequestContent);
        Assert.Equal("application/json", handler.LastRequest!.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PutAsync_SendsPutRequestWithJson() {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.PutAsync("/path", new { Name = "bar" }, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"bar\"}", handler.RequestContent);
        Assert.Equal("application/json", handler.LastRequest!.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PatchAsync_SendsPatchRequestWithJson() {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.PatchAsync("/path", new { Name = "baz" }, CancellationToken.None);

        Assert.Equal(new HttpMethod("PATCH"), handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
        Assert.Equal("{\"Name\":\"baz\"}", handler.RequestContent);
        Assert.Equal("application/json", handler.LastRequest!.Content?.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task DeleteAsync_SendsDeleteRequest() {
        var handler = new MockHttpMessageHandler();
        var client = CreateClient(handler);

        await client.DeleteAsync("/path", CancellationToken.None);

        Assert.Equal(HttpMethod.Delete, handler.LastRequest?.Method);
        Assert.Equal("https://example.com/path", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetAsync_CancelledToken_Throws() {
        var handler = new CancelMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            UserAgent = "TestAgent"
        };
        var client = new ServiceNowClient(http, settings);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetAsync("/path", cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }

    [Fact]
    public async Task GetAsync_UsesBearerToken_WhenProvided() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            Token = "abc"
        };

        var client = new ServiceNowClient(http, settings);
        await client.GetAsync("/path", CancellationToken.None);

        Assert.Equal("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
        Assert.Equal("abc", handler.LastRequest?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_RetrievesToken_WhenMissing() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("{\"access_token\":\"t1\"}")
        });
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("{}")
        });
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            ClientId = "cid",
            ClientSecret = "secret",
            TokenUrl = "https://example.com/token"
        };
        var client = new ServiceNowClient(http, settings);

        await client.GetAsync("/resource", CancellationToken.None);

        Assert.Equal("https://example.com/token", handler.Requests[0].RequestUri?.ToString());
        Assert.Equal("Bearer", handler.Requests[1].Headers.Authorization?.Scheme);
        Assert.Equal("t1", handler.Requests[1].Headers.Authorization?.Parameter);
    }
}