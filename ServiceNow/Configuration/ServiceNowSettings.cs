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
    /// Optional custom user agent string.
    /// </summary>
    public string UserAgent { get; set; } = "ServiceNow.NET";
}