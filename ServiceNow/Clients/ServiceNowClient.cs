using ServiceNow.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ServiceNow.Utilities;

namespace ServiceNow.Clients;

/// <summary>
/// Basic client for interacting with the ServiceNow REST API.
/// </summary>
public class ServiceNowClient : IServiceNowClient {
    private readonly HttpClient _httpClient;
    private readonly ServiceNowSettings _settings;

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

        _httpClient.BaseAddress = new Uri(settings.BaseUrl);
        _httpClient.Timeout = settings.Timeout;
        if (!settings.UseOAuth) {
            var authBytes = Encoding.ASCII.GetBytes($"{settings.Username}:{settings.Password}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        }
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.UserAgent);
    }

    private async Task EnsureTokenAsync(CancellationToken cancellationToken) {
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
        if (!response.IsSuccessStatusCode) {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ServiceNowException(response.StatusCode, text);
        }

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);
        _settings.Token = doc.RootElement.GetProperty("access_token").GetString();
        if (doc.RootElement.TryGetProperty("refresh_token", out var rt)) {
            _settings.RefreshToken = rt.GetString();
        }
        if (doc.RootElement.TryGetProperty("expires_in", out var exp)) {
            var seconds = exp.GetInt32();
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