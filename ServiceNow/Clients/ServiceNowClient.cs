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
    private readonly ServiceNowSettings _settings;
    private readonly TokenService _tokenService;

    public ServiceNowSettings Settings => _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowClient"/> class.
    /// </summary>
    /// <param name="httpClient">The underlying HTTP client.</param>
    /// <param name="settings">Client configuration settings.</param>
    /// <param name="tokenService">Service handling OAuth tokens.</param>
    public ServiceNowClient(HttpClient httpClient, ServiceNowSettings settings, TokenService tokenService) {
        if (string.IsNullOrEmpty(settings.BaseUrl)) {
            throw new ArgumentException("BaseUrl is required", nameof(settings));
        }

        _httpClient = httpClient;
        _settings = settings;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Sends a GET request to the specified relative URL.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> GetAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        await _tokenService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        return await _httpClient.GetAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with the provided payload.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="payload">Payload object.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T payload, CancellationToken cancellationToken = default) {
        await _tokenService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
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
        await _tokenService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var content = new StringContent(JsonSerializer.Serialize(payload, ServiceNowJson.Default), Encoding.UTF8, "application/json");
        return await _httpClient.PutAsync(relativeUrl, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URL.
    /// </summary>
    /// <param name="relativeUrl">Relative request URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<HttpResponseMessage> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default) {
        await _tokenService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        return await _httpClient.DeleteAsync(relativeUrl, cancellationToken).ConfigureAwait(false);
    }
}