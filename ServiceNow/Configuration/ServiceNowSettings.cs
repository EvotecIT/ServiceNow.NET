namespace ServiceNow.Configuration;

/// <summary>
/// Settings used for connecting to a ServiceNow instance.
/// </summary>
public sealed record ServiceNowSettings {
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    public string? BaseUrl { get; init; }

    /// <summary>
    /// Username used for authentication.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Password used for authentication.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Use OAuth authentication instead of basic auth.
    /// </summary>
    public bool UseOAuth { get; init; }

    /// <summary>
    /// OAuth client identifier.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// OAuth client secret.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Token endpoint used when obtaining an OAuth token.
    /// </summary>
    public string TokenUrl { get; init; } = "/oauth_token.do";

    /// <summary>
    /// Access token retrieved from the OAuth endpoint.
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    /// Refresh token used to obtain a new access token.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Expiration time of the access token in UTC.
    /// </summary>
    public DateTimeOffset TokenExpires { get; init; }

    /// <summary>
    /// Optional store used for persisting tokens.
    /// </summary>
    public ITokenStore? TokenStore { get; init; }

    /// <summary>
    /// Optional custom user agent string.
    /// </summary>
    public string UserAgent { get; init; } = "ServiceNow.NET";

    /// <summary>
    /// Timeout for HTTP requests. Defaults to 100 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// ServiceNow REST API version segment. Defaults to "v2".
    /// </summary>
    public string ApiVersion { get; init; } = "v2";

    /// <summary>
    /// Duration to cache table metadata responses. Defaults to 10 minutes.
    /// </summary>
    public TimeSpan MetadataCacheDuration { get; init; } = TimeSpan.FromMinutes(10);
}