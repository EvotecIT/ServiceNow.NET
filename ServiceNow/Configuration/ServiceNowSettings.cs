namespace ServiceNow.Configuration;

/// <summary>
/// Settings used for connecting to a ServiceNow instance.
/// </summary>
public class ServiceNowSettings {
    /// <summary>
    /// Base URL of the ServiceNow instance.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Username used for authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password used for authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Use OAuth authentication instead of basic auth.
    /// </summary>
    public bool UseOAuth { get; set; }

    /// <summary>
    /// OAuth client identifier.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// OAuth client secret.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Token endpoint used when obtaining an OAuth token.
    /// </summary>
    public string TokenUrl { get; set; } = "/oauth_token.do";

    /// <summary>
    /// Access token retrieved from the OAuth endpoint.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Optional custom user agent string.
    /// </summary>
    public string UserAgent { get; set; } = "ServiceNow.NET";

    /// <summary>
    /// Timeout for HTTP requests. Defaults to 100 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// ServiceNow REST API version segment. Defaults to "v2".
    /// </summary>
    public string ApiVersion { get; set; } = "v2";
}