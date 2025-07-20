namespace ServiceNow.Models;

/// <summary>
/// Represents a resource plan record.
/// </summary>
public class ResourcePlan {
    /// <summary>
    /// Gets or sets the unique identifier of the resource plan.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the name or number of the plan.
    /// </summary>
    public string? Name { get; set; }
}
