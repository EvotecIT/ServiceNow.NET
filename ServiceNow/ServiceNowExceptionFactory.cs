using System.Net;

namespace ServiceNow;

/// <summary>
/// Creates specialized exceptions based on HTTP status codes.
/// </summary>
public static class ServiceNowExceptionFactory {
    /// <summary>
    /// Creates an appropriate <see cref="ServiceNowException"/> instance for the given status code.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="content">Response content.</param>
    public static ServiceNowException Create(HttpStatusCode statusCode, string? content)
        => statusCode switch {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new ServiceNowAuthorizationException(statusCode, content),
            HttpStatusCode.NotFound => new ServiceNowNotFoundException(statusCode, content),
            (HttpStatusCode)429 => new ServiceNowRateLimitException(statusCode, content),
            >= HttpStatusCode.InternalServerError => new ServiceNowServerException(statusCode, content),
            _ => new ServiceNowException(statusCode, content)
        };
}
