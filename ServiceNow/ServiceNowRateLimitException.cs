using System.Net;

namespace ServiceNow;

/// <summary>
/// Exception thrown when rate limits are exceeded.
/// </summary>
public class ServiceNowRateLimitException : ServiceNowException {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowRateLimitException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="content">Response content.</param>
    public ServiceNowRateLimitException(HttpStatusCode statusCode, string? content)
        : base(statusCode, content) { }
}
