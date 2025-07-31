using ServiceNow.Clients;
using ServiceNow.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using ServiceNow.Extensions;
using System.Text.Json;

namespace ServiceNow.Tests;

public class ServiceNowClientTests {
    private static ServiceNowClient CreateClient(HttpMessageHandler handler) {
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            UserAgent = "TestAgent"
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        return (ServiceNowClient)provider.GetRequiredService<IServiceNowClient>();
    }

    [Fact]
    public void AddServiceNow_ConfiguresHttpClient() {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass",
            Timeout = TimeSpan.FromSeconds(5),
            UserAgent = "TestAgent"
        };

        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient(ServiceNowClient.HttpClientName);

        Assert.Equal(new Uri("https://example.com"), client.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(5), client.Timeout);
        Assert.Equal("Basic", client.DefaultRequestHeaders.Authorization?.Scheme);
        var param = client.DefaultRequestHeaders.Authorization?.Parameter;
        Assert.Equal(Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("user:pass")), param);
        Assert.Contains(client.DefaultRequestHeaders.UserAgent, h => h.Product?.Name == "TestAgent");
    }

    [Fact]
    public void AddServiceNow_MissingUsername_Throws() {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Password = "pass"
        };

        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        Assert.Throws<ArgumentException>(() => factory.CreateClient(ServiceNowClient.HttpClientName));
    }

    [Fact]
    public void AddServiceNow_MissingPassword_Throws() {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user"
        };

        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        Assert.Throws<ArgumentException>(() => factory.CreateClient(ServiceNowClient.HttpClientName));
    }


    [Fact]
    public void Constructor_NullBaseUrl_Throws() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings { Username = "user", Password = "pass" };

        var tokenService = new TokenService(http, settings);
        Assert.Throws<ArgumentException>(() => new ServiceNowClient(http, settings, tokenService));
    }

    [Fact]
    public void Constructor_EmptyBaseUrl_Throws() {
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings { BaseUrl = string.Empty, Username = "user", Password = "pass" };

        var tokenService = new TokenService(http, settings);
        Assert.Throws<ArgumentException>(() => new ServiceNowClient(http, settings, tokenService));
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
        var client = CreateClient(handler);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetAsync("/path", cts.Token));
        Assert.True(cts.IsCancellationRequested);
    }

    [Fact]
    public async Task GetAsync_UsesBearerToken_WhenProvided() {
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            Token = "abc"
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IServiceNowClient>();
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
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            ClientId = "cid",
            ClientSecret = "secret",
            TokenUrl = "https://example.com/token"
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IServiceNowClient>();

        await client.GetAsync("/resource", CancellationToken.None);

        Assert.Equal("https://example.com/token", handler.Requests[0].RequestUri?.ToString());
        Assert.Equal("Bearer", handler.Requests[1].Headers.Authorization?.Scheme);
        Assert.Equal("t1", handler.Requests[1].Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_UsesStoredToken() {
        var store = new InMemoryTokenStore {
            Token = new TokenInfo {
                AccessToken = "stored",
                RefreshToken = "r1",
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            }
        };
        var handler = new MockHttpMessageHandler();
        var services = new ServiceCollection();
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            TokenStore = store
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IServiceNowClient>();

        await client.GetAsync("/path", CancellationToken.None);

        Assert.Equal("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
        Assert.Equal("stored", handler.LastRequest?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_RefreshesExpiredToken() {
        var handler = new SequenceMessageHandler();
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("{\"access_token\":\"new\",\"refresh_token\":\"r2\",\"expires_in\":3600}")
        });
        handler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });
        var services = new ServiceCollection();
        var store = new InMemoryTokenStore {
            Token = new TokenInfo {
                AccessToken = "old",
                RefreshToken = "r1",
                Expires = DateTimeOffset.UtcNow.AddSeconds(-5)
            }
        };
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            TokenUrl = "https://example.com/token",
            TokenStore = store
        };
        services.AddServiceNow(settings);
        services.AddHttpClient(ServiceNowClient.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IServiceNowClient>();

        await client.GetAsync("/resource", CancellationToken.None);

        Assert.Contains("grant_type=refresh_token", handler.RequestContents[0]);
        Assert.Contains("refresh_token=r1", handler.RequestContents[0]);
        Assert.Equal("new", store.Token?.AccessToken);
        Assert.Equal("r2", store.Token?.RefreshToken);
        Assert.Equal("Bearer", handler.Requests[1].Headers.Authorization?.Scheme);
        Assert.Equal("new", handler.Requests[1].Headers.Authorization?.Parameter);
    }
}