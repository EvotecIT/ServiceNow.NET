using System.Net;

namespace ServiceNow;

/// <summary>
/// Exception thrown when a resource cannot be found.
/// </summary>
public class ServiceNowNotFoundException : ServiceNowException {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowNotFoundException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="content">Response content.</param>
    public ServiceNowNotFoundException(HttpStatusCode statusCode, string? content)
        : base(statusCode, content) { }
}
