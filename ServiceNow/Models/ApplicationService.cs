namespace ServiceNow.Models;

/// <summary>
/// Represents an application service in ServiceNow.
/// </summary>
public class ApplicationService {
    /// <summary>
    /// Gets or sets the unique identifier of the service.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string? Name { get; set; }
}
