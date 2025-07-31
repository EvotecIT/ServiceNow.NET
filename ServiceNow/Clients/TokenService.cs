using ServiceNow.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using ServiceNow.Extensions;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Handles acquiring and refreshing OAuth tokens for ServiceNow.
/// </summary>
public class TokenService {
    private readonly HttpClient _httpClient;
    private readonly ServiceNowSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client used for token requests.</param>
    /// <param name="settings">Configuration settings.</param>
    public TokenService(HttpClient httpClient, ServiceNowSettings settings) {
        _httpClient = httpClient;
        _settings = settings;
    }

    /// <summary>
    /// Ensures a valid access token is present and attached to the HTTP client.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task EnsureTokenAsync(CancellationToken cancellationToken) {
        if (!_settings.UseOAuth) {
            return;
        }

        if (string.IsNullOrEmpty(_settings.Token) && _settings.TokenStore is not null) {
            var stored = await _settings.TokenStore.LoadAsync(cancellationToken).ConfigureAwait(false);
            if (stored is not null) {
                _settings.Token = stored.AccessToken;
                _settings.RefreshToken = stored.RefreshToken;
                _settings.TokenExpires = stored.Expires;
            }
        }

        if (!string.IsNullOrEmpty(_settings.Token) && (_settings.TokenExpires == default || _settings.TokenExpires > DateTimeOffset.UtcNow)) {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);
            return;
        }

        if (!string.IsNullOrEmpty(_settings.RefreshToken)) {
            var refreshPairs = new Dictionary<string, string> {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _settings.RefreshToken
            };
            if (!string.IsNullOrEmpty(_settings.ClientId)) {
                refreshPairs["client_id"] = _settings.ClientId!;
            }
            if (!string.IsNullOrEmpty(_settings.ClientSecret)) {
                refreshPairs["client_secret"] = _settings.ClientSecret!;
            }
            await AcquireTokenAsync(refreshPairs, cancellationToken).ConfigureAwait(false);
            return;
        }

        var pairs = new Dictionary<string, string> {
            ["grant_type"] = !string.IsNullOrEmpty(_settings.ClientId) &&
                             !string.IsNullOrEmpty(_settings.ClientSecret) &&
                             string.IsNullOrEmpty(_settings.Username)
                ? "client_credentials"
                : "password"
        };

        if (pairs["grant_type"] == "client_credentials") {
            pairs["client_id"] = _settings.ClientId!;
            pairs["client_secret"] = _settings.ClientSecret!;
        } else {
            pairs["username"] = _settings.Username!;
            pairs["password"] = _settings.Password!;
            if (!string.IsNullOrEmpty(_settings.ClientId)) {
                pairs["client_id"] = _settings.ClientId!;
            }
            if (!string.IsNullOrEmpty(_settings.ClientSecret)) {
                pairs["client_secret"] = _settings.ClientSecret!;
            }
        }

        await AcquireTokenAsync(pairs, cancellationToken).ConfigureAwait(false);
    }

    private async Task AcquireTokenAsync(Dictionary<string, string> pairs, CancellationToken cancellationToken) {
        var content = new FormUrlEncodedContent(pairs);
        var response = await _httpClient.PostAsync(_settings.TokenUrl, content, cancellationToken)
            .ConfigureAwait(false);
        await response.EnsureServiceNowSuccessAsync().ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        _settings.Token = doc.RootElement.GetProperty("access_token").GetString();
        if (doc.RootElement.TryGetProperty("refresh_token", out var rt)) {
            _settings.RefreshToken = rt.GetString();
        }
        if (doc.RootElement.TryGetProperty("expires_in", out var exp)) {
            int seconds;
            if (exp.ValueKind == JsonValueKind.Number && exp.TryGetInt32(out var num)) {
                seconds = num;
            } else if (exp.ValueKind == JsonValueKind.String && int.TryParse(exp.GetString(), out var parsed)) {
                seconds = parsed;
            } else {
                seconds = 3600;
            }
            _settings.TokenExpires = DateTimeOffset.UtcNow.AddSeconds(seconds);
        } else {
            _settings.TokenExpires = DateTimeOffset.UtcNow.AddHours(1);
        }
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.Token);

        if (_settings.TokenStore is not null && !string.IsNullOrEmpty(_settings.Token)) {
            var info = new TokenInfo {
                AccessToken = _settings.Token,
                RefreshToken = _settings.RefreshToken,
                Expires = _settings.TokenExpires
            };
            await _settings.TokenStore.SaveAsync(info, cancellationToken).ConfigureAwait(false);
        }
    }
}
