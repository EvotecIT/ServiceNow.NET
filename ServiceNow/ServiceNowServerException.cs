using System.Net;

namespace ServiceNow;

/// <summary>
/// Exception thrown for server-side errors.
/// </summary>
public class ServiceNowServerException : ServiceNowException {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowServerException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="content">Response content.</param>
    public ServiceNowServerException(HttpStatusCode statusCode, string? content)
        : base(statusCode, content) { }
}
