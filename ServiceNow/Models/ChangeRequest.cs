namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow change request record.
/// </summary>
public class ChangeRequest {
    /// <summary>
    /// Gets or sets the unique identifier of the change request.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the change request number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the short description of the request.
    /// </summary>
    public string? ShortDescription { get; set; }
}
