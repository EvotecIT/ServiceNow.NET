using ServiceNow.Clients;
using ServiceNow.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ServiceNow.Tests;

public class TokenServiceTests {
    [Fact]
    public async Task EnsureTokenAsync_UsesStoredToken() {
        var store = new InMemoryTokenStore {
            Token = new TokenInfo {
                AccessToken = "stored",
                RefreshToken = "r1",
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            }
        };
        var handler = new MockHttpMessageHandler();
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            TokenStore = store
        };
        var tokenService = new TokenService(http, settings);

        await tokenService.EnsureTokenAsync(CancellationToken.None);

        Assert.Equal("Bearer", http.DefaultRequestHeaders.Authorization?.Scheme);
        Assert.Equal("stored", http.DefaultRequestHeaders.Authorization?.Parameter);
    }

    [Fact]
    public async Task EnsureTokenAsync_ParsesNumericExpiresIn() {
        var handler = new MockHttpMessageHandler();
        handler.Response.Content = new StringContent("{\"access_token\":\"n\",\"expires_in\":3600}");
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            ClientId = "cid",
            ClientSecret = "secret",
            TokenUrl = "https://example.com/token"
        };
        var tokenService = new TokenService(http, settings);

        var before = DateTimeOffset.UtcNow;
        await tokenService.EnsureTokenAsync(CancellationToken.None);
        var diff = settings.TokenExpires - before;

        Assert.InRange(diff.TotalSeconds, 3595, 3605);
    }

    [Fact]
    public async Task EnsureTokenAsync_ParsesStringExpiresIn() {
        var handler = new MockHttpMessageHandler();
        handler.Response.Content = new StringContent("{\"access_token\":\"n\",\"expires_in\":\"3600\"}");
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            ClientId = "cid",
            ClientSecret = "secret",
            TokenUrl = "https://example.com/token"
        };
        var tokenService = new TokenService(http, settings);

        var before = DateTimeOffset.UtcNow;
        await tokenService.EnsureTokenAsync(CancellationToken.None);
        var diff = settings.TokenExpires - before;

        Assert.InRange(diff.TotalSeconds, 3595, 3605);
    }

    [Fact]
    public async Task EnsureTokenAsync_InvalidExpiresIn_DefaultsToOneHour() {
        var handler = new MockHttpMessageHandler();
        handler.Response.Content = new StringContent("{\"access_token\":\"n\",\"expires_in\":\"x\"}");
        var http = new HttpClient(handler);
        var settings = new ServiceNowSettings {
            BaseUrl = "https://example.com",
            UseOAuth = true,
            ClientId = "cid",
            ClientSecret = "secret",
            TokenUrl = "https://example.com/token"
        };
        var tokenService = new TokenService(http, settings);

        var before = DateTimeOffset.UtcNow;
        await tokenService.EnsureTokenAsync(CancellationToken.None);
        var diff = settings.TokenExpires - before;

        Assert.InRange(diff.TotalSeconds, 3595, 3605);
    }
}
