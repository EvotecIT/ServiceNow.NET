namespace ServiceNow.Models;

/// <summary>
/// Represents a dependency between two service map nodes.
/// </summary>
public class ServiceMapDependency {
    /// <summary>
    /// Gets or sets the parent node identifier.
    /// </summary>
    public string? Parent { get; set; }

    /// <summary>
    /// Gets or sets the child node identifier.
    /// </summary>
    public string? Child { get; set; }
}
