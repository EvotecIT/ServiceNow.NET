namespace ServiceNow.Models;

/// <summary>
/// Represents a node in a service map.
/// </summary>
public class ServiceMapNode {
    /// <summary>
    /// Gets or sets the node identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the node.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the node type.
    /// </summary>
    public string? Type { get; set; }
}
