using ServiceNow.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ServiceNow.Utilities;
using ServiceNow.Extensions;

namespace ServiceNow.Clients;

/// <summary>
/// Basic client for interacting with the ServiceNow REST API.
/// </summary>
public class ServiceNowClient : IServiceNowClient {
    /// <summary>
    /// Name used when configuring the underlying <see cref="HttpClient"/>.
    /// </summary>
    public const string HttpClientName = "ServiceNow";

    private readonly HttpClient _httpClient;
    private ServiceNowSettings _settings;

    public ServiceNowSettings Settings => _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    public ServiceNowClient(HttpClient httpClient, ServiceNowSettings settings) {
        if (string.IsNullOrEmpty(settings.BaseUrl)) {
            throw new ArgumentException("BaseUrl is required", nameof(settings));
        }

        _httpClient = httpClient;
        _settings = settings;
    }

    private async Task EnsureTokenAsync(CancellationToken cancellationToken) {
        if (!_settings.UseOAuth) {
            return;
        }

        if (string.IsNullOrEmpty(_settings.Token) && _settings.TokenStore is not null) {
            var stored = await _settings.TokenStore.LoadAsync(cancellationToken).ConfigureAwait(false);
            if (stored is not null) {
                _settings = _settings with {
                    Token = stored.AccessToken,
                    RefreshToken = stored.RefreshToken,
                    TokenExpires = stored.Expires
                };
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
        var token = doc.RootElement.GetProperty("access_token").GetString();
        string? refresh = null;
        if (doc.RootElement.TryGetProperty("refresh_token", out var rt)) {
            refresh = rt.GetString();
        }
        DateTimeOffset expires;
        if (doc.RootElement.TryGetProperty("expires_in", out var exp)) {
            var seconds = exp.GetInt32();
            expires = DateTimeOffset.UtcNow.AddSeconds(seconds);
        } else {
            expires = DateTimeOffset.UtcNow.AddHours(1);
        }
        _settings = _settings with {
            Token = token,
            RefreshToken = refresh,
            TokenExpires = expires
        };
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

    /// <summary>
    /// Sends a GET request to the specified relative URL.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        return await _httpClient.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with the provided payload.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="payload">Payload object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var content = new StringContent(JsonSerializer.Serialize(payload, ServiceNowJson.Default), Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request with the provided payload.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="payload">Payload object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> PutAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var content = new StringContent(JsonSerializer.Serialize(payload, ServiceNowJson.Default), Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URL.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        return await _httpClient.DeleteAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
    }
}