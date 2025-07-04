using ServiceNow.Configuration;
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

        if (!string.IsNullOrEmpty(_settings.Token)) {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);
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
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.Token);
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