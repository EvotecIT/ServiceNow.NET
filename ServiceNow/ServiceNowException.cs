using System.Net;

namespace ServiceNow;

/// <summary>
/// Exception thrown when a ServiceNow API call returns an unsuccessful response.
/// </summary>
public class ServiceNowException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNowException"/> class.
    /// </summary>
    /// <param name="statusCode">HTTP status code returned by the API.</param>
    /// <param name="content">Response content.</param>
    public ServiceNowException(HttpStatusCode statusCode, string? content)
        : base($"Request failed with status code {(int)statusCode} ({statusCode})") {
        StatusCode = statusCode;
        Content = content;
    }

    /// <summary>
    /// HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Response content returned by the API.
    /// </summary>
    public string? Content { get; }
}
