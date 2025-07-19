using System.Net;

namespace ServiceNow;

/// <summary>
/// Exception thrown when authorization fails.
/// </summary>
public class ServiceNowAuthorizationException : ServiceNowException {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowAuthorizationException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="content">Response content.</param>
    public ServiceNowAuthorizationException(HttpStatusCode statusCode, string? content)
        : base(statusCode, content) { }
}
