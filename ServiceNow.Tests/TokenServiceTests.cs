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
}
